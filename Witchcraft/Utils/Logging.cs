using BepInEx.Logging;

namespace Witchcraft.Utils;

/// <summary>Witchcraft's logging class that extends BepInEx's and provides some utilities.</summary>
public static class Logging
{
    /// <summary>A dictionary containing the loggers for registered mods.</summary>
    private static readonly Dictionary<string, ManualLogSource> ModLoggers = new();

    /// <summary>A dictionary containing the saved logs as string for registered mods.</summary>
    private static readonly Dictionary<string, string> SavedLogs = new();

    /// <summary>A dictionary containing the saved keys as string for registered mods and their assemblies.</summary>
    private static readonly Dictionary<string, string> SavedAssemblyNames = new();

    /// <summary>A dictionary containing the saved keys as string for registered mods and their assemblies.</summary>
    private static readonly Dictionary<string, Assembly> SavedAssemblies = new();

    /// <summary>Creates a logger that displays logs from <paramref name="modName"/>. If <paramref name="modName"/> is null, then the the logger is registered under the assembly's name.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <returns>A new <see cref="ManualLogSource"/> used for logging <paramref name="modName"/>'s messages.</returns>
    public static ManualLogSource Init(string modName = null!)
    {
        var assembly = Assembly.GetCallingAssembly();
        var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
        modName ??= assemblyName;
        var key = modName.Replace(" ", string.Empty);
        var log = BepInEx.Logging.Logger.CreateLogSource(key);
        ModLoggers[key] = log;
        SavedLogs[key] = string.Empty;
        SavedAssemblyNames[assemblyName] = key;
        SavedAssemblies[assemblyName] = assembly;
        return log;
    }

    /// <summary>Logs messages for <paramref name="modName"/>.</summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="level">The level of the message.</param>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    private static void LogSomething(object message, LogLevel level, string modName = null!, bool logIt = false)
    {
        var assembly = Assembly.GetCallingAssembly().GetName().Name;

        if (modName == null)
            SavedAssemblyNames.TryGetValue(assembly, out modName);

        modName ??= assembly;
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

    /// <summary>Logs messages for the mod that calls it.</summary>
    /// <param name="message">The message being logged.</param>
    /// <param name="level">The level of the message.</param>
    /// <param name="logIt">An override for whether you want to see the message regardless of the settings.</param>
    private static void LogSomething(object message, LogLevel level, bool logIt = false) => LogSomething(message, level, null!, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, string, bool)"/>
    public static void LogError(string modName, object message) => LogSomething(message, LogLevel.Error, modName, true);

    /// <inheritdoc cref="LogSomething(object, LogLevel, string, bool)"/>
    public static void LogMessage(string modName, object message, bool logIt = false) => LogSomething(message, LogLevel.Message, modName, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, string, bool)"/>
    public static void LogFatal(string modName, object message) => LogSomething(message, LogLevel.Fatal, modName, true);

    /// <inheritdoc cref="LogSomething(object, LogLevel, string, bool)"/>
    public static void LogInfo(string modName, object message, bool logIt = false) => LogSomething(message, LogLevel.Info, modName, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, string, bool)"/>
    public static void LogWarning(string modName, object message, bool logIt = false) => LogSomething(message, LogLevel.Warning, modName, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, string, bool)"/>
    public static void LogDebug(string modName, object message, bool logIt = false) => LogSomething(message, LogLevel.Debug, modName, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, string, bool)"/>
    public static void LogNone(string modName, object message, bool logIt = false) => LogSomething(message, LogLevel.None, modName, logIt);

    /// <inheritdoc cref="LogSomething(object, LogLevel, string, bool)"/>
    public static void LogAll(string modName, object message, bool logIt = false) => LogSomething(message, LogLevel.All, modName, logIt);

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
