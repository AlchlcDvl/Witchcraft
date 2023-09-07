namespace Main;

[SalemMod]
public class Witchcraft
{
    internal static readonly List<Type> Registered = new();
    internal static readonly List<string> Directories = new();

    public static void Start()
    {
        WitchLogger.InitializeLoggers();
        WitchLogger.Init();
        Register("Witchcraft", typeof(Witchcraft));

        if (!Directory.Exists(DiskLogListener.ModPath))
            Directory.CreateDirectory(DiskLogListener.ModPath);

        Directory.GetDirectories(DiskLogListener.ModPath).ForEach(x => Directories.Add(Path.GetFileName(x)));
        Console.WriteLine("Magic is brewing!");
    }

    public static bool Register(string modName, params Type[] types)
    {
        if (types.All(Registered.Contains))
        {
            WitchLogger.LogError($"Types in {modName} are already registered");
            return false;
        }

        foreach (var type in types)
        {
            if (!Registered.Contains(type))
            {
                WitchLogger.LogInfo($"Patching {type.Name} from {modName}");
                Registered.Add(type);
                HarmonyQuickPatcher.ApplyHarmonyPatches(type);
            }
        }

        return true;
    }
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