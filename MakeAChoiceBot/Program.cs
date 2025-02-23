using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace MakeAChoiceBot;

public sealed class Program {
    public static DiscordClient Client { get; private set; }
    private static SlashCommandsExtension _commands;

    static async Task Main(string[] args) {
        var config = new ConfigUtil();
        await config.ReadConfig();

        if (config.Token == default) {
            return;
        }

        Client = new DiscordClient(new DiscordConfiguration
        {
            Token = config.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
            AutoReconnect = true
        });

        Client.UseInteractivity(new DSharpPlus.Interactivity.InteractivityConfiguration
        {
            Timeout = TimeSpan.FromMinutes(1)
        });

        Client.Ready += OnReady;

        _commands = Client.UseSlashCommands();
        _commands.RegisterCommands<Commands>();

        await Client.ConnectAsync();
        await Task.Delay(-1); // Keep the bot running
    }
    
    static Task OnReady(DiscordClient sender, ReadyEventArgs e)
    {
        Console.WriteLine($"Bot is connected as {sender.CurrentUser.Username}!");
        return Task.CompletedTask;
    }
}