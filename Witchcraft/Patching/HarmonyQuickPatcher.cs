namespace Witchcraft.Patching;

/// <summary>Patches all relevant methods with the <see cref="QuickHarmonyAttribute"/> attribute.</summary>
public static class HarmonyQuickPatcher
{
    /// <summary>The pseudo Harmony instance being used to patch types.</summary>
    private static readonly Harmony Pseudo = new("PseudoWitchcraftHarmony");

    /// <summary>Applies relevant patches.</summary>
    /// <param name="modName">The name of the mod.</param>
    /// <param name="assembly">The <see cref="Assembly"/> that <paramref name="modName"/> is attached to.</param>
    public static void ApplyHarmonyPatches(string modName, Assembly assembly)
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
                            throw new ArgumentOutOfRangeException(nameof(method));
                    }
                }));
    }
}