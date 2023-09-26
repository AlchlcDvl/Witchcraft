namespace Witchcraft.Patching;

/// <summary>Shortened Harmony Prefix.</summary>
public class QuickPrefixAttribute : QuickHarmonyAttribute
{
    /// <summary>Initializes a new instance of the <see cref="QuickPrefixAttribute"/> class.</summary>
    /// <param name="targetType">The <see cref="Type"/> being patched.</param>
    /// <param name="methodName"><paramref name="targetType"/>'s method that's being patched.</param>
    /// <param name="priority">The patch's priority within Harmony.</param>
    public QuickPrefixAttribute(Type targetType, string methodName, int priority = HarmonyLib.Priority.Normal)
        : base(targetType, methodName, HarmonyPatchType.Prefix, priority)
    {
    }
}
