namespace Witchcraft.Logging;

/// <summary>Logs entries using a console spawned by BepInEx.</summary>
public class ConsoleLogListener : ILogListener
{
    /// <inheritdoc />
    public void LogEvent(object sender, LogEventArgs eventArgs)
    {
        ConsoleManager.SetConsoleColor(eventArgs.Level.GetConsoleColor());
        ConsoleManager.ConsoleStream?.Write(eventArgs.ToStringLine());
        ConsoleManager.SetConsoleColor(ConsoleColor.Gray);
    }

    /// <inheritdoc />
    public void Dispose() {}
}
