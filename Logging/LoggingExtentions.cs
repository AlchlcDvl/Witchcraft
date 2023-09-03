namespace Witchcraft.Logging;

/// <summary>Helper methods for log level handling.</summary>
public static class LogLevelExtensions
{
    /// <summary>Gets the highest log level when there could potentially be multiple levels provided.</summary>
    /// <param name="levels">The log level(s).</param>
    /// <returns>The highest log level supplied.</returns>
    public static LogLevel GetHighestLevel(this LogLevel levels)
    {
        var enums = Enum.GetValues(typeof(LogLevel));
        Array.Sort(enums);

        foreach (var e in enums.Cast<LogLevel>())
        {
            if ((levels & e) != LogLevel.None)
                return e;
        }

        return LogLevel.None;
    }

    /// <summary>Returns a translation of a log level to it's associated console colour.</summary>
    /// <param name="level">The log level(s).</param>
    /// <returns>A console color associated with the highest log level supplied.</returns>
    public static ConsoleColor GetConsoleColor(this LogLevel level) => GetHighestLevel(level) switch
    {
        LogLevel.Fatal => ConsoleColor.Red,
        LogLevel.Critical => ConsoleColor.Magenta,
        LogLevel.Error => ConsoleColor.DarkRed,
        LogLevel.Warning => ConsoleColor.Yellow,
        LogLevel.Info => ConsoleColor.White,
        LogLevel.All or LogLevel.None or _ => ConsoleColor.Gray
    };
}