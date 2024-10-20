namespace Witchcraft;

[SalemMod]
[SalemMenuItem]
[WitchcraftMod(typeof(Witchcraft), hasFolder: true)]
public class Witchcraft
{
    public static WitchcraftMod? Instance { get; private set;}

    public void Start()
    {
        Instance = ModSingleton<Witchcraft>.Instance;
        Instance!.Message("Magic is brewing!", true);
    }

    [UponAssetsLoaded]
    public static void UponLoad() => LogsButton.Icon = ModSingleton<Witchcraft>.Instance!.Assets?.GetSprite("Thumbnail");

    public static readonly SalemMenuButton LogsButton = new()
    {
        Label = "Witchcraft Logs",
        OnClick = DumpAndOpen
    };

    private static void DumpAndOpen()
    {
        Directory.Delete(Instance!.ModPath, true);
        Directory.CreateDirectory(Instance!.ModPath);
        LogManager.SaveAllTheLogs();
        Instance!.OpenDirectory();
    }
}