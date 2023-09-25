namespace Witchcraft;

[SalemMod]
/// <summary>Witch's main class.</summary>
public class Witchcraft
{
    public static readonly Dictionary<string, Assembly> Registered = new();

    public static string ModPath => Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Witchcraft");

    /// <summary>The start function for Witchcraft.</summary>
    public void Start()
    {
        if (!Directory.Exists(ModPath))
            Directory.CreateDirectory(ModPath);

        _ = Register("Witchcraft", Assembly.GetExecutingAssembly());
        Console.Write("Magic is brewing!");
    }

    /// <summary>Registers and patches methods from <paramref name="assembly"/>.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="assembly">The <see cref="Assembly"/> that <paramref name="modName"/> is attached to.</param>
    /// <returns><see langword="true"/> if <paramref name="modName"/> was succesfully registered.</returns>
    public static bool Register(string modName, Assembly assembly)
    {
        if (Registered.ContainsKey(modName))
        {
            Console.WriteLine($"Types in {modName} are already registered");
            return false;
        }

        HarmonyQuickPatcher.ApplyHarmonyPatches(assembly, modName);
        return Registered.TryAdd(modName, assembly);
    }
}
