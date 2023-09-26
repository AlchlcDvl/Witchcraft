namespace Witchcraft;

/// <summary>Witchcraft's main class.</summary>
[SalemMod]
public class Witchcraft
{
    /// <summary>All mods and thier assemblies that are registered by Witchcraft.</summary>
    public static readonly Dictionary<string, Assembly> Registered = new();

    /// <summary>Gets Witchcraft's mod path.</summary>
    public static string ModPath => Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Witchcraft");

    /// <summary>The start function for Witchcraft.</summary>
    public void Start()
    {
        if (!Directory.Exists(ModPath))
            Directory.CreateDirectory(ModPath);

        _ = Register("Witchcraft", Assembly.GetExecutingAssembly());
        Patches.Logs = string.Empty;
        Patches.SaveLogs();
        Logging.Log("Magic is brewing!");
    }

    /// <summary>Registers and patches methods from <paramref name="assembly"/>.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="assembly">The <see cref="Assembly"/> that <paramref name="modName"/> is attached to.</param>
    /// <returns><see langword="true"/> if <paramref name="modName"/> was succesfully registered.</returns>
    public static bool Register(string modName, Assembly assembly)
    {
        if (Registered.ContainsKey(modName))
        {
            Logging.Log($"Types in {modName} are already registered");
            return false;
        }

        HarmonyQuickPatcher.ApplyHarmonyPatches(modName, assembly);
        return Registered.TryAdd(modName, assembly);
    }
}
