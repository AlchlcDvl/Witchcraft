namespace Witchcraft.Utils;

/// <summary>A class for all things numbers.</summary>
public static class NumberUtils
{
    /// <summary>Checks if <paramref name="num"/> is within the range formed by <paramref name="min"/> and <paramref name="max"/> depending on the given inclusivity settings.</summary>
    /// <param name="num">The number to be compared.</param>
    /// <param name="min">The minimum reference.</param>
    /// <param name="max">The maximum reference.</param>
    /// <param name="minInclusive">Dictates if <paramref name="min"/> is included in the range for comparison.</param>
    /// <param name="maxInclusive">Dictates if <paramref name="max"/> is included in the range for comparison.</param>
    /// <returns><see langword="true"/> if <paramref name="num"/> happens to be within the range formed by <paramref name="min"/> and <paramref name="max"/>.</returns>
    public static bool IsInRange(float num, float min, float max, bool minInclusive = false, bool maxInclusive = false)
    {
        if (minInclusive && maxInclusive)
            return num >= min && num <= max;
        else if (minInclusive)
            return num >= min && num < max;
        else if (maxInclusive)
            return num > min && num <= max;
        else
            return num > min && num < max;
    }

    /// <summary>Checks if a random number falls under the given probability.</summary>
    /// <param name="probability">The max probability limit.</param>
    /// <returns><see langword="false"/> if the random number was above <paramref name="probability"/> or <paramref name="probability"/> is 0; <see langword="true"/> if the random number was equal to or below <paramref name="probability"/> or <paramref name="probability"/> is 100.</returns>
    public static bool Check(int probability)
    {
        if (probability == 0)
            return false;

        if (probability == 100)
            return true;

        return URandom.RandomRangeInt(1, 100) <= probability;
    }

    /// <summary>Cycles a given value through the provided max and min values. Going over these limits will loop it back onto the other end of the range.</summary>
    /// <param name="max">The maximum limit.</param>
    /// <param name="min">The minimum limit.</param>
    /// <param name="currentVal">The current value going into the cycle.</param>
    /// <param name="increment">Decides if the values is being increased or descreased.</param>
    /// <param name="change">By how much does the value change.</param>
    /// <returns>The new value of <paramref name="currentVal"/> after it has been cycled through the given limits.</returns>
    public static float CycleFloat(float max, float min, float currentVal, bool increment, float change = 1f)
    {
        var value = change * (increment ? 1 : -1);
        currentVal += value;

        if (currentVal > max)
            currentVal = min;
        else if (currentVal < min)
            currentVal = max;

        return currentVal;
    }

    /// <inheritdoc cref="CycleFloat(float, float, float, bool, float)"/>
    public static int CycleInt(int max, int min, int currentVal, bool increment, int change = 1) => (int)CycleFloat(max, min, currentVal, increment, change);

    /// <inheritdoc cref="CycleFloat(float, float, float, bool, float)"/>
    public static byte CycleByte(int max, int min, int currentVal, bool increment, int change = 1) => (byte)CycleInt(max, min, currentVal, increment, change);

    /// <summary>Indicates whether a specified string is null, empty, or consists only of white-space characters.</summary>
    /// <param name="text">The maximum limit.</param>
    /// <returns><see langword="true"/> if the value parameter is null or string.Empty, or if value consists exclusively of white-space characters.</returns>
    public static bool IsNullEmptyOrWhiteSpace(string text) => text is null or "" || text.All(x => x == ' ') || text.Length == 0 || string.IsNullOrWhiteSpace(text);

    /// <summary>Saves text within the mod folder of Witchcraft.</summary>
    /// <param name="fileName">The name of the file being saved.</param>
    /// <param name="textToSave">The text to be saved.</param>
    /// <param name="overrideText">Toggles whether the existing text is overriden or not.</param>
    public static void SaveText(string fileName, string textToSave, bool overrideText = true)
    {
        try
        {
            var text = Path.Combine(Witchcraft.ModPath, $"{fileName}-temp");
            var text2 = Path.Combine(Witchcraft.ModPath, fileName);
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
    public static string ReadText(string fileName)
    {
        try
        {
            return File.ReadAllText(Path.Combine(Witchcraft.ModPath, fileName));
        }
        catch
        {
            Logging.Log($"Error Loading {fileName}");
            return string.Empty;
        }
    }
}
