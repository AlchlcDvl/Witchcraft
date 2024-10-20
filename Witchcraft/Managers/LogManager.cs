using BepInEx.Logging;

namespace Witchcraft.Managers;

public class LogManager
{
    public string Name { get; }
    private int LogMessageCount { get; set; }
    private string SavedLogs { get; set; }
    private ManualLogSource? Logger { get; }

    private static string? allLogs = string.Empty;
    private static int allLogsCount;

    public static List<LogManager> Managers { get; set; } = [];

    public LogManager(string? name = null)
    {
        Name = name ?? Assembly.GetCallingAssembly().GetName().Name;
        LogMessageCount = 0;
        SavedLogs = string.Empty;
        Logger = BepInEx.Logging.Logger.CreateLogSource(Name.Replace(" ", string.Empty));
        Managers.Add(this);
        Message("Logger initliased");
    }

    private void LogSomething(object? message, LogLevel level, bool logIt = false)
    {
        if (logIt || WitchcraftSettings.Debug())
        {
            message ??= $"message was null";
            var now = DateTime.UtcNow;
            Logger!.Log(level, $"[{now}] {message}");
            SavedLogs += $"[{level,-7}, {now}] {message}\n";
            allLogs += $"[{Name}, {level,-7}, {now}] {message}\n";
            LogMessageCount++;
            allLogsCount++;

            if (LogMessageCount >= 10 || level is not LogLevel.Message or LogLevel.Info or LogLevel.Debug)
                SaveLogs();

            if (allLogsCount >= 10 || level is not LogLevel.Message or LogLevel.Info or LogLevel.Debug)
                GeneralUtils.SaveText("AllLogs.log", allLogs!);
        }
    }

    public void Error(object? message) => LogSomething(message, LogLevel.Error, true);

    public void Message(object? message, bool logIt = false) => LogSomething(message, LogLevel.Message, logIt);

    public void Fatal(object? message) => LogSomething(message, LogLevel.Fatal, true);

    public void Info(object? message, bool logIt = false) => LogSomething(message, LogLevel.Info, logIt);

    public void Warning(object? message, bool logIt = false) => LogSomething(message, LogLevel.Warning, logIt);

    public void Debug(object? message, bool logIt = false) => LogSomething(message, LogLevel.Debug, logIt);

    public static void SaveAllLogs() => GeneralUtils.SaveText("AllLogs.log", allLogs!);

    public void SaveLogs() => GeneralUtils.SaveText($"{Name}.log", SavedLogs);

    public static LogManager? Log<T>() => ModSingleton<T>.Instance?.Logs;

    public static void SaveAllTheLogs()
    {
        Managers.ForEach(x => x.SaveLogs());
        SaveAllLogs();
    }
}