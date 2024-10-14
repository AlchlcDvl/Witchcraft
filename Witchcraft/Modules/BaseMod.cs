namespace Witchcraft.Modules;

[AttributeUsage(AttributeTargets.Class)]
public class WitchcraftModAttribute : Attribute
{
    public AssetManager Assets { get; set; }
    public LogManager Logs { get; set; }
    public string ModPath { get; }
    public Type ModType { get; set; }
    public bool HasFolder { get; set; }
    public string Name { get; set; }

    public WitchcraftModAttribute(Type modType, string name = null!, string[] bundles = null!) : base()
    {
        Name = name ?? modType.Name;
        ModType = modType;
        ModPath = Path.Combine(ModManager.ModFoldersPath, Name.Replace(" ", string.Empty));

        Logs = new(Name);

        var method1 = modType.GetMethods().FirstOrDefault(x => x.GetCustomAttribute<UponAssetsLoadedAttribute>() != null || x.Name.Contains("UponAssetsLoaded")) ??
            typeof(WitchcraftModAttribute).GetMethod(nameof(BlankVoid));
        Assets = new(Name, () => method1.Invoke(null, null), modType.Assembly, bundles ?? [], Logs);

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

    public static implicit operator bool(WitchcraftModAttribute exists) => exists != null;
}

[AttributeUsage(AttributeTargets.Method)]
public class UponAssetsLoadedAttribute : Attribute;

public static class ModSingleton<T>
{
    public static WitchcraftModAttribute ? Instance
    {
        get
        {
            if (_instance == null)
                _instance = ModManager.Instance(typeof(T));

            return _instance;
        }
    }
    private static WitchcraftModAttribute ? _instance;
}