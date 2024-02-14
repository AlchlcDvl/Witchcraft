namespace Witchcraft;

/// <summary><see cref="Witchcraft"/>'s main class.</summary>
[SalemMod]
[SalemMenuItem]
public class Witchcraft
{
    /// <summary>Gets the mod folder path used by SML.</summary>
    public static string ModFoldersPath => Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "ModFolders");

    /// <summary>Gets <see cref="Witchcraft"/>'s mod path.</summary>
    public static string ModPath => Path.Combine(ModFoldersPath, "Witchcraft");

    /// <summary>All sprite assets for Witchcraft.</summary>
    public static readonly Dictionary<string, Sprite?> Assets = new();

    /// <summary>Gets the core assembly for .</summary>
    private static Assembly Core => typeof(Witchcraft).Assembly;

    /// <summary>The start function for Witchcraft.</summary>
    public void Start()
    {
        if (!Directory.Exists(ModPath))
            Directory.CreateDirectory(ModPath);

        Logging.Init();

        Core.GetManifestResourceNames().ForEach(x =>
        {
            if (x.EndsWith(".png", StringComparison.InvariantCulture))
            {
                var sprite = FromResources.LoadSprite(x);
                var name = x.Split('.')[^2];
                sprite.DontDestroyOnLoad();
                sprite.name = name;
                Assets[name] = sprite;
            }
        });

        Logging.LogMessage("Magic is brewing!", true);
    }

    /// <summary>The menu button for <see cref="Witchcraft"/>'s.</summary>
    public static SalemMenuButton menuButtonName = new()
    {
        Label = "Witchcraft",
        Icon = FromResources.LoadSprite("Witchcraft.Resources.Thumbnail.png"),
        OnClick = OpenDirectory
    };

    /// <summary>Opens <see cref="Witchcraft"/>'s mod folder.</summary>
    public static void OpenDirectory()
    {
        // code stolen from jan who stole from tuba
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            System.Diagnostics.Process.Start("open", $"\"{ModPath}\"");
        else
            Application.OpenURL($"file://{ModPath}");
    }
}
