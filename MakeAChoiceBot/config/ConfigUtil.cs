using Newtonsoft.Json;

class ConfigUtil
{
    public static readonly string ConfigFile = Path.Combine(AppContext.BaseDirectory, "config.json");

    public string Token { get; set; }
    public string Prefix { get; set; }

    public async Task ReadConfig()
    {
        if (!File.Exists(ConfigFile))
        {
            Console.WriteLine($"[ERROR] Config file not found at: {ConfigFile}");
            Console.WriteLine("Make sure the file exists or check your project settings.");
            return;
        }

        using StreamReader sr = new(ConfigFile);
        string json = await sr.ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<ConfigStructure>(json);

        Token = data.Token;
        Prefix = data.Prefix;
    }
}

internal sealed class ConfigStructure
{
    public string Token { get; set; }
    public string Prefix { get; set; }
}
