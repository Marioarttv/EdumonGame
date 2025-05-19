using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EdumonGame.Models;

namespace EdumonGame.Services
{
    public interface IEdumonDbService
    {
        List<EdumonBase> GetAllEdumons();
        EdumonBase GetEdumonById(string id);
        EdumonBase GetEdumonByName(string name);
        Task InitializeAsync();
    }

    public class EdumonDbService : IEdumonDbService
    {
        private Dictionary<string, EdumonBase> _edumonsById = new();
        private Dictionary<string, EdumonBase> _edumonsByName = new();
        private readonly IWebHostEnvironment _environment;

        public EdumonDbService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task InitializeAsync()
        {
            await LoadEdumonsAsync();
        }

        private async Task<List<EdumonBase>> LoadEdumonsAsync()
        {
            try
            {
                string filePath = Path.Combine(_environment.WebRootPath, "data", "edumons.json");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Error: Edumon data file not found at {filePath}");
                    return new List<EdumonBase>();
                }

                string json = await File.ReadAllTextAsync(filePath);
                var edumons = JsonSerializer.Deserialize<List<EdumonBase>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                foreach (var edumon in edumons)
                {
                    _edumonsById[edumon.Id] = edumon;
                    _edumonsByName[edumon.Name.ToLower()] = edumon;
                }

                Console.WriteLine($"Loaded {edumons.Count} edumons successfully");
                return edumons;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading edumons: {ex.Message}");
                return new List<EdumonBase>();
            }
        }

        public List<EdumonBase> GetAllEdumons() => _edumonsById.Values.ToList();

        public EdumonBase GetEdumonById(string id)
        {
            return _edumonsById.TryGetValue(id, out var edumon) ? edumon : null;
        }

        public EdumonBase GetEdumonByName(string name)
        {
            return _edumonsByName.TryGetValue(name.ToLower(), out var edumon) ? edumon : null;
        }
    }

    // Static helper class for EdumonDB
    public static class EdumonDB
    {
        private static IEdumonDbService _service;

        public static void Initialize(IEdumonDbService edumonDbService)
        {
            _service = edumonDbService;
        }

        public static EdumonBase GetObjectByName(string name)
        {
            return _service.GetEdumonByName(name);
        }

        public static EdumonBase GetObjectById(string id)
        {
            return _service.GetEdumonById(id);
        }

        public static List<EdumonBase> GetAllObjects()
        {
            return _service.GetAllEdumons();
        }
    }
}