using BepInEx.Configuration;

namespace Witchcraft.Modules;

public class Config<T>(ConfigEntry<T> entry)
{
    internal ConfigEntry<T> Entry { get; set; } = entry;

    public T Value
    {
        get => Entry.Value;
        set => Entry.Value = value;
    }
}