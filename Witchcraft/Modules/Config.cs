using BepInEx.Configuration;

namespace Witchcraft.Modules;

public class ConfigBase;

public class Config<T>(ConfigEntry<T> entry) : ConfigBase
{
    internal ConfigEntry<T> Entry { get; } = entry;

    public T Value
    {
        get => Entry.Value;
        set => Entry.Value = value;
    }
}