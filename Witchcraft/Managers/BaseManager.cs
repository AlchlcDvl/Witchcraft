namespace Witchcraft.Managers;

public abstract class BaseManager(string name, BaseMod mod)
{
    public string Name { get; } = name;
    public BaseMod Mod { get; } = mod;
}