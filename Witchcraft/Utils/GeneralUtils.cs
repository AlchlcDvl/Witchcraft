namespace Witchcraft.Utils;

public static class GeneralUtils
{
    public static void SaveText(string fileName, string textToSave, bool overrideText = true, string? path = null)
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

    public static string ReadText(string fileName, string? path = null)
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