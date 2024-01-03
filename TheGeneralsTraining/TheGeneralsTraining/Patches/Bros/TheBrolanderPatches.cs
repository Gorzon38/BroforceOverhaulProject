using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(TheBrolander))]
    public class TheBrolanderPatches
    {
        [HarmonyPatch("IsAmmoFull")]
        [HarmonyPrefix]
        private static bool NoMoreAmmoPickupWithPocketedSpecial(TheBrolander __instance, ref bool __result)
        {
            if (Main.CanUsePatch)
            {
                if (World.Generation.MapGenV4.ProcGenGameMode.UseProcGenRules)
                {
                    __result = __instance.SpecialAmmo >= 6;
                }
                else if (__instance.pockettedSpecialAmmo.Count > 0)
                {
                    __result = true;
                }
                else
                {
                    __result = __instance.SpecialAmmo >= __instance.maxSpecialAmmo;
                }
                return false;
            }
            return true;
        }
    }
}
