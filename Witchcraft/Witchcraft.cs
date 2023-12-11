namespace Witchcraft;

/// <summary>Witchcraft's main class.</summary>
[SalemMod]
[SalemMenuItem]
public class Witchcraft
{
    /// <summary>Gets Witchcraft's mod path.</summary>
    public static string ModPath => Path.Combine(ModFoldersPath, "Witchcraft");

    /// <summary>Gets the mod folder path used by SML.</summary>
    public static string ModFoldersPath => Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "ModFolders");

    /// <summary>The start function for Witchcraft.</summary>
    public void Start()
    {
        if (!Directory.Exists(ModPath))
            Directory.CreateDirectory(ModPath);

        Logging.Init();
        Logging.LogMessage("Magic is brewing!", true);
    }

    /// <summary>The menu button.</summary>
    public static SalemMenuButton menuButtonName = new()
    {
        Label = "Witchcraft",
        Icon = FromResources.LoadSprite("Witchcraft.Resources.Thumbnail.png"),
        OnClick = OpenDirectory
    };

    /// <summary>Opens Witchcraft's mod folder.</summary>
    public static void OpenDirectory()
    {
        // code stolen from jan who stole from tuba
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            System.Diagnostics.Process.Start("open", $"\"{ModPath}\"");
        else
            Application.OpenURL($"file://{ModPath}");
    }
}
