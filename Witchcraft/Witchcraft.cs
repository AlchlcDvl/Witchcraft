namespace Main;

[SalemMod]
public class Witchcraft
{
    internal static readonly List<Type> Registered = new();
    internal static readonly List<string> Directories = new();

    public static void Start()
    {
        //WitchLogger.Init();
        //Register(Assembly.GetExecutingAssembly());

        if (!Directory.Exists(DiskLogListener.ModPath))
            Directory.CreateDirectory(DiskLogListener.ModPath);

        Directory.GetDirectories(DiskLogListener.ModPath).ForEach(x => Directories.Add(Path.GetFileName(x)));
        Console.WriteLine("Magic is brewing!");
    }

    public static bool Register(Type type)
    {
        //WitchLogger.LogInfo($"Registering {assembly.FullName}");

        if (Registered.Contains(type))
            return false;

        Registered.Add(type);
        HarmonyQuickPatcher.ApplyHarmonyPatches(type);
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
        var text = Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Witchcraft");

        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            System.Diagnostics.Process.Start("open", "\"" + text + "\""); //code stolen from jan who stole from tuba
        else
            Application.OpenURL("file://" + text);
    }
}