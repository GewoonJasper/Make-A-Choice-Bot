using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

class ChoiceCommands : BaseCommandModule
{
    private GameOptions _gameOptions = new GameOptions();

    [Command("choose")]
    public async Task ChooseRandomGame(CommandContext ctx, int playerAmount) {
        await ctx.Channel.SendMessageAsync($"Random game for {playerAmount} players: {_gameOptions.GetRandomGame(playerAmount)}");
    }

    [Command("add")]
    public async Task AddGame(CommandContext ctx, int playerAmount, params string[] game) {
        var sb = new StringBuilder();
        foreach (var gamePart in game) {
            sb.Append(gamePart + " ");
        }
        sb.Length--;

        _gameOptions.AddGame(playerAmount, game.ToString());
        await ctx.Channel.SendMessageAsync($"Added game '{game}' for {playerAmount} players.");
    }

    [Command("remove")]
    public async Task RemoveGame(CommandContext ctx, int playerAmount, params string[] game) {
        var sb = new StringBuilder();
        foreach (var gamePart in game) {
            sb.Append(gamePart + " ");
        }
        sb.Length--;

        await ctx.Channel.SendMessageAsync(_gameOptions.RemoveGame(playerAmount, sb.ToString()));
    }

    [Command("list")]
    public async Task ListAllGames(CommandContext ctx) {
        await ctx.Channel.SendMessageAsync(_gameOptions.ListAllGames());
    }
}