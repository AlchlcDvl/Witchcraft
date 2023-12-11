namespace Witchcraft.Utils;

/// <summary>Witchcraft's constants class.</summary>
public static class Constants
{
    /// <summary>Gets a value indicating whether mod debugging is on or not.</summary>
    public static bool Debug => ModSettings.GetBool("Enable Debugging", "alchlcsystm.witchcraft");
}
