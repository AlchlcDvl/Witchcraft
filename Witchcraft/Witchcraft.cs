namespace Witchcraft;

[SalemMod]
[SalemMenuItem]
public class Witchcraft
{
    public static string ModFoldersPath => Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "ModFolders");
    public static string ModPath => Path.Combine(ModFoldersPath, "Witchcraft");
    public static readonly Dictionary<string, Sprite?> Assets = [];
    private static Assembly Core => typeof(Witchcraft).Assembly;

    public void Start()
    {
        if (!Directory.Exists(ModPath))
            Directory.CreateDirectory(ModPath);

        Logging.Init();

        Core.GetManifestResourceNames().ForEach(x =>
        {
            try
            {
                if (x.EndsWith(".png"))
                {
                    var sprite = FromResources.LoadSprite(x);
                    var name = x.Split('.')[^2];
                    sprite.DontDestroyOnLoad();
                    sprite.name = name;
                    Assets[name] = sprite;

                    if (name == "Thumbnail")
                        MenuButton.Icon = sprite;
                }
            }
            catch (Exception e)
            {
                Logging.LogError($"Loading {x} had an error:\n{e}");
            }
        });

        Logging.LogMessage("Magic is brewing!", true);
    }

    public static readonly SalemMenuButton MenuButton = new()
    {
        Label = "Witchcraft Logs",
        OnClick = OpenDirectory
    };

    public static void OpenDirectory()
    {
        // code stolen from jan who stole from tuba
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            System.Diagnostics.Process.Start("open", $"\"{ModPath}\"");
        else
            Application.OpenURL($"file://{ModPath}");
    }
}