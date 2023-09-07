namespace Witchcraft.Logging;

public class LogSource : ILogSource
{
    public string SourceName { get; }

    public LogSource(string sourceName) => SourceName = sourceName;

    public void Dispose() {}

    public event EventHandler<LogEventArgs> LogEvent;

    public void Log(LogLevel level, object data) => LogEvent?.Invoke(this, new(data, level, this));

    public void LogFatal(object data) => Log(LogLevel.Fatal, data);

    public void LogCritical(object data) => Log(LogLevel.Critical, data);

    public void LogError(object data) => Log(LogLevel.Error, data);

    public void LogWarning(object data) => Log(LogLevel.Warning, data);

    public void LogInfo(object data) => Log(LogLevel.Info, data);

    public void LogAll(object data) => Log(LogLevel.All, data);

    public void LogNone(object data) => Log(LogLevel.None, data);
}