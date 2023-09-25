namespace Witchcraft;

[SalemMenuItem]
/// <summary>A class for mod's meu item.</summary>
public class MenuItem
{
    /// <summary>The menu button.</summary>
    public static SalemMenuButton menuButtonName = new()
    {
        Label = "Witchcraft",
        Icon = FromResources.LoadSprite("Witchcraft.Resources.Icon.png"),
        OnClick = OpenDirectory
    };

    /// <summary>Opens Witchcraft's mod folder.</summary>
    public static void OpenDirectory()
    {
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            System.Diagnostics.Process.Start("open", $"\"{Witchcraft.ModPath}\""); // code stolen from jan who stole from tuba
        else
            Application.OpenURL($"file://{Witchcraft.ModPath}");
    }
}
