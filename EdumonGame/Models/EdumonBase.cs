using System.Collections.Generic;

namespace EdumonGame.Models
{
    public class EdumonBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // In Blazor we'll use URLs to images instead of Sprite references
        public string FrontSpriteUrl { get; set; }
        public string BackSpriteUrl { get; set; }

        public EdumonType Type1 { get; set; }
        public EdumonType Type2 { get; set; } = EdumonType.None;

        // Base Stats
        public int MaxHp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int SpAttack { get; set; }
        public int SpDefense { get; set; }
        public int Speed { get; set; }

        public int ExpYield { get; set; }
        public GrowthRate GrowthRate { get; set; }

        public int CatchRate { get; set; } = 255;

        public List<LearnableMove> LearnableMoves { get; set; } = new List<LearnableMove>();
        public List<MoveBase> LearnableByItems { get; set; } = new List<MoveBase>();

        public List<Evolution> Evolutions { get; set; } = new List<Evolution>();

        public static int MaxNumOfMoves { get; set; } = 4;

        public int GetExpForLevel(int level)
        {
            return GrowthRate switch
            {
                GrowthRate.Fast => 4 * (level * level * level) / 5,
                GrowthRate.MediumFast => level * level * level,
                _ => -1
            };
        }
    }

    public class LearnableMove
    {
        public MoveBase MoveBase { get; set; }
        public int Level { get; set; }
    }

    public enum Stat
    {
        Attack,
        Defense,
        SpAttack,
        SpDefense,
        Speed,
        // Not Actual Stats
        Accuracy,
        Evasion
    }

    public enum GrowthRate
    {
        Fast,
        MediumFast,
        MediumSlow,
        Slow,
        Erratic
    }

    public class Evolution
    {
        public EdumonBase EvolvesInto { get; set; }
        public int RequiredLevel { get; set; }
        public EvolutionItem RequiredItem { get; set; }
    }

    public enum EdumonType
    {
        None,
        Normal,
        Fighting,
        Flying,
        Poison,
        Ground,
        Rock,
        Bug,
        Ghost,
        Steel,
        Fire,
        Water,
        Grass,
        Electric,
        Psychic,
        Ice,
        Dragon,
        Dark,
        Fairy
    }
}