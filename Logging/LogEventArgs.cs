namespace Witchcraft.Logging;

/// <summary>Log event arguments. Contains info about the log message.</summary>
public class LogEventArgs : EventArgs
{
    /// <summary>Creates the log event</summary>
    /// <param name="data">Logged data.</param>
    /// <param name="level">Log level of the data.</param>
    /// <param name="source">Log source that emits these args.</param>
    public LogEventArgs(object data, LogLevel level, ILogSource source)
    {
        Data = data;
        Level = level;
        Source = source;
    }

    /// <summary>Logged data.</summary>
    public object Data { get; }

    /// <summary>Log levels for the data.</summary>
    public LogLevel Level { get; }

    /// <summary>Log source that emitted the log event.</summary>
    public ILogSource Source { get; }

    /// <summary>Expresses the <see cref="LogEventArgs"/> as a <see cref="string"/>.</summary>
    /// <returns>Same output as <see cref="ToString" /> but with new line.</returns>
    public override string ToString() => $"[{Level, -10}:{Source.SourceName, 10}] {Data}";

    /// <summary>Like <see cref="ToString"/> but appends newline at the end.</summary>
    /// <returns>Same output as <see cref="ToString"/> but with new line.</returns>
    public string ToStringLine() => $"[{Level, -10}:{Source.SourceName, 10}] {Data}{Environment.NewLine}";
}