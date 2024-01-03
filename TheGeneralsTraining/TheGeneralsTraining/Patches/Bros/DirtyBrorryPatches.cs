using HarmonyLib;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(BroBase))]
    public class DirtyBrorryPatches
    {
        [HarmonyPatch("AnimateMeleeCommon")]
        [HarmonyPostfix]
        private static void ReloadOnPunch(BroBase __instance)
        {
            // Reload on punch hit
            if (Main.CanUsePatch && __instance is DirtyHarry && Main.settings.reloadOnPunch)
            {
                if (__instance.GetBool("meleeHasHit"))
                {
                    // Reset gun stats
                    __instance.SetFieldValue("bulletCount", 0);
                    __instance.SetFieldValue("reloadDelay", 0);
                    __instance.fireDelay = 0f;
                }
                float num = 6;
                Vector3 vector = new Vector3(__instance.X + (float)__instance.Direction * (num + 7f), __instance.Y + 8f, 0f);
                if (Map.HitClosestUnit(__instance, __instance.playerNum, 0, DamageType.None, num, num * 2f, vector.x, vector.y, 0, 0, false, false, __instance.IsMine, false, false))
                {
                    __instance.SetFieldValue("bulletCount", 0);
                    __instance.SetFieldValue("reloadDelay", 0);
                    __instance.fireDelay = 0f;
                }
            }
        }
    }
}
