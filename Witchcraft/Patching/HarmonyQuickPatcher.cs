namespace Witchcraft.Patching;

public static class HarmonyQuickPatcher
{
    private static readonly Harmony Pseudo = new("PseudoHarmony");

    public static void ApplyHarmonyPatches(Assembly assembly)
    {
        assembly.GetTypes()
            .SelectMany(type => type.GetMethods())
            .Where(t => t.GetCustomAttribute<QuickHarmonyAttribute>() != null)
            .ForEach(method =>
            {
                var harmonyAttribute = method.GetCustomAttribute<QuickHarmonyAttribute>()!;
                var harmonyMethod = new HarmonyMethod(method, priority: harmonyAttribute.Priority);
                var targetMethod = (MethodBase)AccessTools.Method(harmonyAttribute.TargetType, harmonyAttribute.MethodName);
                //Logger.LogInfo($"Quick Patching => {targetMethod.Name} ({harmonyAttribute.TargetType})");
                
                switch (harmonyAttribute.PatchType)
                {
                    case HarmonyPatchType.Prefix:
                        Pseudo.Patch(targetMethod, prefix: harmonyMethod);
                        break;

                    case HarmonyPatchType.Postfix:
                        Pseudo.Patch(targetMethod, postfix: harmonyMethod);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
    }
}