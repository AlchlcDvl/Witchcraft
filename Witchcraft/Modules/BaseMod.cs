using System.Diagnostics.CodeAnalysis;

namespace Witchcraft.Modules;

[AttributeUsage(AttributeTargets.Class)]
public class WitchcraftMod : Attribute
{
    public AssetManager? Assets { get; }
    public LogManager? Logs { get; }
    public ConfigManager? Configs { get; }
    public string ModPath { get; }
    public Type ModType { get; }
    public string Name { get; }
    public ModInfo? ModInfo { get; }

    public WitchcraftMod(Type modType, string? name = null, string[]? bundles = null, bool hasFolder = false)
    {
        ModType = modType;
        Name = name ?? modType.Name;
        ModPath = Path.Combine(ModManager.ModFoldersPath!, Name.Replace(" ", string.Empty));

        Logs = new(Name, this); // Generating the logger first

        var blank = typeof(WitchcraftMod).GetMethod("BlankVoid", AccessTools.all);

        // Conditionally creating the asset manager
        var method1 = modType.GetMethod(x => x.GetCustomAttribute<UponAssetsLoadedAttribute>() != null || x.Name.Contains("UponAssetsLoaded")) ?? blank;
        var method2 = modType.GetMethod(x => x.GetCustomAttribute<UponAllAssetsLoadedAttribute>() != null || x.Name.Contains("UponAllAssetsLoaded")) ?? blank;

        if (!method1!.IsStatic || !method2!.IsStatic)
            Fatal("Cannot properly load assets if the marked methods are not static");
        else
        {
            Assets = new(Name, this, () => method1.Invoke(null, null), () => method2.Invoke(null, null), modType.Assembly, bundles ?? []);
            ModInfo = AssetManager.DeserializeJson<ModInfo>(Assets.GetString("modinfo")!);
        }

        // Conditionally creating the config manager
        var method3 = modType.GetMethod(x => x.GetCustomAttribute<LoadConfigsAttribute>() != null || x.Name.Contains("LoadConfigs")) ?? blank;

        if (!method3!.IsStatic)
            Fatal("Cannot load configs because the method is not static");
        else
            Configs = new(Name, this, () => method3.Invoke(null, null));

        // Attempting to create a directory
        try
        {
            if (!Directory.Exists(ModPath) && hasFolder)
                Directory.CreateDirectory(ModPath);
        }
        catch (Exception ex)
        {
            Fatal($"Failed to create a directory because:\n{ex}");
        }

        // Attempting to register the mod
        if (!ModManager.RegisteredMods.TryAdd(Name, this))
            Fatal("Error in registering the mod because there was already another mod with the same name!");

        Message("Attribute mod loaded!");
    }

    public void Error(object? message) => Logs!.Error(message);

    public void Fatal(object? message) => Logs!.Fatal(message);

    public void Warning(object? message, bool logIt = false) => Logs!.Warning(message, logIt);

    public void Info(object? message, bool logIt = false) => Logs!.Info(message, logIt);

    public void Message(object? message, bool logIt = false) => Logs!.Message(message, logIt);

    public void Debug(object? message, bool logIt = false) => Logs!.Debug(message, logIt);

    public void OpenDirectory() => GeneralUtils.OpenDirectory(ModPath);

    private static void BlankVoid() {}

    public static implicit operator bool(WitchcraftMod exists) => exists != null;
}

public static class ModSingleton<T>
{
    public static WitchcraftMod? Instance => InstancePriv ??= ModManager.Instance(typeof(T));
    private static WitchcraftMod? InstancePriv;
}