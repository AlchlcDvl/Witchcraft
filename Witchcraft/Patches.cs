namespace Witchcraft;

/// <summary>A class containing Witchcraft patches.</summary>
public static class Patches
{
    /// <summary>Saved logs.</summary>
    public static string? Logs;

    /// <summary>Saves logs to the Witchcraft mod folder.</summary>
    // [QuickPrefix(typeof(WhateverClass), nameof(WhateverClass.ExitGame))] waiting on curt/tina's replies :cri
    public static void SaveLogs()
    {
        GeneralUtils.SaveText("SavedLogs", Logs!);
        Logs = string.Empty;
    }
}
