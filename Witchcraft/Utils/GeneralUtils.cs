namespace Witchcraft.Utils;

/// <summary>A class for general utilities.</summary>
public static class GeneralUtils
{
    /// <summary>Saves text within the mod folder of Witchcraft.</summary>
    /// <param name="fileName">The name of the file being saved.</param>
    /// <param name="textToSave">The text to be saved.</param>
    /// <param name="overrideText">Toggles whether the existing text is overriden or not.</param>
    /// <param name="path">The path where the text is stored.</param>
    public static void SaveText(string fileName, string textToSave, bool overrideText = true, string path = "")
    {
        try
        {
            if (path == string.Empty)
                path = Witchcraft.ModPath;

            var text = Path.Combine(path, $"{fileName}-temp");
            var text2 = Path.Combine(path, fileName);
            var toOverride = overrideText ? string.Empty : ReadText(fileName);
            File.WriteAllText(text, toOverride + textToSave);
            File.Delete(text2);
            File.Move(text, text2);
        }
        catch
        {
            Logging.Log($"Unable to save {textToSave} to {fileName}");
        }
    }

    /// <summary>Reads the text from the given file name within Witchcraft's mod folder.</summary>
    /// <param name="fileName">The name of the file being read.</param>
    /// <returns>A string that was read from the file.</returns>
    /// <param name="path">The path where the text is stored.</param>
    public static string ReadText(string fileName, string path = "")
    {
        try
        {
            if (path == string.Empty)
                path = Witchcraft.ModPath;

            return File.ReadAllText(Path.Combine(path, fileName));
        }
        catch
        {
            Logging.Log($"Error Loading {fileName}");
            return string.Empty;
        }
    }
}
