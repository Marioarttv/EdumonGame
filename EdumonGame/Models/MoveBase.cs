using System.Collections.Generic;

namespace EdumonGame.Models
{
    public class MoveBase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EdumonType Type { get; set; }
        public int Power { get; set; }
        public int Accuracy { get; set; }
        public bool AlwaysHits { get; set; }
        public int Priority { get; set; }
        public MoveCategory Category { get; set; }
        public MoveEffects Effects { get; set; }
        public List<SecondaryEffects> Secondaries { get; set; }
        public MoveTarget Target { get; set; }

        // In Blazor, we'll use sound URL rather than AudioClip
        public string SoundUrl { get; set; }

        // For animation, we'll use a different approach in web
        public string AnimationId { get; set; }

        public class MoveEffects
        {
            public List<StatBoost> Boosts { get; set; }
            public ConditionID Status { get; set; }
            public ConditionID VolatileStatus { get; set; }
        }

        public class SecondaryEffects : MoveEffects
        {
            public int Chance { get; set; }
            public MoveTarget Target { get; set; }
        }

        public class StatBoost
        {
            public Stat stat;
            public int boost;
        }

        public enum MoveCategory
        {
            Physical, Special, Status
        }

        public enum MoveTarget
        {
            Foe, Self
        }
    }
}