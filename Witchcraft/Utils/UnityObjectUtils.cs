namespace Witchcraft.Utils;

/// <summary>A class for dealing with <see cref="UObject"/>s.</summary>
public static class UnityObjectUtils
{
    /// <summary>Stops <paramref name="obj"/> from being destroyed.</summary>
    /// <param name="obj">The object to stop from being destroyed.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>Passed <paramref name="obj"/>.</returns>
    public static T DontDestroy<T>(this T obj) where T : UObject
    {
        obj.hideFlags |= HideFlags.HideAndDontSave;
        return obj.DontDestroyOnLoad();
    }

    /// <summary>Stops <paramref name="obj"/> from being unloaded.</summary>
    /// <param name="obj">The object to stop from being unloaded.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>Passed <paramref name="obj"/>.</returns>
    public static T DontUnload<T>(this T obj) where T : UObject
    {
        obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        return obj;
    }

    /// <summary>Stops <paramref name="obj"/> from being destroyed on load.</summary>
    /// <param name="obj">The object to stop from being destroyed on load.</param>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <returns>Passed <paramref name="obj"/>.</returns>
    public static T DontDestroyOnLoad<T>(this T obj) where T : UObject
    {
        UObject.DontDestroyOnLoad(obj);
        return obj;
    }

    /// <summary>Destroys the <paramref name="obj"/>.</summary>
    /// <param name="obj">The object to destroy.</param>
    public static void Destroy(this UObject obj) => UObject.Destroy(obj);

    /// <summary>Destroys the <paramref name="obj"/> immediately.</summary>
    /// <param name="obj">The object to destroy immediately.</param>
    public static void DestroyImmediate(this UObject obj) => UObject.DestroyImmediate(obj);
}
