using BepInEx.Logging;

namespace Witchcraft.Utils;

/// <summary>Witchcraft's logging class that extends BepInEx's and provides some utilities.</summary>
public static class Logging
{
    /// <summary>A dictionary containing the loggers for registered mods.</summary>
    private static readonly Dictionary<string, ManualLogSource> ModLoggers = new();

    /// <summary>A dictionary containing the saved logs as string for registered mods.</summary>
    private static readonly Dictionary<string, string> SavedLogs = new();

    /// <summary>Creates a logger that displays logs from <paramref name="modName"/>. If <paramref name="modName"/> is null, then the the logger is registered under the assembly's name.</summary>
    /// <param name="modName">The name of the mod.</param>
    public static void InitVoid(string? modName = null!) => Init(modName ?? Assembly.GetCallingAssembly().GetName().Name);

    /// <summary>Creates a logger that displays logs from <paramref name="modName"/>. If <paramref name="modName"/> is null, then the the logger is registered under the assembly's name.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <returns>A new <see cref="ManualLogSource"/> used for logging <paramref name="modName"/>'s messages.</returns>
    public static ManualLogSource Init(string? modName = null!)
    {
        modName ??= Assembly.GetCallingAssembly().GetName().Name;
        var key = modName.Replace(" ", string.Empty);
        var log = BepInEx.Logging.Logger.CreateLogSource(key);
        ModLoggers[key] = log;
        SavedLogs[key] = string.Empty;
        return log;
    }

    /// <summary>Logs messages for <paramref name="modName"/>.</summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="level">The level of the message.</param>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    private static void LogSomething(object? message, LogLevel level, string? modName, bool logIt = false)
    {
        var key = modName!.Replace(" ", string.Empty);
        var log = ModLoggers.TryGetValue(key, out var log1) ? log1 : Init(key);
        message ??= $"message was null";
        message = $"[{DateTime.UtcNow}] {message}";

        if (logIt || WitchcraftConstants.Debug)
        {
            ModLoggers[key].Log(level, message);
            SavedLogs[key] += $"[{level,-7}] {message}\n";
        }
    }

    /// <summary>Logs errors for the mod that calls it.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="message">The error being logged.</param>
    public static void LogError(string? modName, object? message) => LogSomething(message, LogLevel.Error, modName, true);

    /// <summary>Logs messages for the mod that calls it.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="message">The message being logged.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    public static void LogMessage(string? modName, object? message, bool logIt = false) => LogSomething(message, LogLevel.Message, modName, logIt);

    /// <summary>Logs fatal errors for the mod that calls it.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="message">The fatal error being logged.</param>
    public static void LogFatal(string? modName, object? message) => LogSomething(message, LogLevel.Fatal, modName, true);

    /// <summary>Logs info messages for the mod that calls it.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="message">The info message being logged.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    public static void LogInfo(string? modName, object? message, bool logIt = false) => LogSomething(message, LogLevel.Info, modName, logIt);

    /// <summary>Logs warnings for the mod that calls it.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="message">The message being logged.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    public static void LogWarning(string? modName, object? message, bool logIt = false) => LogSomething(message, LogLevel.Warning, modName, logIt);

    /// <summary>Logs debug messages for the mod that calls it.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="message">The message being logged.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    public static void LogDebug(string? modName, object? message, bool logIt = false) => LogSomething(message, LogLevel.Debug, modName, logIt);

    /// <summary>Logs errors for the mod that calls it.</summary>
    /// <param name="message">The message being logged.</param>
    public static void LogError(object? message) => LogSomething(message, LogLevel.Error, Assembly.GetCallingAssembly().GetName().Name, true);

    /// <summary>Logs messages for the mod that calls it.</summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    public static void LogMessage(object? message, bool logIt = false) => LogSomething(message, LogLevel.Message, Assembly.GetCallingAssembly().GetName().Name, logIt);

    /// <summary>Logs fatal errors for the mod that calls it.</summary>
    /// <param name="message">The message being logged.</param>
    public static void LogFatal(object? message) => LogSomething(message, LogLevel.Fatal, Assembly.GetCallingAssembly().GetName().Name, true);

    /// <summary>Logs info messages for the mod that calls it.</summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    public static void LogInfo(object? message, bool logIt = false) => LogSomething(message, LogLevel.Info, Assembly.GetCallingAssembly().GetName().Name, logIt);

    /// <summary>Logs warnings for the mod that calls it.</summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    public static void LogWarning(object? message, bool logIt = false) => LogSomething(message, LogLevel.Warning, Assembly.GetCallingAssembly().GetName().Name, logIt);

    /// <summary>Logs debug messages for the mod that calls it.</summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    public static void LogDebug(object? message, bool logIt = false) => LogSomething(message, LogLevel.Debug, Assembly.GetCallingAssembly().GetName().Name, logIt);

    /// <summary>Saves all of the logs from the mods as a text file for each registered mod within Witchcraft's folder.</summary>
    public static void SaveLogs() => SavedLogs.ForEach((x, y) => GeneralUtils.SaveText($"{x}.txt", y));
}
