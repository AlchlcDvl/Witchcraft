namespace Witchcraft.Logging;

public static class Logger
{
    private static LogSource WitchcraftLogger;

    public static void Init() => WitchcraftLogger = new("Witchcraft");

    public static void LogFatal(object data) => WitchcraftLogger.LogFatal(data);

    public static void LogCritical(object data) => WitchcraftLogger.LogCritical(data);

    public static void LogError(object data) => WitchcraftLogger.LogError(data);

    public static void LogWarning(object data) => WitchcraftLogger.LogWarning(data);

    public static void LogInfo(object data) => WitchcraftLogger.LogInfo(data);

    public static void LogAll(object data) => WitchcraftLogger.LogAll(data);

    public static void LogNone(object data) => WitchcraftLogger.LogNone(data);
}