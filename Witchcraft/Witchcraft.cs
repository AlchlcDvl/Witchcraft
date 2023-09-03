namespace Main;

[SalemMod]
public class Witchcraft
{
    internal static readonly List<Assembly> RegisteredAssemblies = new();

    public static void Start()
    {
        //Logger.Init();
        Register(Assembly.GetExecutingAssembly());
        Console.WriteLine("Magic is brewing!");
    }

    public static bool Register(Assembly assembly)
    {
        //Logger.LogInfo($"Registering {assembly.FullName}");

        if (RegisteredAssemblies.Contains(assembly))
            return false;

        RegisteredAssemblies.Add(assembly);
        HarmonyQuickPatcher.ApplyHarmonyPatches(assembly);
        return true;
    }

    [QuickPostfix(typeof(ModSettings), nameof(ModSettings.GetBool))]
    public static void QuickPostfix(ref bool __result) => Console.WriteLine(__result);
}