using System.Text.Json;

class GameOptions {
    private readonly string optionsFile = "GameOptions.json";
    private Dictionary<int, List<string>> _options;

    public GameOptions()
    {
        _options = new Dictionary<int, List<string>>();
        SeedDictionary();
    }

    private void SeedDictionary() 
    {
        if (File.Exists(optionsFile))
        {
            var json = File.ReadAllText(optionsFile);
            _options = JsonSerializer.Deserialize<Dictionary<int, List<string>>>(json);
            Console.WriteLine($"Options loaded from {optionsFile}");
        }
        else
        {
            Console.WriteLine("No game file found, starting with an empty list.");
            _options = [];
        }
    }

    public string GetRandomGame(int playerAmount) 
    {
        var random = new Random();
        var index = random.Next(_options[playerAmount].Count);
        return _options[playerAmount][index];
    }

    public void AddGame(int playerAmount, string game) 
    {
        if (!_options.ContainsKey(playerAmount))
        {
            _options[playerAmount] = new List<string>();
        }
        _options[playerAmount].Add(game);

        SaveDictionaryToFile();
    }

    public string RemoveGame(int playerAmount, string game) 
    {
        if (_options.ContainsKey(playerAmount))
        {
            var gameList = _options[playerAmount];
            var gameToRemove = gameList.FirstOrDefault(g => string.Equals(g, game, StringComparison.OrdinalIgnoreCase));

            if (gameToRemove != null)
            {
                gameList.Remove(gameToRemove);
                if (gameList.Count == 0)
                {
                    _options.Remove(playerAmount); // Clean up empty lists
                }

                SaveDictionaryToFile();


                return $"Removed game '{gameToRemove}' for {playerAmount} players.";
            }
            else
            {
                return $"Game '{game}' not found for {playerAmount} players.";
            }
        }
        else
        {
            return $"No games found for {playerAmount} players.";
        }
    }

    private void SaveDictionaryToFile()
    {
        var json = JsonSerializer.Serialize(_options, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(optionsFile, json);
        Console.WriteLine($"Options saved to {optionsFile}");
    }

    public string ListAllGames() {
        if (_options.Count == 0)
        {
            return "No games have been added yet.";
        }
        else
        {
            var response = new System.Text.StringBuilder("Available games:\n");

            foreach (var entry in _options)
            {
                response.AppendLine(string.Empty);
                response.AppendLine($"{entry.Key} players:");
                foreach (var game in entry.Value)
                {
                    response.AppendLine($"- {game}");
                }
            }

            return response.ToString();
        }
    }
}