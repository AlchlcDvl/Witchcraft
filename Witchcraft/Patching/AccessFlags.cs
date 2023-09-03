using System.Reflection;
using static System.Reflection.BindingFlags;

namespace Witchcraft.Patching;

public struct AccessFlags
{
    public const BindingFlags AllAccessFlags = Static | Instance | Public | NonPublic;
    public const BindingFlags InstanceAccessFlags = Instance | Public | NonPublic;
    public const BindingFlags StaticAccessFlags = Static | Public | NonPublic;
}