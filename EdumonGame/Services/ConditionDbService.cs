using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EdumonGame.Models;
using Microsoft.AspNetCore.Hosting;

namespace EdumonGame.Services
{
    public interface IConditionDbService
    {
        Dictionary<ConditionID, Condition> GetAllConditions();
        Condition GetCondition(ConditionID id);
        Task InitializeAsync();
    }

    public class ConditionDbService : IConditionDbService
    {
        public Dictionary<ConditionID, Condition> Conditions { get; private set; } = new();
        private readonly IWebHostEnvironment _environment;
        private readonly Random _random = new();

        public ConditionDbService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task InitializeAsync()
        {
            // First ensure we have base condition objects
            InitializeBaseConditions();

            // Try to load conditions from JSON if available
            await LoadConditionDataAsync();

            // Then set up the behavior logic
            SetupConditionBehaviors();
        }

        // Create basic condition objects to ensure they exist
        private void InitializeBaseConditions()
        {
            // If not already in dictionary, create base condition objects
            if (!Conditions.ContainsKey(ConditionID.Poison))
                Conditions[ConditionID.Poison] = new Condition
                {
                    Id = ConditionID.Poison,
                    Name = "Poison",
                    Description = "Causes damage to the Edumon at the end of each turn.",
                    StartMessage = "was poisoned!"
                };

            if (!Conditions.ContainsKey(ConditionID.Burn))
                Conditions[ConditionID.Burn] = new Condition
                {
                    Id = ConditionID.Burn,
                    Name = "Burn",
                    Description = "Causes damage at the end of each turn and reduces Attack.",
                    StartMessage = "was burned!"
                };

            if (!Conditions.ContainsKey(ConditionID.Sleep))
                Conditions[ConditionID.Sleep] = new Condition
                {
                    Id = ConditionID.Sleep,
                    Name = "Sleep",
                    Description = "Edumon cannot move for several turns.",
                    StartMessage = "fell asleep!"
                };

            if (!Conditions.ContainsKey(ConditionID.Paralysis))
                Conditions[ConditionID.Paralysis] = new Condition
                {
                    Id = ConditionID.Paralysis,
                    Name = "Paralysis",
                    Description = "Edumon may be unable to move and its Speed is reduced.",
                    StartMessage = "was paralyzed!"
                };

            if (!Conditions.ContainsKey(ConditionID.Freeze))
                Conditions[ConditionID.Freeze] = new Condition
                {
                    Id = ConditionID.Freeze,
                    Name = "Freeze",
                    Description = "Edumon cannot move until it thaws.",
                    StartMessage = "was frozen solid!"
                };

            if (!Conditions.ContainsKey(ConditionID.Confusion))
                Conditions[ConditionID.Confusion] = new Condition
                {
                    Id = ConditionID.Confusion,
                    Name = "Confusion",
                    Description = "Edumon may hurt itself in confusion.",
                    StartMessage = "became confused!"
                };
        }

        private async Task LoadConditionDataAsync()
        {
            try
            {
                string filePath = Path.Combine(_environment.WebRootPath, "data", "conditions.json");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Warning: Condition data file not found at {filePath}, using default conditions.");
                    return;
                }

                string json = await File.ReadAllTextAsync(filePath);
                var loadedConditions = JsonSerializer.Deserialize<List<Condition>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Update or add the loaded conditions to our dictionary
                foreach (var condition in loadedConditions)
                {
                    Conditions[condition.Id] = condition;
                }

                Console.WriteLine($"Loaded {loadedConditions.Count} conditions successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading conditions: {ex.Message}");
                // Continue with default conditions
            }
        }

        // Setup the behavior logic for each condition
        private void SetupConditionBehaviors()
        {
            // Poison
            var poison = Conditions[ConditionID.Poison];
            poison.OnStart = SetPoisonedMessage;
            poison.OnAfterTurn = PoisonEffect;

            // Burn
            var burn = Conditions[ConditionID.Burn];
            burn.OnStart = SetBurnedMessage;
            burn.OnAfterTurn = BurnEffect;

            // Paralysis
            var paralysis = Conditions[ConditionID.Paralysis];
            paralysis.OnStart = SetParalyzedMessage;
            paralysis.OnBeforeMove = ParalysisEffect;

            // Sleep
            var sleep = Conditions[ConditionID.Sleep];
            sleep.OnStart = SetSleepMessage;
            sleep.OnBeforeMove = SleepEffect;

            // Freeze
            var freeze = Conditions[ConditionID.Freeze];
            freeze.OnStart = SetFreezeMessage;
            freeze.OnBeforeMove = FreezeEffect;

            // Confusion
            var confusion = Conditions[ConditionID.Confusion];
            confusion.OnStart = SetConfusionMessage;
            confusion.OnBeforeMove = ConfusionEffect;
        }

        // Condition effect implementations
        private void SetPoisonedMessage(Edumon edumon)
        {
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} was poisoned!");
        }

        private void PoisonEffect(Edumon edumon)
        {
            edumon.DecreaseHP(edumon.MaxHp / 8);
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} was hurt by poison!");
        }

        private void SetBurnedMessage(Edumon edumon)
        {
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} was burned!");
        }

        private void BurnEffect(Edumon edumon)
        {
            edumon.DecreaseHP(edumon.MaxHp / 16);
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} was hurt by its burn!");
        }

        private void SetParalyzedMessage(Edumon edumon)
        {
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} is paralyzed, it may be unable to move!");
        }

        private bool ParalysisEffect(Edumon edumon)
        {
            if (_random.Next(1, 5) == 1)
            {
                edumon.StatusChanges.Enqueue($"{edumon.Base.Name} is paralyzed and can't move!");
                return false;
            }
            return true;
        }

        private void SetSleepMessage(Edumon edumon)
        {
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} fell asleep!");
            // Sleep for 1-3 turns
            edumon.StatusTime = _random.Next(1, 4);
        }

        private bool SleepEffect(Edumon edumon)
        {
            if (edumon.StatusTime <= 0)
            {
                edumon.CureStatus();
                edumon.StatusChanges.Enqueue($"{edumon.Base.Name} woke up!");
                return true;
            }

            edumon.StatusTime--;
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} is sleeping!");
            return false;
        }

        private void SetFreezeMessage(Edumon edumon)
        {
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} was frozen solid!");
        }

        private bool FreezeEffect(Edumon edumon)
        {
            // 20% chance to thaw each turn
            if (_random.Next(1, 6) == 1)
            {
                edumon.CureStatus();
                edumon.StatusChanges.Enqueue($"{edumon.Base.Name} thawed out!");
                return true;
            }

            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} is frozen solid!");
            return false;
        }

        private void SetConfusionMessage(Edumon edumon)
        {
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} became confused!");
            // Confused for 1-4 turns
            edumon.VolatileStatusTime = _random.Next(1, 5);
        }

        private bool ConfusionEffect(Edumon edumon)
        {
            if (edumon.VolatileStatusTime <= 0)
            {
                edumon.CureVolatileStatus();
                edumon.StatusChanges.Enqueue($"{edumon.Base.Name} snapped out of confusion!");
                return true;
            }

            edumon.VolatileStatusTime--;
            edumon.StatusChanges.Enqueue($"{edumon.Base.Name} is confused!");

            // 50% chance to hurt itself
            if (_random.Next(1, 3) == 1)
            {
                edumon.StatusChanges.Enqueue($"It hurt itself in confusion!");
                edumon.DecreaseHP(edumon.MaxHp / 8);
                return false;
            }

            return true;
        }

        public Dictionary<ConditionID, Condition> GetAllConditions() => Conditions;

        public Condition GetCondition(ConditionID id)
        {
            return Conditions.TryGetValue(id, out var condition) ? condition : null;
        }
    }

    // Static helper class to provide global access for saved game loading
    public static class ConditionsDB
    {
        public static Dictionary<ConditionID, Condition> Conditions { get; private set; }

        public static void Initialize(IConditionDbService conditionDbService)
        {
            Conditions = conditionDbService.GetAllConditions();
        }
    }
}