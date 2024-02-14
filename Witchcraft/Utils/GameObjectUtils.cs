namespace Witchcraft.Utils;

public static class GameObjectUtils
{
    public static GameObject CreateGameObject(string name, Transform? parent = null, Vector3? scale = null, Vector3? localPosition = null)
    {
        var gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent);
        gameObject.transform.localScale = scale ?? new(1f, 1f, 1f);
        gameObject.transform.localPosition = localPosition ?? default;
        return gameObject;
    }

    public static GameObject NewChild(this GameObject gameObject, string name, Vector3? scale = null, Vector3? localPosition = null) => CreateGameObject(name, gameObject.transform, scale,
        localPosition);

    public static GameObject NewChild(this GameObject gameObject, string name, Vector3 localPosition = default) => CreateGameObject(name, gameObject.transform, null, localPosition);

    public static GameObject NewChild(this GameObject gameObject, string name, Vector3? scale = null) => CreateGameObject(name, gameObject.transform, scale);
}