using BepInEx.Configuration;

namespace AddHandSlot;

public static class ConfigManager
{
    internal static ConfigFile Config;

    public static ConfigEntry<T> Get<T>(string section, string key)
    {
        return Config.TryGetEntry<T>(section, key, out var config) ? config : null;
    }

    public static bool IsEnable(string section, string key)
    {
        var config = Get<bool>(section, key);
        return config is not null && config.Value;
    }
}