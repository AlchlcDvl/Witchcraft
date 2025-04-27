namespace Witchcraft;

public class Witchcraft : BaseMod<Witchcraft>
{
    public override string Name => "Witchcraft";
    public override bool HasFolder => true;

    public override void Start() => Message("Magic is brewing!", true);

    public override void UponAssetsLoaded() => MenuItem.LogsButton.Icon = Assets.GetSprite("Thumbnail");
}

[SalemMod]
public class SmlModLoadingIsShit
{
    public void Start() => Witchcraft.Instance!.Start();
}

[SalemMenuItem]
public class MenuItem
{
    public static readonly SalemMenuButton LogsButton = new()
    {
        Label = "Witchcraft Logs",
        OnClick = DumpAndOpen
    };

    private static void DumpAndOpen()
    {
        Directory.GetFiles(Witchcraft.Instance!.ModPath, "*.log").ForEach(File.Delete);
        LogManager.SaveAllTheLogs();
        Witchcraft.Instance.OpenDirectory();
    }
}