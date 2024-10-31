using System.Xml;
using Home.Services;
using Home.Shared;
using SalemModLoader;

namespace Witchcraft;

[HarmonyPatch(typeof(ApplicationController), nameof(ApplicationController.QuitGame))]
public static class ExitGamePatch
{
    public static void Prefix() => LogManager.SaveAllTheLogs();
}

[HarmonyPatch(typeof(HomeLocalizationService), nameof(HomeLocalizationService.RebuildStringTables))]
[HarmonyPriority(Priority.First)]
public static class DumpStringTables
{
    public static void Postfix(HomeLocalizationService __instance)
    {
        foreach (var manager in AssetManager.Managers)
        {
            var text = __instance.GetStringTableFilename(__instance.m_selectedUILanguageId).Split('/')[^1].Replace(".xml", "");

            if (!manager.Xmls.TryGetValue(text, out var xml))
            {
                if (!manager.Xmls.TryGetValue("StringTable", out xml))
                    continue;
                else
                    text = "StringTable";
            }

            if (StringUtils.IsNullEmptyOrWhiteSpace(xml))
                continue;

            manager.AssetLogs.Message($"Loading: {text} xml");
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            foreach (var textEntry in XMLStringTable.Load(xmlDocument).entries)
            {
                if (!__instance.stringTable_.ContainsKey(textEntry.key))
                    __instance.stringTable_.Add(textEntry.key, textEntry.value);
                else
                    manager.AssetLogs.Warning($"{manager.Name}: Duplicate String Table Key \"{textEntry.key}\"!");

                if (StringUtils.IsNullEmptyOrWhiteSpace(textEntry.style))
                    continue;

                if (!__instance.styleTable_.ContainsKey(textEntry.key))
                    __instance.styleTable_.Add(textEntry.key, textEntry.style);
                else
                    manager.AssetLogs.Warning($"{manager.Name}: Duplicate Style Table Key \"{textEntry.key}\"!");
            }
        }
    }
}

[HarmonyPatch(typeof(Launch), "SetAssetBundleValues")]
public static class LoadAllAssets
{
    public static void Postfix() => AssetManager.Managers.ForEach(x => x.BeginLoading());
}