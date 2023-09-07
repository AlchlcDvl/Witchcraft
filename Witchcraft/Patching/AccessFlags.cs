using static System.Reflection.BindingFlags;

namespace Witchcraft.Patching;

public static class AccessFlags
{
    public const BindingFlags AllAccessFlags = Static | Instance | Public | NonPublic;
    public const BindingFlags InstanceAccessFlags = Instance | Public | NonPublic;
    public const BindingFlags StaticAccessFlags = Static | Public | NonPublic;
}