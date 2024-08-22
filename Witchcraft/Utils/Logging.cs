using BepInEx.Logging;

namespace Witchcraft.Utils;

public static class Logging
{
    private static readonly Dictionary<string, ManualLogSource> ModLoggers = [];
    private static readonly Dictionary<string, string> SavedLogs = [];
    private static readonly Dictionary<string, int> LogMessageCount = [];
    private static string? AllLogs = "";
    private static int AllLogsCount = 0;

    public static void InitVoid(string? modName = null) => Init(modName ?? Assembly.GetCallingAssembly().GetName().Name);

    public static ManualLogSource Init(string? modName = null)
    {
        modName ??= Assembly.GetCallingAssembly().GetName().Name;
        var key = modName.Replace(" ", string.Empty);
        var log = BepInEx.Logging.Logger.CreateLogSource(key);
        ModLoggers[key] = log;
        SavedLogs[key] = string.Empty;
        LogMessageCount[key] = 0;
        return log;
    }

    private static void LogSomething(object? message, LogLevel level, string? modName, bool logIt = false)
    {
        var key = modName!.Replace(" ", string.Empty);
        var log = ModLoggers.TryGetValue(key, out var log1) ? log1 : Init(key);
        message ??= $"message was null";

        if (logIt || WitchcraftConstants.Debug)
        {
            var now = DateTime.UtcNow;
            ModLoggers[key].Log(level, $"[{now}] {message}");
            SavedLogs[key] += $"[{level, -7}, {now}] {message}\n";
            AllLogs += $"[{key}, {level, -7}, {now}] {message}\n";
            LogMessageCount[key]++;
            AllLogsCount++;

            if (LogMessageCount[key] >= 10 || level is not LogLevel.Message or LogLevel.Info or LogLevel.Debug)
            {
                SaveLogs(key);
                GeneralUtils.SaveText("AllLogs.txt", AllLogs!);
            }
            else if (AllLogsCount >= 10 || level is not LogLevel.Message or LogLevel.Info or LogLevel.Debug)
                GeneralUtils.SaveText("AllLogs.txt", AllLogs!);
        }
    }

    public static void LogError(string? modName, object? message) => LogSomething(message, LogLevel.Error, modName, true);

    public static void LogMessage(string? modName, object? message, bool logIt = false) => LogSomething(message, LogLevel.Message, modName, logIt);

    public static void LogFatal(string? modName, object? message) => LogSomething(message, LogLevel.Fatal, modName, true);

    public static void LogInfo(string? modName, object? message, bool logIt = false) => LogSomething(message, LogLevel.Info, modName, logIt);

    public static void LogWarning(string? modName, object? message, bool logIt = false) => LogSomething(message, LogLevel.Warning, modName, logIt);

    public static void LogDebug(string? modName, object? message, bool logIt = false) => LogSomething(message, LogLevel.Debug, modName, logIt);

    public static void LogError(object? message) => LogSomething(message, LogLevel.Error, Assembly.GetCallingAssembly().GetName().Name, true);

    public static void LogMessage(object? message, bool logIt = false) => LogSomething(message, LogLevel.Message, Assembly.GetCallingAssembly().GetName().Name, logIt);

    public static void LogFatal(object? message) => LogSomething(message, LogLevel.Fatal, Assembly.GetCallingAssembly().GetName().Name, true);

    public static void LogInfo(object? message, bool logIt = false) => LogSomething(message, LogLevel.Info, Assembly.GetCallingAssembly().GetName().Name, logIt);

    public static void LogWarning(object? message, bool logIt = false) => LogSomething(message, LogLevel.Warning, Assembly.GetCallingAssembly().GetName().Name, logIt);

    public static void LogDebug(object? message, bool logIt = false) => LogSomething(message, LogLevel.Debug, Assembly.GetCallingAssembly().GetName().Name, logIt);

    public static void SaveLogs()
    {
        SavedLogs.Keys.ForEach(SaveLogs);
        GeneralUtils.SaveText("AllLogs.txt", AllLogs!);
    }

    public static void SaveLogs(string? modName = null!)
    {
        var key = modName!.Replace(" ", string.Empty);

        if (SavedLogs.TryGetValue(key!, out var logs))
            GeneralUtils.SaveText($"{key}.txt", logs);
    }
}