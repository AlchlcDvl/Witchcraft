namespace Witchcraft.Utils;

/// <summary>Witchcraft's logging class.</summary>
public static class Logging
{
    /// <summary>Logs a message.</summary>
    /// <param name="message">The message being sent.</param>
    /// <param name="level">The level of severeity of the message being logged.</param>
    /// <param name="modName">The name of the mod.</param>
    public static void Log(string message, string level, string modName = "Witchcraft")
    {
        var paste = $"[{modName:15} : {level:15}] {message}";
        Patches.Logs += paste;
        Console.WriteLine(paste);
    }
}
