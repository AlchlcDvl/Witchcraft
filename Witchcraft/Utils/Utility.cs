using System.Text;

namespace Witchcraft.Utils;

public static class Utility
{
    public static string CombinePaths(params string[] parts) => parts.Aggregate(Path.Combine);

    /// <summary>An encoding for UTF-8 which does not emit a byte order mark (BOM).</summary>
    public static Encoding UTF8NoBom { get; } = new UTF8Encoding(false);

    /// <summary>Tries to create a file with the given name</summary>
    /// <param name="path">Path of the file to create</param>
    /// <param name="mode">File open mode</param>
    /// <param name="fileStream">Resulting filestream</param>
    /// <param name="access">File access options</param>
    /// <param name="share">File share options</param>
    /// <returns></returns>
    public static bool TryOpenFileStream(string path, FileMode mode, out FileStream fileStream, FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
    {
        try
        {
            fileStream = new(path, mode, access, share);
            return true;
        }
        catch (IOException) 
        {
            fileStream = null;
            return false;
        }
    }

}