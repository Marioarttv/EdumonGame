using EdumonGame.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static EdumonGame.Models.MoveBase;

namespace EdumonGame.Models
{
    public class Edumon
    {
        public EdumonBase Base { get; set; }
        public int Level { get; set; }

        public Edumon(EdumonBase pBase, int pLevel)
        {
            if (pBase == null)
                throw new ArgumentNullException(nameof(pBase), "EdumonBase cannot be null");

            Base = pBase;
            Level = pLevel;
            Init();
        }

        public int Exp { get; set; }
        public int HP { get; set; }
        public List<Move> Moves { get; set; }
        public Move CurrentMove { get; set; }
        public Dictionary<Stat, int> Stats { get; private set; }
        public Dictionary<Stat, int> StatBoosts { get; private set; }
        public Condition Status { get; private set; }
        public int StatusTime { get; set; }
        public Condition VolatileStatus { get; private set; }
        public int VolatileStatusTime { get; set; }

        public Queue<string> StatusChanges { get; private set; }
        public event Action OnStatusChanged;
        public event Action OnHPChanged;

        private readonly Random random = new Random();

        public void Init()
        {
            // Generate Moves
            Moves = new List<Move>();

            // Ensure LearnableMoves is not null before iterating
            if (Base.LearnableMoves != null)
            {
                foreach (var move in Base.LearnableMoves)
                {
                    if (move.Level <= Level)
                        Moves.Add(new Move(move.MoveBase));

                    if (Moves.Count >= EdumonBase.MaxNumOfMoves)
                        break;
                }
            }

            Exp = Base.GetExpForLevel(Level);

            CalculateStats();
            HP = MaxHp;

            StatusChanges = new Queue<string>();
            ResetStatBoost();
            Status = null;
            VolatileStatus = null;
        }

        // Constructor for loading saved data
        public Edumon(EdumonSaveData saveData)
        {
            Base = EdumonDB.GetObjectByName(saveData.name);
            if (Base == null)
                throw new ArgumentException($"No Edumon found with name: {saveData.name}");

            HP = saveData.hp;
            Level = saveData.level;
            Exp = saveData.exp;

            if (saveData.statusId != null)
                Status = ConditionsDB.Conditions[saveData.statusId.Value];
            else
                Status = null;

            Moves = saveData.moves.Select(s => new Move(s)).ToList();

            CalculateStats();
            StatusChanges = new Queue<string>();
            ResetStatBoost();
            VolatileStatus = null;
        }

        public EdumonSaveData GetSaveData()
        {
            var saveData = new EdumonSaveData()
            {
                name = Base.Name,
                hp = HP,
                level = Level,
                exp = Exp,
                statusId = Status?.Id,
                moves = Moves.Select(m => m.GetSaveData()).ToList()
            };

            return saveData;
        }

        private void CalculateStats()
        {
            Stats = new Dictionary<Stat, int>();
            Stats.Add(Stat.Attack, (int)Math.Floor((Base.Attack * Level) / 100f) + 5);
            Stats.Add(Stat.Defense, (int)Math.Floor((Base.Defense * Level) / 100f) + 5);
            Stats.Add(Stat.SpAttack, (int)Math.Floor((Base.SpAttack * Level) / 100f) + 5);
            Stats.Add(Stat.SpDefense, (int)Math.Floor((Base.SpDefense * Level) / 100f) + 5);
            Stats.Add(Stat.Speed, (int)Math.Floor((Base.Speed * Level) / 100f) + 5);

            int oldMaxHP = MaxHp;
            MaxHp = (int)Math.Floor((Base.MaxHp * Level) / 100f) + 10 + Level;

            if (oldMaxHP != 0)
                HP += MaxHp - oldMaxHP;
        }

        private void ResetStatBoost()
        {
            StatBoosts = new Dictionary<Stat, int>()
            {
                {Stat.Attack, 0},
                {Stat.Defense, 0},
                {Stat.SpAttack, 0},
                {Stat.SpDefense, 0},
                {Stat.Speed, 0},
                {Stat.Accuracy, 0},
                {Stat.Evasion, 0},
            };
        }

        private int GetStat(Stat stat)
        {
            int statVal = Stats[stat];

            // Apply stat boost
            int boost = StatBoosts[stat];
            var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

            if (boost >= 0)
                statVal = (int)Math.Floor(statVal * boostValues[boost]);
            else
                statVal = (int)Math.Floor(statVal / boostValues[-boost]);

            return statVal;
        }

        public void ApplyBoosts(List<StatBoost> statBoosts)
        {
            foreach (var statBoost in statBoosts)
            {
                var stat = statBoost.stat;
                var boost = statBoost.boost;

                StatBoosts[stat] = Math.Clamp(StatBoosts[stat] + boost, -6, 6);

                if (boost > 0)
                    StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
                else
                    StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");

                Console.WriteLine($"{stat} has been boosted to {StatBoosts[stat]}");
            }
        }

        public bool CheckForLevelUp()
        {
            if (Exp > Base.GetExpForLevel(Level + 1))
            {
                ++Level;
                CalculateStats();
                return true;
            }

            return false;
        }

        public LearnableMove GetLearnableMoveAtCurrLevel()
        {
            if (Base.LearnableMoves == null)
                return null;

            return Base.LearnableMoves.FirstOrDefault(x => x.Level == Level);
        }

        public void LearnMove(MoveBase moveToLearn)
        {
            if (Moves.Count > EdumonBase.MaxNumOfMoves)
                return;

            Moves.Add(new Move(moveToLearn));
        }

        public bool HasMove(MoveBase moveToCheck)
        {
            return Moves.Count(m => m.Base == moveToCheck) > 0;
        }

        public Evolution CheckForEvolution()
        {
            if (Base.Evolutions == null)
                return null;

            return Base.Evolutions.FirstOrDefault(e => e.RequiredLevel <= Level);
        }

        public Evolution CheckForEvolution(ItemBase item)
        {
            if (Base.Evolutions == null)
                return null;

            return Base.Evolutions.FirstOrDefault(e => e.RequiredItem == item);
        }

        public void Evolve(Evolution evolution)
        {
            Base = evolution.EvolvesInto;
            CalculateStats();
        }

        public void Heal()
        {
            HP = MaxHp;
            OnHPChanged?.Invoke();

            CureStatus();
        }

        // Properties for stats
        public int Attack => GetStat(Stat.Attack);
        public int Defense => GetStat(Stat.Defense);
        public int SpAttack => GetStat(Stat.SpAttack);
        public int SpDefense => GetStat(Stat.SpDefense);
        public int Speed => GetStat(Stat.Speed);
        public int MaxHp { get; private set; }

        // Battle related methods
        public DamageDetails TakeDamage(Move move, Edumon attacker, float multipleChoiceModifier = 1f)
        {
            float critical = 1f;
            if (random.NextDouble() * 100f <= 6.25f)
                critical = 2f;

            float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) *
                         TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

            var damageDetails = new DamageDetails()
            {
                TypeEffectiveness = type,
                Critical = critical,
                Fainted = false
            };

            float attack = (move.Base.Category == MoveBase.MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
            float defense = (move.Base.Category == MoveBase.MoveCategory.Special) ? SpDefense : Defense;

            float modifiers = (float)(random.NextDouble() * (1.0 - 0.85) + 0.85) * type * critical;
            float a = (2 * attacker.Level + 10) / 250f;
            float d = a * move.Base.Power * ((float)attack / defense) + 2;
            int damage = (int)Math.Floor(d * modifiers);

            if (multipleChoiceModifier < 1f)
            {
                damage = (int)Math.Floor(damage * multipleChoiceModifier);
            }

            DecreaseHP(damage);

            return damageDetails;
        }

        public void IncreaseHP(int amount)
        {
            HP = Math.Clamp(HP + amount, 0, MaxHp);
            OnHPChanged?.Invoke();
        }

        public void DecreaseHP(int damage)
        {
            HP = Math.Clamp(HP - damage, 0, MaxHp);
            OnHPChanged?.Invoke();
        }

        public void SetStatus(ConditionID conditionId)
        {
            if (Status != null) return;

            Status = ConditionsDB.Conditions[conditionId];
            Status?.OnStart?.Invoke(this);
            StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
            OnStatusChanged?.Invoke();
        }

        public void CureStatus()
        {
            Status = null;
            OnStatusChanged?.Invoke();
        }

        public void SetVolatileStatus(ConditionID conditionId)
        {
            if (VolatileStatus != null) return;

            VolatileStatus = ConditionsDB.Conditions[conditionId];
            VolatileStatus?.OnStart?.Invoke(this);
            StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
        }

        public void CureVolatileStatus()
        {
            VolatileStatus = null;
        }

        public Move GetRandomMove()
        {
            var movesWithPP = Moves.ToList();

            if (movesWithPP.Count == 0)
            {
                // Return a default basic move if no moves are available
                return new Move(new MoveBase
                {
                    Id = "struggle",
                    Name = "Struggle",
                    Power = 20,
                    Type = EdumonType.Normal,
                    Category = MoveBase.MoveCategory.Physical
                });
            }

            int r = random.Next(0, movesWithPP.Count);
            return movesWithPP[r];
        }

        public bool OnBeforeMove()
        {
            bool canPerformMove = true;
            if (Status?.OnBeforeMove != null)
            {
                if (!Status.OnBeforeMove(this))
                    canPerformMove = false;
            }

            if (VolatileStatus?.OnBeforeMove != null)
            {
                if (!VolatileStatus.OnBeforeMove(this))
                    canPerformMove = false;
            }

            return canPerformMove;
        }

        public void OnAfterTurn()
        {
            Status?.OnAfterTurn?.Invoke(this);
            VolatileStatus?.OnAfterTurn?.Invoke(this);
        }

        public void OnBattleOver()
        {
            VolatileStatus = null;
            ResetStatBoost();
        }
    }

    public class DamageDetails
    {
        public bool Fainted { get; set; }
        public float Critical { get; set; }
        public float TypeEffectiveness { get; set; }
    }

    public class EdumonSaveData
    {
        public string name;
        public int hp;
        public int level;
        public int exp;
        public ConditionID? statusId;
        public List<MoveSaveData> moves;
    }
}