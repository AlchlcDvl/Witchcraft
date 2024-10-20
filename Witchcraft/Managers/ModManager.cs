namespace Witchcraft.Managers;

public static class ModManager
{
    public static readonly Dictionary<string, WitchcraftMod> RegisteredMods = [];
    public static readonly string? ModFoldersPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "ModFolders");
    public static readonly string? ModsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "Mods");

    public static T? Instance<T>() where T : WitchcraftMod => RegisteredMods.Values.FirstOrDefault(x => x is T) as T;

    public static WitchcraftMod Instance(Type type) => RegisteredMods.Values.FirstOrDefault(x => x.ModType == type);
}