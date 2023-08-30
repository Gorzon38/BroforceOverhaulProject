using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheGeneralsTraining.Patches.Bros.BroHeart
{
    [HarmonyPatch(typeof(TestVanDammeAnim), "IsAmmoFull")]
    static class IsAmmoFull_Patch
    {
        static bool Prefix(TestVanDammeAnim __instance, ref bool __result)
        {
            // Retrieve sword in Ammo
            if (Main.CanUsePatch && __instance.As<BroveHeart>() && Main.settings.retrieveSwordInAmmo && __instance.As<BroveHeart>().GetFieldValue<bool>("disarmed"))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
