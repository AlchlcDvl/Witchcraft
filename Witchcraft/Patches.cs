using Game.Interface;
using Game.Services;
using Home.Services;
using Home.Shared;
using Server.Shared.State;

namespace Witchcraft;

/// <summary>A class containing Witchcraft patches.</summary>
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

    [HarmonyPatch(typeof(RoleService), nameof(RoleService.Init))]
    private static class PatchRoleService
    {
        public static void Postfix(RoleService __instance)
        {
            Logging.LogMessage("Patching RoleService.Init");
            __instance.roleInfoLookup[Role.VAMPIRE].sprite = Witchcraft.Vampire;
            __instance.roleInfoLookup[Role.CURSED_SOUL].sprite = Witchcraft.CursedSoul;
            __instance.roleInfoLookup[Role.MARSHAL].sprite = Witchcraft.Marshal;
            __instance.roleInfoLookup[Role.SOCIALITE].sprite = Witchcraft.Socialite;
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
            __instance.roleInfoLookup[Role.GHOST_TOWN] = CreateNewInfo(Witchcraft.GhostTown!, Role.GHOST_TOWN, "GT");
        }

        private static RoleInfo CreateNewInfo(Sprite sprite, Role role, string shortRoleName) => new()
        {
            sprite = sprite,
            role = role,
            shortRoleName = shortRoleName
        };
    }

    [HarmonyPatch(typeof(HomeScrollService), nameof(HomeScrollService.Init))]
    private static class PatchScrollService
    {
        public static void Postfix(HomeScrollService __instance)
        {
            Logging.LogMessage("Patching RoleService.Init");
            __instance.scrollInfoLookup_[(int)Role.JESTER].decoration.sprite = Witchcraft.Jester;
            __instance.cursedScrollInfoLookup_[(int)Role.JESTER].decoration.sprite = Witchcraft.Jester;
            __instance.scrollInfoLookup_[(int)Role.MARSHAL].decoration.sprite = Witchcraft.Marshal;
            __instance.cursedScrollInfoLookup_[(int)Role.MARSHAL].decoration.sprite = Witchcraft.Marshal;
            __instance.scrollInfoLookup_[(int)Role.SOCIALITE].decoration.sprite = Witchcraft.Socialite;
            __instance.cursedScrollInfoLookup_[(int)Role.SOCIALITE].decoration.sprite = Witchcraft.Socialite;
        }
    }
}
