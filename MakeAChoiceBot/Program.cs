using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.Text.Json;

DiscordClient Client;
CommandsNextExtension Commands;

await SetupDiscord();

async Task SetupDiscord()
{
    var config = new ConfigUtil();
    await config.ReadConfig();

    Client =  new DiscordClient(new DiscordConfiguration
    {
        Token = config.Token,
        TokenType = TokenType.Bot,
        Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
        AutoReconnect = true
    });

    Client.Ready += OnReady;

    Commands = Client.UseCommandsNext(new CommandsNextConfiguration() {
        StringPrefixes = [config.Prefix],
        EnableMentionPrefix = true,
        EnableDefaultHelp = true
    });

    Commands.RegisterCommands<ChoiceCommands>();

    // Client.MessageCreated += OnMessageCreated;

    await Client.ConnectAsync();
    await Task.Delay(-1); // Keep the bot running
}

static Task OnReady(DiscordClient sender, ReadyEventArgs e)
{
    Console.WriteLine($"Bot is connected as {sender.CurrentUser.Username}!");
    return Task.CompletedTask;
}

// Task OnMessageCreated(DiscordClient sender, MessageCreateEventArgs e)
// {
//     if (e.Message.Content.StartsWith("/mac get"))
//     {
//         if (Options.Count == 0) { 
//             e.Message.RespondAsync($"No games available. Please use '/mac add [number of players] [game name]' to add a game");
//             return Task.CompletedTask;
//         }

//         var parts = e.Message.Content.Split(' ');
//         if (parts.Length == 3 && int.TryParse(parts[2], out int playerCount))
//         {
//             if (Options.ContainsKey(playerCount) && Options[playerCount].Count > 0)
//             {
//                 var randomGame = GetRandomOption(Options[playerCount]);
//                 e.Message.RespondAsync($"Random game for {playerCount} players: {randomGame}");
//             }
//             else
//             {
//                 var currPlayerOptions = Options.Keys.Select(k => k.ToString()).Aggregate((a, b) => $"{a}, {b}");
//                 e.Message.RespondAsync($"No games available for {playerCount} players. The current player amounts are: {currPlayerOptions}");
//             }
//         }
//         else
//         {
//             e.Message.RespondAsync("Usage: /mac get [number of players]");
//         }
//     }
//     else if (e.Message.Content.StartsWith("/mac add"))
//     {
//         var command = e.Message.Content;
//         var prefix = "/mac add ";

//         if (command.Length > prefix.Length)
//         {
//             var args = command.Substring(prefix.Length).Split(' ', 2); // Split after prefix, limit to 2 parts

//             if (args.Length == 2 && int.TryParse(args[0], out int playerCount))
//             {
//                 var gameName = args[1]; // The rest is the game name
//                 if (!Options.ContainsKey(playerCount))
//                 {
//                     Options[playerCount] = new List<string>();
//                 }
//                 Options[playerCount].Add(gameName);
//                 SaveOptionsToFile();
//                 e.Message.RespondAsync($"Added game '{gameName}' for {playerCount} players.");
//             }
//             else
//             {
//                 e.Message.RespondAsync("Usage: /mac add [number of players] [game name]");
//             }
//         }
//         else
//         {
//             e.Message.RespondAsync("Usage: /mac add [number of players] [game name]");
//         }
//     }
//     else if (e.Message.Content.StartsWith("/mac list"))
//     {
//         if (Options.Count == 0)
//         {
//             e.Message.RespondAsync("No games have been added yet.");
//         }
//         else
//         {
//             var response = new System.Text.StringBuilder("Available games:\n");

//             foreach (var entry in Options)
//             {
//                 response.AppendLine(string.Empty);
//                 response.AppendLine($"{entry.Key} players:");
//                 foreach (var game in entry.Value)
//                 {
//                     response.AppendLine($"- {game}");
//                 }
//             }

//             e.Message.RespondAsync(response.ToString());
//         }
//     }
//     else if (e.Message.Content.StartsWith("/mac remove"))
//     {
//         var command = e.Message.Content;
//         var prefix = "/mac remove ";

//         if (command.Length > prefix.Length)
//         {
//             var args = command.Substring(prefix.Length).Split(' ', 2); // Split after prefix, limit to 2 parts

//             if (args.Length == 2 && int.TryParse(args[0], out int playerCount))
//             {
//                 var gameName = args[1];
//                 if (Options.ContainsKey(playerCount))
//                 {
//                     var gameList = Options[playerCount];
//                     var gameToRemove = gameList.FirstOrDefault(g => string.Equals(g, gameName, StringComparison.OrdinalIgnoreCase));

//                     if (gameToRemove != null)
//                     {
//                         gameList.Remove(gameToRemove);
//                         if (gameList.Count == 0)
//                         {
//                             Options.Remove(playerCount); // Clean up empty lists
//                         }
//                         SaveOptionsToFile();
//                         e.Message.RespondAsync($"Removed game '{gameToRemove}' for {playerCount} players.");
//                     }
//                     else
//                     {
//                         e.Message.RespondAsync($"Game '{gameName}' not found for {playerCount} players.");
//                     }
//                 }
//                 else
//                 {
//                     e.Message.RespondAsync($"No games found for {playerCount} players.");
//                 }
//             }
//             else
//             {
//                 e.Message.RespondAsync("Usage: /mac remove [number of players] [game name]");
//             }
//         }
//         else
//         {
//             e.Message.RespondAsync("Usage: /mac remove [number of players] [game name]");
//         }
//     }

//     return Task.CompletedTask;
// }

string GetRandomOption(List<string> options)
{
    var random = new Random();
    var index = random.Next(options.Count);
    return options[index];
}