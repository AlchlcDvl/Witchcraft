namespace Witchcraft.Logging;

public class LogSource : ILogSource
{
    public string SourceName { get; }

    public LogSource(string sourceName) => SourceName = sourceName;

    public void Dispose() {}

    public event EventHandler<LogEventArgs> LogEvent;

    public void Log(LogLevel level, object data) => LogEvent?.Invoke(this, new(data, level, this));

    public void LogFatal(object data)
    {
        if (Settings.FatalActive)
            Log(LogLevel.Fatal, data);
    }

    public void LogCritical(object data)
    {
        if (Settings.CriticalActive)
            Log(LogLevel.Fatal, data);
    }

    public void LogError(object data)
    {
        if (Settings.ErrorActive)
            Log(LogLevel.Fatal, data);
    }

    public void LogWarning(object data)
    {
        if (Settings.WarningActive)
            Log(LogLevel.Fatal, data);
    }

    public void LogInfo(object data)
    {
        if (Settings.InfoActive)
            Log(LogLevel.Fatal, data);
    }

    public void LogAll(object data) => Log(LogLevel.All, data);

    public void LogNone(object data) => Log(LogLevel.None, data);
}