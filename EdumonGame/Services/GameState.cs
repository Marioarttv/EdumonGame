using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EdumonGame.Models;

namespace EdumonGame.Services
{
    // Manages the current game state
    public class GameState
    {
        private readonly IEdumonDbService _edumonDb;
        private readonly IMoveDbService _moveDb;

        public GameState(IEdumonDbService edumonDb = null, IMoveDbService moveDb = null)
        {
            _edumonDb = edumonDb;
            _moveDb = moveDb;
        }

        // Player's collection of Edumon
        public List<Edumon> PlayerEdumons { get; private set; } = new();

        // Current battle state
        public BattleState CurrentBattle { get; set; }

        // Player data
        public PlayerData Player { get; set; } = new();

        public async Task InitializeNewGameAsync()
        {
            // Create a starter Edumon
            // If we can't load from DB, create a hardcoded default starter
            EdumonBase starterBase = null;

            if (_edumonDb != null)
            {
                // Try to get from the database
                starterBase = _edumonDb.GetEdumonByName("Bulbasaur");
            }

            // If we couldn't get from DB, create a default one
            if (starterBase == null)
            {
                starterBase = CreateDefaultStarter();
            }

            // Now we should have a valid starter base
            var starter = new Edumon(starterBase, 5);
            PlayerEdumons.Add(starter);

            Player = new PlayerData { Name = "Player", Coins = 100 };

            // Other initialization
        }

        // Create a default starter if we can't load from DB
        private EdumonBase CreateDefaultStarter()
        {
            var tackle = new MoveBase
            {
                Id = "tackle",
                Name = "Tackle",
                Description = "A physical attack in which the user charges and slams into the target with its whole body.",
                Type = EdumonType.Normal,
                Power = 40,
                Accuracy = 100,
                Category = MoveBase.MoveCategory.Physical
            };

            var growl = new MoveBase
            {
                Id = "growl",
                Name = "Growl",
                Description = "The user growls in an endearing way, making opposing Pokémon less wary. This lowers their Attack stat.",
                Type = EdumonType.Normal,
                Power = 0,
                Accuracy = 100,
                Category = MoveBase.MoveCategory.Status,
                Effects = new MoveBase.MoveEffects
                {
                    Boosts = new List<MoveBase.StatBoost>
                    {
                        new MoveBase.StatBoost { stat = Stat.Attack, boost = -1 }
                    }
                }
            };

            var vineWhip = new MoveBase
            {
                Id = "vinewhip",
                Name = "Vine Whip",
                Description = "The target is struck with slender, whiplike vines.",
                Type = EdumonType.Grass,
                Power = 45,
                Accuracy = 100,
                Category = MoveBase.MoveCategory.Physical
            };

            var starter = new EdumonBase
            {
                Id = "001",
                Name = "Bulbasaur",
                Description = "A strange seed was planted on its back at birth. The plant sprouts and grows with this Pokémon.",
                FrontSpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/1.png",
                BackSpriteUrl = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/back/1.png",
                Type1 = EdumonType.Grass,
                Type2 = EdumonType.Poison,
                MaxHp = 45,
                Attack = 49,
                Defense = 49,
                SpAttack = 65,
                SpDefense = 65,
                Speed = 45,
                ExpYield = 64,
                GrowthRate = GrowthRate.MediumSlow,
                LearnableMoves = new List<LearnableMove>
                {
                    new LearnableMove { MoveBase = tackle, Level = 1 },
                    new LearnableMove { MoveBase = growl, Level = 3 },
                    new LearnableMove { MoveBase = vineWhip, Level = 5 }
                }
            };

            return starter;
        }

        // Save game
        public async Task SaveGameAsync()
        {
            // Implement saving logic
        }

        // Load game
        public async Task LoadGameAsync(string saveId)
        {
            // Implement loading logic
        }
    }

    public class PlayerData
    {
        public string Name { get; set; }
        public int Coins { get; set; }
        // Other player stats
    }

    public class BattleState
    {
        public Edumon PlayerEdumon { get; set; }
        public Edumon OpponentEdumon { get; set; }
        // Battle state properties
    }
}