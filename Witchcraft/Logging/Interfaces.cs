namespace Witchcraft.Logging;

/// <summary>Log source that can output log messages.</summary>
public interface ILogSource : IDisposable
{
    /// <summary>Name of the log source.</summary>
    public string SourceName { get; }

    /// <summary>Event that sends the log message. Call <see cref="EventHandler.Invoke" /> to send a log message.</summary>
    public event EventHandler<LogEventArgs> LogEvent;
}

/// <summary>A generic log listener that receives log events and can route them to some output (e.g. file, console, socket).</summary>
public interface ILogListener : IDisposable
{
    /// <summary>Handle an incoming log event.</summary>
    /// <param name="sender">Log source that sent the event. Don't use; instead use <see cref="LogEventArgs.Source" /></param>
    /// <param name="eventArgs">Information about the log message.</param>
    public void LogEvent(object sender, LogEventArgs eventArgs);
}

public interface IConsoleDriver
{
    public TextWriter StandardOut { get; }
    public TextWriter ConsoleOut { get; }

    public bool ConsoleActive { get; }
    public bool ConsoleIsExternal { get; }

    public void PreventClose();

    public void Initialize(bool alreadyActive, bool useManagedEncoder);

    // Apparently Windows code-pages work in Mono.
    // https://stackoverflow.com/a/33456543
    public void CreateConsole(uint codepage);

    public void DetachConsole();

    public void SetConsoleColor(ConsoleColor color);

    public void SetConsoleTitle(string title);
}