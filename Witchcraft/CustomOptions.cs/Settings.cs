namespace Witchcraft.CustomOptions;

public static class Settings
{
    public static bool LoggingActive => ModSettings.GetBool("Enable Logging");
    public static bool FatalActive => ModSettings.GetBool("Enable Fatal Logging");
    public static bool CriticalActive => ModSettings.GetBool("Enable Critical Logging");
    public static bool ErrorActive => ModSettings.GetBool("Enable Error Logging");
    public static bool WarningActive => ModSettings.GetBool("Enable Warning Logging");
    public static bool InfoActive => ModSettings.GetBool("Enable Info Logging");
    public static bool PreventClose => ModSettings.GetBool("Prevent Close");
    public static bool ConsoleShiftJis => ModSettings.GetBool("Shift-JIS Encoding");
    public static bool ForceTTYDriver => ModSettings.GetBool("Force TTY Driver");
    public static ConsoleOutRedirectType ConfigConsoleOutRedirectType => ModSettings.GetString("Console Out Redirect Type") switch
    {
        "Console Out" => ConsoleOutRedirectType.ConsoleOut,
        "Standard Out" => ConsoleOutRedirectType.StandardOut,
        "Auto" or _ => ConsoleOutRedirectType.Auto
    };
}