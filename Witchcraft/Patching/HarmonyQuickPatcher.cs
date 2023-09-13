namespace Witchcraft.Patching;

public static class HarmonyQuickPatcher
{
    private static readonly Harmony Pseudo = new("PseudoWitchcraftHarmony");

    public static void ApplyHarmonyPatches(Assembly assembly, string modName)
    {
        if (Witchcraft.Registered.ContainsValue(assembly))
            return;

        assembly.GetTypes()
            .ForEach(x => x.GetMethods(AccessFlags.StaticAccessFlags)
                .Where(t => t.GetCustomAttribute<QuickHarmonyAttribute>() != null)
                .ForEach(method =>
                {
                    var harmonyAttribute = method.GetCustomAttribute<QuickHarmonyAttribute>()!;
                    var harmonyMethod = new HarmonyMethod(method, priority: harmonyAttribute.Priority);
                    var targetMethod = (MethodBase)AccessTools.Method(harmonyAttribute.TargetType, harmonyAttribute.MethodName);
                    Console.WriteLine($"Quick Patching => {targetMethod.Name} ({harmonyAttribute.TargetType}) from {modName}");
                    
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
                })
            );
    }
}