using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using MakeAChoiceBot;

class Commands : ApplicationCommandModule
{
    private GameOptions _gameOptions = new GameOptions();

    [SlashCommand("list", "Lists all the available games in the system")] 
    public async Task ListGames(InteractionContext ctx) {
        await ctx.DeferAsync();

        var interactivity = Program.Client.GetInteractivity();

        var emojiNumbers = _gameOptions.GetInteractivityEmojiNames().Distinct();
        var playerAmountOptions = emojiNumbers
            .Select(emoji => DiscordEmoji.FromName(Program.Client, $":{emoji}:"))
            .ToArray();

        var embed = new DiscordEmbedBuilder() {
            Color = DiscordColor.Blue,
            Title = "List of all available games in the system",
            Description = "Please select the number of players by reacting with an emoji."
        };

        var sentMessage = await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

        foreach (var playerAmount in playerAmountOptions) {
            await sentMessage.CreateReactionAsync(playerAmount);
        }

        interactivity.Client.MessageReactionAdded += async (sender, eventArgs) =>
        {
            // Check if the reaction is from the correct message
            if (eventArgs.Message.Id != sentMessage.Id || eventArgs.User.IsBot) return;

            var number = GetNumberFromEmoji(eventArgs.Emoji.Name);
            DiscordEmbedBuilder updatedEmbed;

            if (number == -1) {
                updatedEmbed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Title = $"Something went wrong while getting the list of players"
                };
            } else {
                updatedEmbed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blue,
                    Title = $"List of games for {(number == 10 ? "10+" : number)} players",
                    Description = _gameOptions.ListGamesFromPlayerAmount(number)
                };
            }

            await sentMessage.ModifyAsync(msg => msg.Embed = updatedEmbed.Build());

            await eventArgs.Message.DeleteReactionAsync(eventArgs.Emoji, eventArgs.User);
        };
    }

    private static int GetNumberFromEmoji(string emoji) {
        var emojiToNumber = new Dictionary<string, int>
        {
            {"1️⃣", 1},
            {"2️⃣", 2},
            {"3️⃣", 3},
            {"4️⃣", 4},
            {"5️⃣", 5},
            {"6️⃣", 6},
            {"7️⃣", 7},
            {"8️⃣", 8},
            {"9️⃣", 9},
            {"⏫", 10} //Games for 10 or more players
        };

        return emojiToNumber.TryGetValue(emoji, out var number) ? number : -1;
    }

    [SlashCommand("add", "Adds a new game to the list")]
    public async Task AddGame(InteractionContext ctx, [Option("players", "Number of players")] string players, [Option("game", "Game name")] string gameName) {
        await ctx.DeferAsync();

        var playerNumbers = players.Split(' ')
                                .Select(p => int.TryParse(p, out int num) ? num : (int?)null)
                                .Where(num => num.HasValue)
                                .Select(num => num.Value)
                                .ToList();
        
        if (playerNumbers.Count == 0)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Invalid player numbers"));
            return;
        }

        var addedPlayerNumbers = new List<int>();
        foreach (var playerAmount in playerNumbers)
        {
            if (!_gameOptions.AddGame(playerAmount, gameName)) {
                await ctx.Channel.SendMessageAsync($"The games in the list for {playerAmount} players already contains the game {gameName}");
            } else {
                addedPlayerNumbers.Add(playerAmount);
            }
        }

        if (addedPlayerNumbers.Count == 0) {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Did not add any games to the list"));
        } else {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Added '{gameName}' for {string.Join(", ", addedPlayerNumbers)} players!"));
        }
    }

    [SlashCommand("remove", "Removes an existing game from the list.")]
    public async Task RemoveGame(InteractionContext ctx, [Option("players", "Number of players")] string players, [Option("game", "Game name")] string gameName)
    {
        await ctx.DeferAsync();

        // Parse player numbers
        var playerNumbers = players.Split(' ')
                                .Select(p => int.TryParse(p, out int num) ? num : (int?)null)
                                .Where(num => num.HasValue)
                                .Select(num => num.Value)
                                .ToList();

        if (!playerNumbers.Any())
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Invalid player numbers"));
            return;
        }

        var removedPlayerNumbers = new List<int>();
        foreach (var playerAmount in playerNumbers)
        {
            if (!_gameOptions.RemoveGame(playerAmount, gameName)) {
                await ctx.Channel.SendMessageAsync($"The games in the list for {playerAmount} players does not contain the game {gameName}");
            } else {
                removedPlayerNumbers.Add(playerAmount);
            }
        }

        if (removedPlayerNumbers.Count == 0) {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Did not remove any games from the list"));
        } else {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Removed '{gameName}' for {string.Join(", ", removedPlayerNumbers)} players!"));
        }
    }

    [SlashCommand("choose", "Chooses a random game from the list, for the selected number of players")]
    public async Task ChooseGame(InteractionContext ctx, [Option("players", "Number of players")] string playerAmountString)
    {
        await ctx.DeferAsync();

        if (!int.TryParse(playerAmountString, out int playerAmount)) {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Invalid number of players"));
            return;
        }

        string randomGame = _gameOptions.GetRandomGame(playerAmount);

        if (randomGame == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"No games found for {playerAmount} players."));
            return;
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"🎲 Random game for {playerAmount} players: {randomGame}"));
    }
}