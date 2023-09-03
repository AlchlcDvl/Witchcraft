namespace Witchcraft.CustomOptions;

public static class Settings
{
    public static bool LoggingActive => ModSettings.GetBool("Enable Logging");
}