using BepInEx.Configuration;

namespace Witchcraft.Managers;

public class ConfigManager : BaseManager
{
    public static List<ConfigManager> Managers { get; set; } = [];

    public List<ConfigEntryBase> Configs { get; set; } = [];

    public ConfigManager(string name, WitchcraftMod mod) : base(name, mod) => Managers.Add(this);

    public Config<T> Bind<T>(string key, string descKey, T defaultValue)
    {
        var config = ModManager.SMLInstance!.Config.Bind(Mod.ModInfo!.HarmonyId, key, defaultValue, descKey);
        Configs.Add(config);
        return new(config);
    }
}