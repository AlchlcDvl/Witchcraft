using System.Collections;
using System.Diagnostics;

namespace Witchcraft.Utils;

public static class GeneralUtils
{
    public static void SaveText(string fileName, string textToSave, bool overrideText = true, string? path = null)
    {
        path ??= Witchcraft.Instance!.ModPath;

        try
        {
            File.WriteAllText(Path.Combine(path, fileName), (overrideText ? string.Empty : ReadText(fileName, path!)) + textToSave);
        }
        catch (Exception e)
        {
            Witchcraft.Instance!.Error($"Unable to save to {fileName}, {path}:\n{e}");
        }
    }

    public static string ReadText(string fileName, string? path = null)
    {
        path ??= Witchcraft.Instance!.ModPath;

        try
        {
            return File.ReadAllText(Path.Combine(path, fileName));
        }
        catch
        {
            Witchcraft.Instance!.Error($"Error reading {fileName}, {path}");
            return string.Empty;
        }
    }

    public static byte[] ReadFully(this Stream input)
    {
        using var memoryStream = new MemoryStream();
        input.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public static void OpenDirectory(string path)
    {
        // code stolen from jan who stole from tuba
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            Process.Start("open", $"\"{path}\"");
        else
            Application.OpenURL($"file://{path}");
    }

    public static int GetNumeral(this BitArray array, int startIndex, int bitLength)
    {
        var newArray = new BitArray(bitLength);

        for (var i = 0; i < bitLength; i++)
            newArray[i] = array.Length > startIndex + i && array.Get(startIndex + i);

        return newArray.ToNumeral();
    }

    public static int ToNumeral(this BitArray array)
    {
        if (array == null)
        {
            Witchcraft.Instance!.Error("Array is nothing.");
            return 0;
        }

        if (array.Length > 32)
        {
            Witchcraft.Instance!.Error("Must be at most 32 bits long.");
            return 0;
        }

        var result = new int[1];
        array.CopyTo(result, 0);
        return result[0];
    }

    public static MethodInfo? GetMethod(this Type type, Func<MethodInfo, bool> predicate) => type.GetMethods().Find(predicate);
}