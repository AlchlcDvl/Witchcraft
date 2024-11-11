namespace Witchcraft.Modules;

[AttributeUsage(AttributeTargets.Class)]
public class WitchcraftMod : Attribute
{
    public AssetManager Assets { get; set; }
    public LogManager Logs { get; set; }
    public string ModPath { get; }
    public Type ModType { get; set; }
    public bool HasFolder { get; set; }
    public string Name { get; set; }

    public WitchcraftMod(Type modType, string name = null!, string[] bundles = null!, bool hasFolder = false) : base()
    {
        ModType = modType;
        HasFolder = hasFolder;
        Name = name ?? modType.Name;

        ModPath = Path.Combine(ModManager.ModFoldersPath, Name.Replace(" ", string.Empty));

        Logs = new(Name);

        var method1 = modType.GetMethod(x => x.GetCustomAttribute<UponAssetsLoadedAttribute>() != null || x.Name.Contains("UponAssetsLoaded")) ?? typeof(WitchcraftMod).GetMethod("BlankVoid");
        var method2 = modType.GetMethod(x => x.GetCustomAttribute<UponAllAssetsLoadedAttribute>() != null || x.Name.Contains("UponAllAssetsLoaded")) ??
            typeof(WitchcraftMod).GetMethod("BlankVoid");
        Assets = new(Name, () => method1.Invoke(null, null), () => method2.Invoke(null, null), modType.Assembly, bundles ?? [], Logs);

        if (!Directory.Exists(ModPath) && HasFolder)
            Directory.CreateDirectory(ModPath);

        ModManager.RegisteredMods[Name] = this;
        Message("Attribute mod loaded!");
    }

    public static void BlankVoid() {}

    public void Error(object? message) => Logs!.Error(message);

    public void Fatal(object? message) => Logs!.Fatal(message);

    public void Warning(object? message, bool logIt = true) => Logs!.Warning(message, logIt);

    public void Info(object? message, bool logIt = true) => Logs!.Info(message, logIt);

    public void Message(object? message, bool logIt = true) => Logs!.Message(message, logIt);

    public void Debug(object? message, bool logIt = true) => Logs!.Debug(message, logIt);

    public void OpenDirectory() => GeneralUtils.OpenDirectory(ModPath);

    public static implicit operator bool(WitchcraftMod exists) => exists != null;
}

public static class ModSingleton<T>
{
    public static WitchcraftMod? Instance
    {
        get
        {
            if (_instance == null)
                _instance = ModManager.Instance(typeof(T));

            return _instance;
        }
    }
    private static WitchcraftMod ? _instance;
}