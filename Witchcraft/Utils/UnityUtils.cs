namespace Witchcraft.Utils;

public static class UnityUtils
{
    public static void Destroy(this UObject obj) => UObject.Destroy(obj);

    public static void DestroyImmediate(this UObject obj) => UObject.DestroyImmediate(obj);

    public static T? AddComponent<T>(this Component component) where T : Component => component?.gameObject?.AddComponent<T>();

    public static T? GetComponent<T>(this Component component) where T : Component => component?.gameObject?.GetComponent<T>();

    public static T? EnsureComponent<T>(this Component component) where T : Component => component?.gameObject?.EnsureComponent<T>();

    public static T? EnsureComponent<T>(this GameObject gameObject) where T : Component => gameObject?.TryGetComponent<T>(out var comp) == true ? comp : gameObject?.AddComponent<T>();

    public static Transform Get(this Component obj, int childIndex) => obj.transform.GetChild(childIndex);

    public static Transform FindRecursive(this Transform self, string exactName) => self.FindRecursive(child => child.name == exactName);

    public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
    {
        for (var i = 0; i < self.childCount; i++)
        {
            var child = self.GetChild(i);

            if (selector(child))
                return child;

            var finding = child.FindRecursive(selector);

            if (finding)
                return finding;
        }

        return null!;
    }

    public static IEnumerable<Transform> FindAllRecursive(this Transform self, string exactName) => self.FindAllRecursive(child => child.name == exactName);

    public static IEnumerable<Transform> FindAllRecursive(this Transform self, Func<Transform, bool> selector)
    {
        for (var i = 0; i < self.childCount; i++)
        {
            var child = self.GetChild(i);

            if (selector(child))
                yield return child;

            foreach (var found in child.FindAllRecursive(selector))
                yield return found;
        }
    }

    public static IEnumerable<T> GetAllComponents<T>(this Transform self) where T : Component
    {
        if (self.TryGetComponent<T>(out var comp))
            yield return comp;

        for (var i = 0; i < self.childCount; i++)
        {
            foreach (var comp2 in self.GetChild(i).GetAllComponents<T>())
                yield return comp2;
        }
    }

    public static T? AddComponent<T>(this Transform self, string exactName) where T : Component => self.FindRecursive(exactName).AddComponent<T>();

    public static T? GetComponent<T>(this Transform self, string exactName) where T : Component => self.FindRecursive(exactName).GetComponent<T>();

    public static T? EnsureComponent<T>(this Transform self, string exactName) where T : Component => self.FindRecursive(exactName).EnsureComponent<T>();

    public static GameObject CreateGameObject(string name, Transform? parent = null, Vector3? scale = null, Vector3? localPosition = null)
    {
        var gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent);
        gameObject.transform.localScale = scale ?? Vector3.one;
        gameObject.transform.localPosition = localPosition ?? default;
        return gameObject;
    }

    public static GameObject NewChild(this GameObject gameObject, string name, Vector3? localPosition = null, Vector3? scale = null) => CreateGameObject(name, gameObject.transform, scale,
        localPosition);

    public static float Dot(Color a, Color b) => (a.r * b.r) + (a.g * b.g) + (a.b * b.b) + (a.a * b.a);

    public static Color Cross(Color a, Color b) => new((a.g * b.b) - (a.b * b.g), (a.r * b.b) - (a.b * b.r), (a.r * b.g) - (a.g * b.b), Mathf.Lerp(a.a, b.a, 0.5f));

    public static float Dot(Color32 a, Color32 b) => (a.r * b.r) + (a.g * b.g) + (a.b * b.b) + (a.a * b.a);

    public static Color32 Cross(Color32 a, Color32 b) => new((byte)((a.g * b.b) - (a.b * b.g)), (byte)((a.r * b.b) - (a.b * b.r)), (byte)((a.r * b.g) - (a.g * b.b)),
        (byte)Mathf.Lerp(a.a, b.a, 0.5f));

    public static string ToHtmlStringRGBA(this Color32 color) => $"{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";

    public static string ToHtmlStringRGBA(this Color color) => ((Color32)color).ToHtmlStringRGBA();

    public static Transform GetParent(this Transform self, string name) => self.parent.name == name ? self.parent : self.parent.GetParent(name);

    public static Color ToColor(this string html) => ColorUtility.TryParseHtmlString(html.StartsWith("#") ? html : $"#{html}", out var color) ? color : Color.white;

    public static void ClearChildren(this Transform self)
    {
        for (var i = self.childCount - 1; i > -1; i--)
        {
            var child = self.GetChild(i);
            child.transform.SetParent(null);
            child.gameObject.Destroy();
        }
    }
}