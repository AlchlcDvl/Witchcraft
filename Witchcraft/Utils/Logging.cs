using BepInEx.Logging;

namespace Witchcraft.Utils;

/// <summary>Witchcraft's logging class that extends BepInEx's and provides some utilities.</summary>
public static class Logging
{
    /// <summary>A dictionary containing the loggers for registered mods.</summary>
    private static readonly Dictionary<string, ManualLogSource> ModLoggers = new();

    /// <summary>A dictionary containing the saved logs as string for registered mods.</summary>
    private static readonly Dictionary<string, string> SavedLogs = new();

    /// <summary>Creates a logger that displays logs from <paramref name="modName"/>.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <returns>A new <see cref="ManualLogSource"/> used for logging <paramref name="modName"/>'s messages.</returns>
    public static ManualLogSource Init(string modName)
    {
        var key = modName.Replace(" ", string.Empty);
        var log = BepInEx.Logging.Logger.CreateLogSource(key);
        ModLoggers[key] = log;
        SavedLogs[key] = string.Empty;
        return log;
    }

    /// <summary>Creates a logger that displays logs.</summary>
    /// <returns>A new <see cref="ManualLogSource"/> used for logging messages.</returns>
    public static ManualLogSource Init() => Init(Assembly.GetCallingAssembly().GetName().Name);

    /// <summary>Logs messages from <paramref name="modName"/>.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="message">The message being logged.</param>
    /// <param name="level">The level of the message.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    private static void LogSomething(string modName, object message, LogLevel level, bool logIt = false)
    {
        var key = modName.Replace(" ", string.Empty);
        var log = ModLoggers.TryGetValue(key, out var log1) ? log1 : Init(key);
        logIt = logIt || Constants.Debug;
        message = $"[{DateTime.UtcNow}] {message}";

        if (logIt)
        {
            ModLoggers[key].Log(level, message);
            SavedLogs[key] += $"[{level,-7}] {message}\n";
        }
    }

    /// <summary>Logs messages.</summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="level">The level of the message.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    private static void LogSomething(object message, LogLevel level, bool logIt = false) => LogSomething(Assembly.GetCallingAssembly().GetName().Name, message, level, logIt);

    /// <inheritdoc cref="LogSomething(string, object, LogLevel, bool)"/>
    public static void LogError(string modName, object message) => LogSomething(modName, message, LogLevel.Error, true);

    /// <inheritdoc cref="LogSomething(string, object, LogLevel, bool)"/>
    public static void LogMessage(string modName, object message, bool logIt = false) => LogSomething(modName, message, LogLevel.Message, logIt);

    /// <inheritdoc cref="LogSomething(string, object, LogLevel, bool)"/>
    public static void LogFatal(string modName, object message) => LogSomething(modName, message, LogLevel.Fatal, true);

    /// <inheritdoc cref="LogSomething(string, object, LogLevel, bool)"/>
    public static void LogInfo(string modName, object message, bool logIt = false) => LogSomething(modName, message, LogLevel.Info, logIt);

    /// <inheritdoc cref="LogSomething(string, object, LogLevel, bool)"/>
    public static void LogWarning(string modName, object message, bool logIt = false) => LogSomething(modName, message, LogLevel.Warning, logIt);

    /// <inheritdoc cref="LogSomething(string, object, LogLevel, bool)"/>
    public static void LogDebug(string modName, object message, bool logIt = false) => LogSomething(modName, message, LogLevel.Debug, logIt);

    /// <inheritdoc cref="LogSomething(string, object, LogLevel, bool)"/>
    public static void LogNone(string modName, object message, bool logIt = false) => LogSomething(modName, message, LogLevel.None, logIt);

    /// <inheritdoc cref="LogSomething(string, object, LogLevel, bool)"/>
    public static void LogAll(string modName, object message, bool logIt = false) => LogSomething(modName, message, LogLevel.All, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, bool)"/>
    public static void LogError(object message) => LogSomething(message, LogLevel.Error, true);

    /// <inheritdoc cref="LogSomething(object, LogLevel, bool)"/>
    public static void LogMessage(object message, bool logIt = false) => LogSomething(message, LogLevel.Message, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, bool)"/>
    public static void LogFatal(object message) => LogSomething(message, LogLevel.Fatal, true);

    /// <inheritdoc cref="LogSomething(object, LogLevel, bool)"/>
    public static void LogInfo(object message, bool logIt = false) => LogSomething(message, LogLevel.Info, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, bool)"/>
    public static void LogWarning(object message, bool logIt = false) => LogSomething(message, LogLevel.Warning, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, bool)"/>
    public static void LogDebug(object message, bool logIt = false) => LogSomething(message, LogLevel.Debug, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, bool)"/>
    public static void LogNone(object message, bool logIt = false) => LogSomething(message, LogLevel.None, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, bool)"/>
    public static void LogAll(object message, bool logIt = false) => LogSomething(message, LogLevel.All, logIt);

    /// <summary>Saves all of the logs from the mods as a text file for each registered mod within Witchcraft's folder.</summary>
    public static void SaveLogs() => SavedLogs.ForEach((x, y) => GeneralUtils.SaveText($"{x}.txt", y));
}
