namespace Witchcraft.Utils;

/// <summary>A class for dealing with <see cref="GameObject"/>s.</summary>
public static class GameObjectUtils
{
    public static GameObject CreateGameObject(string name, Transform parent = null!, Vector3? scale = null, Vector3 localPosition = default)
    {
        var gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent);
        gameObject.transform.localScale = scale ?? new(1f, 1f, 1f);
        gameObject.transform.localPosition = localPosition;
        return gameObject;
    }

    public static GameObject NewChild(this GameObject gameObject, string name, Vector3? scale = null, Vector3 position = default) =>
        CreateGameObject(name, gameObject.transform, scale, position);

    public static GameObject NewChild(this GameObject gameObject, string name, Vector3 position = default) => CreateGameObject(name, gameObject.transform, null, position);

    public static GameObject NewChild(this GameObject gameObject, string name, Vector3? scale = default) => CreateGameObject(name, gameObject.transform, scale);
}
