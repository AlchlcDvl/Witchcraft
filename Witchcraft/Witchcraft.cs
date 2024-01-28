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

    /// <summary>The icon for Cursed Soul.</summary>
    public static Sprite? CursedSoul;

    /// <summary>The icon for GhostTown.</summary>
    public static Sprite? GhostTown;

    /// <summary>The icon for Vampire.</summary>
    public static Sprite? Vampire;

    /// <summary>The icon for Jester.</summary>
    public static Sprite? Jester;

    /// <summary>The icon for Anonymous Votes.</summary>
    public static Sprite? AnonVotes;

    /// <summary>The icon for Fast Mode.</summary>
    public static Sprite? FastMode;

    /// <summary>The icon for Hidden.</summary>
    public static Sprite? Hidden;

    /// <summary>The icon for Hidden Roles.</summary>
    public static Sprite? HiddenRoles;

    /// <summary>The icon for Marshal.</summary>
    public static Sprite? Marshal;

    /// <summary>The icon for One Trial.</summary>
    public static Sprite? OneTrial;

    /// <summary>The icon for Perfect Town.</summary>
    public static Sprite? PerfectTown;

    /// <summary>The icon for Secret Killers.</summary>
    public static Sprite? SecretKillers;

    /// <summary>The icon for Slow Mode.</summary>
    public static Sprite? SlowMode;

    /// <summary>The icon for Socialite.</summary>
    public static Sprite? Socialite;

    /// <summary>The icon for Stoned.</summary>
    public static Sprite? Stoned;

    /// <summary>The icon for VIP.</summary>
    public static Sprite? VIP;

    /// <summary>The icon for Town Traitor.</summary>
    public static Sprite? TownTraitor;

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
                sprite.DontDestroyOnLoad();

                if (x.Contains("CursedSoul"))
                    CursedSoul = sprite;
                else if (x.Contains("Vampire"))
                    Vampire = sprite;
                else if (x.Contains("GhostTown"))
                    GhostTown = sprite;
                else if (x.Contains("Jester"))
                    Jester = sprite;
                else if (x.Contains("AnonVotes"))
                    AnonVotes = sprite;
                else if (x.Contains("FastMode"))
                    FastMode = sprite;
                else if (x.Contains("Hidden"))
                    Hidden = sprite;
                else if (x.Contains("HiddenRoles"))
                    HiddenRoles = sprite;
                else if (x.Contains("Marshal"))
                    Marshal = sprite;
                else if (x.Contains("OneTrial"))
                    OneTrial = sprite;
                else if (x.Contains("PerfectTown"))
                    PerfectTown = sprite;
                else if (x.Contains("SecretKillers"))
                    SecretKillers = sprite;
                else if (x.Contains("SlowMode"))
                    SlowMode = sprite;
                else if (x.Contains("Socialite"))
                    Socialite = sprite;
                else if (x.Contains("Stoned"))
                    Stoned = sprite;
                else if (x.Contains("TownTraitor"))
                    TownTraitor = sprite;
                else if (x.Contains("VIP"))
                    VIP = sprite;
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
