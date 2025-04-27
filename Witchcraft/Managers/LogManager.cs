using BepInEx.Logging;
using BepInEx;

namespace Witchcraft.Managers;

public class LogManager : BaseManager
{
    private int LogMessageCount { get; set; }
    private string SavedLogs { get; set; }
    private ManualLogSource Logger { get; }
    private Func<Enum, ConsoleColor> LogMap { get; }

    private static string? AllLogs = string.Empty;
    private static int AllLogsCount;
    private static DiskLogListener DiskLog { get; set; }

    public static List<LogManager> Managers { get; } = [];

    public LogManager(string name, BaseMod mod, Func<Enum, ConsoleColor> logMap) : base(name, mod)
    {
        LogMessageCount = 0;
        SavedLogs = string.Empty;
        Logger = BepInEx.Logging.Logger.CreateLogSource(Name.Replace(" ", string.Empty));
        LogMap = logMap;
        Managers.Add(this);
    }

    private void LogSomething(object? message, Enum level, bool logIt = false)
    {
        if (!logIt && !WitchcraftSettings.Debug())
            return;

        message ??= "message was null";
        var now = DateTime.UtcNow;
        SavedLogs += $"[{level,-7}, {now}] {message}\n";
        AllLogs += $"[{Name}, {level,-7}, {now}] {message}\n";

        switch (level)
        {
            case LogLevel bll:
            {
                Logger.Log(bll, $"[{now}] {message}");
                break;
            }
            default:
            {
                var console = $"[{level,-7}:{Name,10}] [{now}] {message}";
                DiskLog.LogWriter.WriteLine(console);
                ConsoleManager.SetConsoleColor(LogMap(level));
                ConsoleManager.ConsoleStream.Write(console + Environment.NewLine);
                ConsoleManager.SetConsoleColor(ConsoleColor.Gray);
                break;
            }
        }

        LogMessageCount++;
        AllLogsCount++;

        if (LogMessageCount >= 10)
            SaveLogs();

        if (AllLogsCount >= 10)
            SaveAllLogs();
    }

    public void Error(object? message) => LogSomething(message, LogLevel.Error, true);

    public void Message(object? message, bool logIt = false) => LogSomething(message, LogLevel.Message, logIt);

    public void Fatal(object? message) => LogSomething(message, LogLevel.Fatal, true);

    public void Info(object? message, bool logIt = false) => LogSomething(message, LogLevel.Info, logIt);

    public void Warning(object? message, bool logIt = false) => LogSomething(message, LogLevel.Warning, logIt);

    public void Debug(object? message, bool logIt = false) => LogSomething(message, LogLevel.Debug, logIt);

    public void Custom(object? message, Enum level, bool logIt = false) => LogSomething(message, level, logIt);

    public static void SaveAllLogs() => GeneralUtils.SaveText("AllLogs.log", AllLogs!);

    public void SaveLogs() => GeneralUtils.SaveText($"{Name}.log", SavedLogs);

    public static LogManager? Log<T>() where T : BaseMod => ModManager.Instance<T>()?.Logs;

    public static void SetUpLogging()
    {
        Directory.GetFiles(Witchcraft.LogsPath, "*.log").ForEach(File.Delete);
        DiskLog = BepInEx.Logging.Logger.Listeners.OfType<DiskLogListener>().FirstOrDefault();
    }

    public static void SaveAllTheLogs()
    {
        Managers.ForEach(x => x.SaveLogs());
        SaveAllLogs();
    }
}