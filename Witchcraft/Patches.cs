using Home.Shared;

namespace Witchcraft;

public static class Patches
{
    [HarmonyPatch(typeof(ApplicationController), nameof(ApplicationController.QuitGame))]
    private static class ExitGamePatch
    {
        public static void Prefix()
        {
            Logging.LogMessage("Patching ApplicationController.QuitGame");
            Logging.SaveLogs();
        }
    }
}