namespace Witchcraft.Utils;

/// <summary>A class for dealing with <see cref="GameObject"/>s.</summary>
public static class GameObjectUtils
{
    /// <summary>Creates a new <see cref="GameObject"/> with the given details.</summary>
    /// <param name="name">The <see cref="GameObject"/>'s name.</param>
    /// <param name="parent">The <see cref="GameObject"/>'s parent.</param>
    /// <param name="scale">The <see cref="GameObject"/>'s scale.</param>
    /// <param name="localPosition">The <see cref="GameObject"/>'s position with respect to the parent.</param>
    /// <returns>A new <see cref="GameObject"/>.</returns>
    public static GameObject CreateGameObject(string name, Transform parent = null!, Vector3? scale = null, Vector3? localPosition = null)
    {
        var gameObject = new GameObject(name);
        gameObject.transform.SetParent(parent);
        gameObject.transform.localScale = scale ?? new(1f, 1f, 1f);
        gameObject.transform.localPosition = localPosition ?? default;
        return gameObject;
    }

    /// <summary>Creates a new child <see cref="GameObject"/> for <paramref name="gameObject"/>.</summary>
    /// <param name="gameObject">The new <see cref="GameObject"/>'s parent.</param>
    /// <param name="name">The <see cref="GameObject"/>'s name.</param>
    /// <param name="scale">The <see cref="GameObject"/>'s scale.</param>
    /// <param name="localPosition">The <see cref="GameObject"/>'s position with respect to the parent.</param>
    /// <returns>A new child <see cref="GameObject"/>.</returns>
    public static GameObject NewChild(this GameObject gameObject, string name, Vector3? scale = null, Vector3? localPosition = null) =>
        CreateGameObject(name, gameObject.transform, scale, localPosition);

    /// <summary>Creates a new child <see cref="GameObject"/> for <paramref name="gameObject"/>.</summary>
    /// <param name="gameObject">The new <see cref="GameObject"/>'s parent.</param>
    /// <param name="name">The <see cref="GameObject"/>'s name.</param>
    /// <param name="localPosition">The <see cref="GameObject"/>'s position with respect to the parent.</param>
    /// <returns>A new child <see cref="GameObject"/>.</returns>
    public static GameObject NewChild(this GameObject gameObject, string name, Vector3 localPosition = default) => CreateGameObject(name, gameObject.transform, null, localPosition);

    /// <summary>Creates a new child <see cref="GameObject"/> for <paramref name="gameObject"/>.</summary>
    /// <param name="gameObject">The new <see cref="GameObject"/>'s parent.</param>
    /// <param name="name">The <see cref="GameObject"/>'s name.</param>
    /// <param name="scale">The <see cref="GameObject"/>'s scale.</param>
    /// <returns>A new child <see cref="GameObject"/>.</returns>
    public static GameObject NewChild(this GameObject gameObject, string name, Vector3? scale = null) => CreateGameObject(name, gameObject.transform, scale);
}
