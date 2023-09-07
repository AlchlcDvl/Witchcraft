namespace Witchcraft.Logging;

/// <summary>Helper methods for log level handling.</summary>
public static class LogLevelExtensions
{
    /// <summary>Returns a translation of a log level to it's associated console colour.</summary>
    /// <param name="level">The log level(s).</param>
    /// <returns>A console color associated with the highest log level supplied.</returns>
    public static ConsoleColor GetConsoleColor(this LogLevel level) => level switch
    {
        LogLevel.Fatal => ConsoleColor.Red,
        LogLevel.Critical => ConsoleColor.Magenta,
        LogLevel.Error => ConsoleColor.DarkRed,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Info => ConsoleColor.White,
        LogLevel.All or LogLevel.None or _ => ConsoleColor.Gray
    };
}