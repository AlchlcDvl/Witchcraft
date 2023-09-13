namespace Witchcraft;

[SalemMod]
public class Witchcraft
{
    internal static readonly Dictionary<string, Assembly> Registered = new();
    public static string ModPath => Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Witchcraft");

    public static void Start()
    {
        if (!Directory.Exists(ModPath))
            Directory.CreateDirectory(ModPath);

        _ = Register("Witchcraft", Assembly.GetExecutingAssembly());
        Console.WriteLine("Magic is brewing!");
    }

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

[SalemMenuItem]
public class MenuItem
{
    public static SalemMenuButton menuButtonName = new()
    {
        Label = "Witchcraft",
        Icon = FromResources.LoadSprite("Witchcraft.Resources.Icon.png"),
        OnClick = OpenDirectory
    };

    private static void OpenDirectory()
    {
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            System.Diagnostics.Process.Start("open", $"\"{Witchcraft.ModPath}\""); //code stolen from jan who stole from tuba
        else
            Application.OpenURL($"file://{Witchcraft.ModPath}");
    }
}