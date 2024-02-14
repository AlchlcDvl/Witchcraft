namespace Witchcraft.Utils;

public static class UnityObjectUtils
{
    public static T DontDestroy<T>(this T obj) where T : UObject
    {
        obj.hideFlags |= HideFlags.HideAndDontSave;
        return obj.DontDestroyOnLoad();
    }

    public static T DontUnload<T>(this T obj) where T : UObject
    {
        obj.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        return obj;
    }

    public static T DontDestroyOnLoad<T>(this T obj) where T : UObject
    {
        UObject.DontDestroyOnLoad(obj);
        return obj;
    }

    public static void Destroy(this UObject obj) => UObject.Destroy(obj);

    public static void DestroyImmediate(this UObject obj) => UObject.DestroyImmediate(obj);
}