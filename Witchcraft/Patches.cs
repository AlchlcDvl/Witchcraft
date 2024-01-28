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
            __instance.roleInfoLookup[Role.VIP] = CreateNewInfo(Witchcraft.VIP!, Role.VIP, "VIP");
            __instance.roleInfoLookup[Role.TOWN_TRAITOR] = CreateNewInfo(Witchcraft.TownTraitor!, Role.TOWN_TRAITOR, "TT");
            __instance.roleInfoLookup[Role.NO_TOWN_HANGED] = CreateNewInfo(Witchcraft.PerfectTown!, Role.NO_TOWN_HANGED, "PT");
            __instance.roleInfoLookup[Role.SLOW_MODE] = CreateNewInfo(Witchcraft.SlowMode!, Role.SLOW_MODE, "SM");
            __instance.roleInfoLookup[Role.FAST_MODE] = CreateNewInfo(Witchcraft.FastMode!, Role.FAST_MODE, "FM");
            __instance.roleInfoLookup[Role.ANONYMOUS_VOTES] = CreateNewInfo(Witchcraft.AnonVotes!, Role.ANONYMOUS_VOTES, "AV");
            __instance.roleInfoLookup[Role.KILLER_ROLES_HIDDEN] = CreateNewInfo(Witchcraft.SecretKillers!, Role.KILLER_ROLES_HIDDEN, "SeK");
            __instance.roleInfoLookup[Role.ROLES_ON_DEATH_HIDDEN] = CreateNewInfo(Witchcraft.HiddenRoles!, Role.ROLES_ON_DEATH_HIDDEN, "HR");
            __instance.roleInfoLookup[Role.ONE_TRIAL_PER_DAY] = CreateNewInfo(Witchcraft.OneTrial!, Role.ONE_TRIAL_PER_DAY, "OT");
            __instance.roleInfoLookup[Role.HIDDEN] = CreateNewInfo(Witchcraft.Hidden!, Role.HIDDEN, "Hidden");
            __instance.roleInfoLookup[Role.STONED] = CreateNewInfo(Witchcraft.Stoned!, Role.STONED, "Stoned");
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
