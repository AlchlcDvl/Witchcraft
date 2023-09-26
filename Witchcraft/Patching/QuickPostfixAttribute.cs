namespace Witchcraft.Patching;

/// <summary>Shortened Harmony Postfix.</summary>
public class QuickPostfixAttribute : QuickHarmonyAttribute
{
    /// <summary>Initializes a new instance of the <see cref="QuickPostfixAttribute"/> class.</summary>
    /// <param name="targetType">The <see cref="Type"/> being patched.</param>
    /// <param name="methodName"><paramref name="targetType"/>'s method that's being patched.</param>
    /// <param name="priority">The patch's priority within Harmony.</param>
    public QuickPostfixAttribute(Type targetType, string methodName, int priority = HarmonyLib.Priority.Normal)
        : base(targetType, methodName, HarmonyPatchType.Postfix, priority)
    {
    }
}
