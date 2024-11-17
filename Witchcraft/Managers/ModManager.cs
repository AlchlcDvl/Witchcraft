using BepInEx.Bootstrap;
using SalemModLoader;

namespace Witchcraft.Managers;

public static class ModManager
{
    public static readonly Dictionary<string, WitchcraftMod> RegisteredMods = [];
    public static readonly string? ModFoldersPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "ModFolders");
    public static readonly string? ModsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "Mods");
    public static Launch? SMLInstance { get; private set; }

    public static WitchcraftMod Instance(Type type) => RegisteredMods.Values.FirstOrDefault(x => x.ModType == type);

    public static void FetchSmlSingleton() => SMLInstance = Chainloader.PluginInfos.Values.Select(x => x.Instance).OfType<Launch>().FirstOrDefault();
}