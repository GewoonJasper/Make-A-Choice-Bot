using System.Text.Json;

class GameOptions {
    private readonly string optionsFile = "GameOptions.json";
    private Dictionary<int, List<string>> _options = new();

    public GameOptions()
    {
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

    public IEnumerable<string> GetInteractivityEmojiNames() {
        var numberNames = new List<string> { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

        foreach (var playerAmount in _options.Keys.Order()) {
            if (playerAmount <= numberNames.Count) {
                yield return numberNames[playerAmount - 1];
            }
            else {
                yield return "arrow_double_up";
            }
        }
    }

    public string ListGamesFromPlayerAmount(int playerAmount) {
        var response = new System.Text.StringBuilder();

        if (playerAmount != 10) {
            var games = _options[playerAmount];
            
            foreach (var game in games)
            {
                response.AppendLine($"- {game}");
            }
        } else {
            foreach (var kvp in _options.Where(x => x.Key >= 10))
            {
                response.AppendLine($"Games for {kvp.Key} players:");

                foreach (var game in kvp.Value)
                {
                    response.AppendLine($"- {game}");
                }

                response.AppendLine();
            }
        }

        return response.ToString();
    }

    public bool AddGame(int playerAmount, string game) 
    {
        if (!_options.ContainsKey(playerAmount))
        {
            _options[playerAmount] = new List<string>();
        }

        if (!_options[playerAmount].Any(g => g.Equals(game, StringComparison.OrdinalIgnoreCase)))
        {
            _options[playerAmount].Add(game);
            SaveDictionaryToFile();
            return true;
        }

        return false;
    }

    public bool RemoveGame(int playerAmount, string game) 
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

                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private void SaveDictionaryToFile()
    {
        var json = JsonSerializer.Serialize(_options, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(optionsFile, json);
        Console.WriteLine($"Options saved to {optionsFile}");
    }

    public string GetRandomGame(int playerAmount) 
    {
        if (!_options.TryGetValue(playerAmount, out List<string> value) || value.Count == 0)
        {
            return null; // No games available for this player count
        }

        var random = new Random();
        var index = random.Next(value.Count);
        return value[index];
    }
}