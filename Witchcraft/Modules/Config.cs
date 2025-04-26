using BepInEx.Configuration;

namespace Witchcraft.Modules;

public abstract class ConfigBase(ConfigEntryBase entry)
{
    internal ConfigEntryBase Base { get; } = entry;
}

public class Config<T>(ConfigEntry<T> entry) : ConfigBase(entry)
{
    internal ConfigEntry<T> Entry { get; } = entry;

    public T Value
    {
        get => Entry.Value;
        set => Entry.Value = value;
    }
}