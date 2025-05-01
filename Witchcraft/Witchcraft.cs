using NewModLoading;

namespace Witchcraft;

public class Witchcraft : BaseMod<Witchcraft>
{
    public override string Name => "Witchcraft";
    public override string HarmonyId => "alchlcsystm.witchcraft";
    public override bool HasFolder => true;

    public override void Start() => Message("Magic is brewing!", true);

    public override void UponAssetsHandled() => MenuButton.LogsButton.Icon = Assets.GetSprite("Thumbnail");
}

[SalemMenuItem]
public static class MenuButton
{
    public static readonly SalemMenuButton LogsButton = new()
    {
        Label = "Witchcraft Logs",
        OnClick = DumpAndOpen
    };

    private static void DumpAndOpen()
    {
        Directory.GetFiles(Witchcraft.Instance!.ModPath, "*.log").Do(File.Delete);
        LogManager.SaveAllTheLogs();
        Witchcraft.Instance!.OpenDirectory();
    }
}