using System.Runtime.InteropServices;
using SalemModLoader;

namespace Witchcraft;

[SalemMod, SalemMenuItem]
public class Witchcraft : BaseMod<Witchcraft>
{
    public override string Name => "Witchcraft";
    public override bool HasFolder => true;

    public static string LogsPath => Path.Combine(ModManager.ModFoldersPath, "Witchcraft", "Logs");
    public static string ModsPath => Path.Combine(ModManager.ModFoldersPath, "Witchcraft", "Mods");

    public override void Start()
    {
        // Attempting to create a directory
        try
        {
            if (!Directory.Exists(LogsPath))
                Directory.CreateDirectory(LogsPath);

            if (!Directory.Exists(ModsPath))
                Directory.CreateDirectory(ModsPath);
        }
        catch (Exception ex)
        {
            Fatal($"Failed to create a directory because:\n{ex}");
        }

        Message("Magic is brewing!", true);
    }

    public override void UponAssetsLoaded() => LogsButton.Icon = Assets.GetSprite("Thumbnail");

    public static readonly SalemMenuButton LogsButton = new()
    {
        Label = "Witchcraft Logs",
        OnClick = DumpAndOpen
    };

    private static void DumpAndOpen()
    {
        Directory.GetFiles(LogsPath, "*.log").ForEach(File.Delete);
        LogManager.SaveAllTheLogs();
        GeneralUtils.OpenDirectory(LogsPath);
    }

    public static void PatchAll(Harmony harmony, Type[] types) => types.ForEach(type =>
    {
        var customAttribute = type.GetCustomAttribute<ConditionalPatch>();
        var createPatch = customAttribute == null;

        if (!createPatch)
        {
            if (customAttribute!.harmonyKey != null)
                createPatch = ModStates.IsEnabled(customAttribute.harmonyKey);

            if (createPatch)
            {
                createPatch = customAttribute.targetOS switch
                {
                    ConditionalPatch.TargetPlatform.Windows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                    ConditionalPatch.TargetPlatform.Mac => RuntimeInformation.IsOSPlatform(OSPlatform.OSX),
                    _ => true,
                };
            }

            if (customAttribute.invert)
                createPatch = !createPatch;
        }

        if (createPatch)
            harmony.CreateClassProcessor(type).Patch();
    });

    public static bool IsModValid(ModInfo mod)
    {
        var validPlatform = false;
        var validRequiredMods = true;
        var validConflictingMods = true;

        if (mod.Platforms is { Length: > 0 })
        {
            mod.Platforms.ForEach(p =>
            {
                if (p == "Windows" && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    validPlatform = true;
                else if (p == "Mac" && RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    validPlatform = true;
            });
        }
        else
            validPlatform = true;

        if (mod.RequiredMods is { Length: > 0 })
            validRequiredMods = mod.RequiredMods.All(ModStates.IsEnabled);

        if (mod.ConflictingMods is { Length: > 0 })
            validConflictingMods = !mod.ConflictingMods.Any(ModStates.IsEnabled);

        if (!validPlatform)
            mod.LoadFail = ModInfo.LoadFailReason.INVALIDPLATFORM;
        else if (!validRequiredMods)
            mod.LoadFail = ModInfo.LoadFailReason.REQUIREDMODMISSING;
        else if (!validConflictingMods)
            mod.LoadFail = ModInfo.LoadFailReason.CONFLICTINGMODINUSE;

        return validPlatform & validRequiredMods & validConflictingMods;
    }

    public static void DefineModButtons(Type[] types) => types.ForEach(t =>
    {
        if (t.GetCustomAttribute<SalemMenuItem>() == null)
            return;

        var button = typeof(SalemMenuButton);

        foreach (var field in AccessTools.GetDeclaredFields(t))
        {
            if (field.FieldType == button)
                AddHomeSceneButtons.buttons.Add((SalemMenuButton)field.GetValue(null));
        }
    });
}