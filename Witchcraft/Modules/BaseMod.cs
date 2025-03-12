namespace Witchcraft.Modules;

[AttributeUsage(AttributeTargets.Class)]
public class WitchcraftMod : Attribute
{
    public AssetManager Assets { get; }
    public LogManager Logs { get; }
    public ConfigManager Configs { get; }
    public string ModPath { get; }
    public Type ModType { get; }
    public string Name { get; }
    public ModInfo ModInfo { get; }

    public WitchcraftMod(Type modType, string? name = null, string[] bundles = null!, bool hasFolder = false) : base()
    {
        ModType = modType;
        Name = name ?? modType.Name;
        ModPath = Path.Combine(ModManager.ModFoldersPath, Name.Replace(" ", string.Empty));

        Logs = new(Name, this);

        var blank = typeof(WitchcraftMod).GetMethod("BlankVoid");

        var method1 = modType.GetMethod(x => x.GetCustomAttribute<UponAssetsLoadedAttribute>() != null || x.Name.Contains("UponAssetsLoaded")) ?? blank;
        var method2 = modType.GetMethod(x => x.GetCustomAttribute<UponAllAssetsLoadedAttribute>() != null || x.Name.Contains("UponAllAssetsLoaded")) ?? blank;
        Assets = new(Name, this, () => method1.Invoke(null, null), () => method2.Invoke(null, null), modType.Assembly, bundles ?? []);

        var method3 = modType.GetMethod(x => x.GetCustomAttribute<LoadConfigsAttribute>() != null || x.Name.Contains("LoadConfigs")) ?? blank;
        Configs = new(Name, this, () => method3.Invoke(null, null));

        ModInfo = AssetManager.DeserializeJson<ModInfo>(Assets.GetString("modinfo")!);

        if (!Directory.Exists(ModPath) && hasFolder)
            Directory.CreateDirectory(ModPath);

        ModManager.RegisteredMods[Name] = this;
        Message("Attribute mod loaded!");
    }

    public void Error(object? message) => Logs!.Error(message);

    public void Fatal(object? message) => Logs!.Fatal(message);

    public void Warning(object? message, bool logIt = true) => Logs!.Warning(message, logIt);

    public void Info(object? message, bool logIt = true) => Logs!.Info(message, logIt);

    public void Message(object? message, bool logIt = true) => Logs!.Message(message, logIt);

    public void Debug(object? message, bool logIt = true) => Logs!.Debug(message, logIt);

    public void OpenDirectory() => GeneralUtils.OpenDirectory(ModPath);

    public static void BlankVoid() {}

    public static implicit operator bool(WitchcraftMod exists) => exists != null;
}

public static class ModSingleton<T>
{
    public static WitchcraftMod? Instance
    {
        get
        {
            if (!_instance!)
                _instance = ModManager.Instance(typeof(T));

            return _instance;
        }
    }
    private static WitchcraftMod? _instance;
}