namespace Witchcraft.Patching;

/// <summary>Types of Harmony patches.</summary>
public enum HarmonyPatchType
{
    /// <summary>Harmony Prefix.</summary>
    Prefix,

    /// <summary>Harmony Postfix.</summary>
    Postfix
}

/// <summary>Quick Harmony patch attribute.</summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class QuickHarmonyAttribute : Attribute
{
    /// <summary>The <see cref="Type"/> being patched.</summary>
    public Type TargetType;

    /// <summary><see cref="TargetType"/>'s method that's being patched.</summary>
    public string MethodName;

    /// <summary>The type of Harmony patch.</summary>
    public HarmonyPatchType PatchType;

    /// <summary>The patch's priority within Harmony.</summary>
    public int Priority;

    /// <summary>Initializes a new instance of the <see cref="QuickHarmonyAttribute"/> class.</summary>
    /// <param name="targetType">The <see cref="Type"/> being patched.</param>
    /// <param name="methodName"><paramref name="targetType"/>'s method that's being patched.</param>
    /// <param name="patchType">The type of Harmony patch.</param>
    /// <param name="priority">The patch's priority within Harmony.</param>
    public QuickHarmonyAttribute(Type targetType, string methodName, HarmonyPatchType patchType, int priority = HarmonyLib.Priority.Normal)
    {
        TargetType = targetType;
        MethodName = methodName;
        PatchType = patchType;
        Priority = priority;
    }
}