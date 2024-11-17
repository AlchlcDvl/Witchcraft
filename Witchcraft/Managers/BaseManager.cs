namespace Witchcraft.Managers;

public abstract class BaseManager(string name, WitchcraftMod mod)
{
    public string Name { get; } = name;
    public WitchcraftMod Mod { get; } = mod;
}