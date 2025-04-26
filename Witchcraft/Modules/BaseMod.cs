namespace Witchcraft.Modules;

public abstract class BaseMod
{
    public abstract string Name { get; }

    public virtual string[] Bundles { get; } = [];
    public virtual bool HasFolder { get; } = false;

    public string ModPath { get; }
    public Type ModType { get; }
    public ModInfo ModInfo { get; }
    public AssetManager Assets { get; }
    public LogManager Logs { get; }
    public ConfigManager Configs { get; }

    public BaseMod()
    {
        ModType = GetType();
        ModPath = Path.Combine(ModManager.ModFoldersPath!, Name.Replace(" ", string.Empty));

        Logs = new(Name, this, CustomLogColor); // Generating the logger first

        Assets = new(Name, this, UponAssetsLoaded, UponAllAssetsLoaded, ModType.Assembly, Bundles ?? []); // Creating the asset manager for the mod

        ModInfo = AssetManager.DeserializeJson<ModInfo>(Assets.GetString("modinfo")!); // Generating mod info for configs
        Configs = new(Name, this, LoadConfigs); // Creating the config manager

        // Attempting to create a directory
        try
        {
            if (!Directory.Exists(ModPath) && HasFolder)
                Directory.CreateDirectory(ModPath);
        }
        catch (Exception ex)
        {
            Fatal($"Failed to create a directory because:\n{ex}");
        }

        // Attempting to register the mod
        if (!ModManager.RegisteredMods.TryAdd(Name, this))
            Fatal("Error in registering the mod because there was already another mod with the same name!");
    }

    public abstract void Start();

    public virtual void UponAssetsLoaded() { }

    public virtual void UponAllAssetsLoaded() { }

    public virtual void LoadConfigs() { }

    public virtual ConsoleColor CustomLogColor(Enum value) => ConsoleColor.DarkGray;

    public void Error(object? message) => Logs.Error(message);

    public void Fatal(object? message) => Logs.Fatal(message);

    public void Warning(object? message, bool logIt = false) => Logs.Warning(message, logIt);

    public void Info(object? message, bool logIt = false) => Logs.Info(message, logIt);

    public void Message(object? message, bool logIt = false) => Logs.Message(message, logIt);

    public void Debug(object? message, bool logIt = false) => Logs.Debug(message, logIt);

    public void Custom(object? message, Enum level, bool logIt = false) => Logs.Custom(message, level, logIt);

    public void OpenDirectory() => GeneralUtils.OpenDirectory(ModPath);

    public static implicit operator bool(BaseMod exists) => exists != null;
}

public abstract class BaseMod<T> : BaseMod where T : BaseMod
{
    public static T? Instance => instance ??= ModManager.Instance<T>();
    private static T? instance;
}

public static class ModSingleton<T> where T : BaseMod
{
    public static T? Instance => instance ??= ModManager.Instance<T>();
    private static T? instance;
}