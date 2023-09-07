namespace Witchcraft.Logging;

public static class WitchLogger
{
    private static readonly Dictionary<string, LogSource> WitchcraftLoggers = new();

    public static void InitializeLoggers()
    {
        try
        {
            ConsoleManager.CheckOverrides();

            if (ConsoleManager.ConsoleEnabled)
            {
                ConsoleManager.CreateConsole();
                new ConsoleLogListener();
                ConsoleManager.SetConsoleTitle("Witchcraft Console");
            }

            if (Settings.WriteToDisk)
                new DiskLogListener("LogOutput.log", Settings.AppendLogs, Settings.InstantFlushing, Settings.ConcurrentFileLimit);
        }
        catch
        {
            Console.WriteLine("Unable to start loggers");
        }
    }

    public static void Init(params string[] names)
    {
        WitchcraftLoggers.TryAdd("Witchcraft", new("Witchcraft"));

        foreach (var name in names)
            WitchcraftLoggers.TryAdd(name, new(name));
    }

    public static void LogFatal(object data, string name = "Witchcraft") => WitchcraftLoggers[name].LogFatal(data);

    public static void LogCritical(object data, string name = "Witchcraft") => WitchcraftLoggers[name].LogCritical(data);

    public static void LogError(object data, string name = "Witchcraft") => WitchcraftLoggers[name].LogError(data);

    public static void LogWarning(object data, string name = "Witchcraft") => WitchcraftLoggers[name].LogWarning(data);

    public static void LogInfo(object data, string name = "Witchcraft") => WitchcraftLoggers[name].LogInfo(data);

    public static void LogAll(object data, string name = "Witchcraft") => WitchcraftLoggers[name].LogAll(data);

    public static void LogNone(object data, string name = "Witchcraft") => WitchcraftLoggers[name].LogNone(data);
}