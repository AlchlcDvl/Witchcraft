namespace Witchcraft;

[SalemMod, SalemMenuItem]
public class Witchcraft : BaseMod<Witchcraft>
{
    public override string Name => "Witchcraft";
    public override bool HasFolder => true;

    public override void Start() => Message("Magic is brewing!", true);

    public override void UponAssetsLoaded() => LogsButton.Icon = Assets.GetSprite("Thumbnail");

    public static readonly SalemMenuButton LogsButton = new()
    {
        Label = "Witchcraft Logs",
        OnClick = DumpAndOpen
    };

    private static void DumpAndOpen()
    {
        Directory.Delete(Instance!.ModPath, true);
        Directory.CreateDirectory(Instance.ModPath);
        LogManager.SaveAllTheLogs();
        Instance.OpenDirectory();
    }
}