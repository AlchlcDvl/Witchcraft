namespace Witchcraft.Utils;

public static class WitchcraftSettings
{
    public static bool Debug() => ModSettings.GetBool("Enable Debugging", "alchlcsystm.witchcraft");
}