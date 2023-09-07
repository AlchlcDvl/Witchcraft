namespace Witchcraft;

[SalemMod]
public class Witchcraft
{
    internal static readonly Dictionary<string, Type[]> Registered = new();
    public static string ModPath => Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Witchcraft");

    public static void Start()
    {
        if (!Directory.Exists(ModPath))
            Directory.CreateDirectory(ModPath);

        _ = Register("Witchcraft", new[] { typeof(Witchcraft) });
        Console.WriteLine("Magic is brewing!");
    }

    public static bool Register(string modName, Type[] types)
    {
        if (Registered.ContainsKey(modName))
        {
            Console.WriteLine($"Types in {modName} are already registered");
            return false;
        }

        foreach (var type in types)
        {
            Console.WriteLine($"Patching {type.Name} from {modName}");
            HarmonyQuickPatcher.ApplyHarmonyPatches(type);
        }

        Registered.Add(modName, types);
        return true;
    }

    /*[QuickPostfix(typeof(Debug), nameof(Debug.Log))]
    public static void Log() => Console.WriteLine(Assembly.GetCallingAssembly().FullName);*/
}

[SalemMenuItem]
public class MenuItem
{
    public static SalemMenuButton menuButtonName = new()
    {
        Label = "Witchcraft",
        Icon = FromResources.LoadSprite("Witchcraft.Resources.thumbnail.png"),
        OnClick = OpenDirectory
    };

    private static void OpenDirectory()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Witchcraft");

        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            System.Diagnostics.Process.Start("open", "\"" + path + "\""); //code stolen from jan who stole from tuba
        else
            Application.OpenURL("file://" + path);
    }
}