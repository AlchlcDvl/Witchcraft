using SalemModLoader;

namespace Witchcraft.Managers;

public class ConfigManager : BaseManager
{
    private static Launch SMLInstance { get; set; }

    public static List<ConfigManager> Managers { get; } = [];

    public List<ConfigBase> Configs { get; } = [];
    private Action Load { get; }

    public ConfigManager(string name, BaseMod mod, Action load) : base(name, mod)
    {
        Load = load;
        Managers.Add(this);
    }

    public Config<T> Bind<T>(string key, string descKey, T defaultValue)
    {
        var config = new Config<T>(SMLInstance.Config.Bind(Mod.ModInfo!.HarmonyId, key, defaultValue, descKey));
        Configs.Add(config);
        return config;
    }

    public Config<T> Bind<T>(string key, T defaultValue) => Bind(key, key, defaultValue);

    public static ConfigManager? Config<T>() where T : BaseMod => ModManager.Instance<T>()?.Configs;

    public static void LoadAllConfigs(Launch __instance)
    {
        SMLInstance = __instance;
        Managers.ForEach(m => m.Load());
    }
}