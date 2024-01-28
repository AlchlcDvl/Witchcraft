namespace Witchcraft.Utils;

/// <summary>A class for general utilities.</summary>
public static class GeneralUtils
{
    /// <summary>Saves text within the mod folder of Witchcraft.</summary>
    /// <param name="fileName">The name of the file being saved.</param>
    /// <param name="textToSave">The text to be saved.</param>
    /// <param name="overrideText">Toggles whether the existing text is overriden or not.</param>
    /// <param name="path">The path where the text is stored.</param>
    public static void SaveText(string fileName, string textToSave, bool overrideText = true, string path = null!)
    {
        try
        {
            File.WriteAllText(Path.Combine(path ?? Witchcraft.ModPath, fileName), (overrideText ? string.Empty : ReadText(fileName, path!)) + textToSave);
        }
        catch
        {
            Logging.LogError($"Unable to save to {fileName}");
        }
    }

    /// <summary>Reads the text from the given file name within Witchcraft's mod folder.</summary>
    /// <param name="fileName">The name of the file being read.</param>
    /// <returns>A string that was read from the file.</returns>
    /// <param name="path">The path where the text is stored.</param>
    public static string ReadText(string fileName, string path = null!)
    {
        try
        {
            return File.ReadAllText(Path.Combine(path ?? Witchcraft.ModPath, fileName));
        }
        catch
        {
            Logging.LogError($"Error Loading {fileName}");
            return string.Empty;
        }
    }
}
