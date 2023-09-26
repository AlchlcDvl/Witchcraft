using static System.Reflection.BindingFlags;

namespace Witchcraft.Patching;

/// <summary>Witchcraft's access values class.</summary>
public static class AccessFlags
{
    /// <summary>All of the required access flags.</summary>
    public const BindingFlags AllAccessFlags = Static | Instance | Public | NonPublic;

    /// <summary>Flags to see if the method requires an instance to be accessed.</summary>
    public const BindingFlags InstanceAccessFlags = Instance | Public | NonPublic;

    /// <summary>Flags to see if the method is static.</summary>
    public const BindingFlags StaticAccessFlags = Static | Public | NonPublic;
}
