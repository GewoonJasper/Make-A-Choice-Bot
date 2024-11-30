using Newtonsoft.Json;

class ConfigUtil
{
    public const string ConfigFile = "config.json";

    public string Token { get; set; }
    public string Prefix { get; set; }

    public async Task ReadConfig()
    {
        using StreamReader sr = new(ConfigFile);

        string json = await sr.ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<ConfigStructure>(json);
        
        Token = data.Token;
        Prefix = data.Prefix;
    }
}

internal sealed class ConfigStructure {
    public string Token { get; set; }
    public string Prefix { get; set; }
}