namespace Witchcraft.Utils;

/// <summary>Witchcraft's logging class.</summary>
public static class Logging
{
    /// <summary>Logs a message.</summary>
    /// <param name="message">The message being sent.</param>
    /// <param name="modName">The name of the mod.</param>
    public static void Log(string message, string modName = "Witchcraft") => Console.WriteLine($"[{modName}] {message}");
}
