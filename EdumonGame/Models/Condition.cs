using System;

namespace EdumonGame.Models
{
    public class Condition
    {
        public ConditionID Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string StartMessage { get; set; }

        // Action/Func delegates work the same in Blazor
        public Action<Edumon> OnStart { get; set; }
        public Func<Edumon, bool> OnBeforeMove { get; set; }
        public Action<Edumon> OnAfterTurn { get; set; }
    }

    public enum ConditionID
    {
        None,
        Poison,
        Burn,
        Sleep,
        Paralysis,
        Freeze,
        Confusion
        // Add other conditions as needed
    }
}