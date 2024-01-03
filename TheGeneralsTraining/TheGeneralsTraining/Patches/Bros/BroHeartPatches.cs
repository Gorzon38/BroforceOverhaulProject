using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(TestVanDammeAnim))]
    public class BroHeartPatches
    {
        [HarmonyPatch("IsAmmoFull")]
        [HarmonyPrefix]
        private static bool CanTakeAmmoBoxIfDisarmed(TestVanDammeAnim __instance, ref bool __result)
        {
            // If Brove Heart is disarmed, return false so the ammo box will be taken and give the sword back
            if (Main.CanUsePatch && __instance.As<BroveHeart>() && Main.settings.retrieveSwordInAmmo && __instance.As<BroveHeart>().GetFieldValue<bool>("disarmed"))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
