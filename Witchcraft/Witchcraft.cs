namespace Witchcraft;

[SalemMod]
[SalemMenuItem]
[WitchcraftMod(typeof(Witchcraft))]
public class Witchcraft
{
    public static WitchcraftModAttribute? Instance { get; private set;}

    public void Start()
    {
        Instance = ModSingleton<Witchcraft>.Instance;
        Instance!.Message("Magic is brewing!", true);
    }

    [UponAssetsLoaded]
    public static void UponLoad()
    {
        LogsButton.Icon = ModSingleton<Witchcraft>.Instance!.Assets?.GetSprite("Thumbnail");
        LogsButton.OnClick = Instance!.OpenDirectory; // why the fuck does this work like this and not when in use in the constructor???
    }

    public static readonly SalemMenuButton LogsButton = new() { Label = "Witchcraft Logs" };
}