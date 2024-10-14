namespace Witchcraft.Managers;

public static class ModManager
{
    public static readonly Dictionary<string, WitchcraftModAttribute> RegisteredMods = [];
    public static readonly string? ModFoldersPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "ModFolders");
    public static readonly string? ModsPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "SalemModLoader", "Mods");

    public static T? Instance<T>() where T : WitchcraftModAttribute => RegisteredMods.Values.FirstOrDefault(x => x is T) as T;

    public static WitchcraftModAttribute Instance(Type type) => RegisteredMods.Values.FirstOrDefault(x => x.ModType == type);
}