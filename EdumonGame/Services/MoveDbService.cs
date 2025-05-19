using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EdumonGame.Models;

namespace EdumonGame.Services
{
    public interface IMoveDbService
    {
        List<MoveBase> GetAllMoves();
        MoveBase GetMoveById(string id);
        MoveBase GetMoveByName(string name);
        Task InitializeAsync();
    }

    public class MoveDbService : IMoveDbService
    {
        private Dictionary<string, MoveBase> _movesById = new();
        private Dictionary<string, MoveBase> _movesByName = new();
        private readonly IWebHostEnvironment _environment;

        public MoveDbService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task InitializeAsync()
        {
            await LoadMovesAsync();
        }

        private async Task LoadMovesAsync()
        {
            try
            {
                string filePath = Path.Combine(_environment.WebRootPath, "data", "moves.json");

                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Error: Move data file not found at {filePath}");
                    return;
                }

                string json = await File.ReadAllTextAsync(filePath);
                var moves = JsonSerializer.Deserialize<List<MoveBase>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                foreach (var move in moves)
                {
                    _movesById[move.Id] = move;
                    _movesByName[move.Name.ToLower()] = move;
                }

                Console.WriteLine($"Loaded {moves.Count} moves successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading moves: {ex.Message}");
            }
        }

        public List<MoveBase> GetAllMoves() => _movesById.Values.ToList();

        public MoveBase GetMoveById(string id)
        {
            return _movesById.TryGetValue(id, out var move) ? move : null;
        }

        public MoveBase GetMoveByName(string name)
        {
            return _movesByName.TryGetValue(name.ToLower(), out var move) ? move : null;
        }
    }

    // Helper class to provide static access for saved game loading
    public static class MoveDB
    {
        private static IMoveDbService _service;

        public static void Initialize(IMoveDbService moveDbService)
        {
            _service = moveDbService;
        }

        public static MoveBase GetObjectByName(string name)
        {
            return _service.GetMoveByName(name);
        }
    }
}