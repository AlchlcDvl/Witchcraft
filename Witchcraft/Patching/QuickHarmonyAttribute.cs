namespace Witchcraft.Patching;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class QuickHarmonyAttribute : Attribute
{
    public Type TargetType;
    public string MethodName;
    public HarmonyPatchType PatchType;
    public int Priority;

    public QuickHarmonyAttribute(Type targetType, string methodName, HarmonyPatchType patchType, int priority = HarmonyLib.Priority.Normal)
    {
        TargetType = targetType;
        MethodName = methodName;
        PatchType = patchType;
        Priority = priority;
    }
}

public class QuickPostfixAttribute : QuickHarmonyAttribute
{
    public QuickPostfixAttribute(Type targetType, string methodName, int priority = HarmonyLib.Priority.Normal) : base(targetType, methodName, HarmonyPatchType.Postfix, priority) {}
}

public class QuickPrefixAttribute : QuickHarmonyAttribute
{
    public QuickPrefixAttribute(Type targetType, string methodName, int priority = HarmonyLib.Priority.Normal) : base(targetType, methodName, HarmonyPatchType.Prefix, priority) {}
}

public enum HarmonyPatchType
{
    Prefix,
    Postfix
}