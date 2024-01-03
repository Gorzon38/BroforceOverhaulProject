using HarmonyLib;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(IndianaBrones))]
    public class IndianaBronesPatches
    {
        [HarmonyPatch("AnimateMelee")]
        [HarmonyPrefix]
        private static void FixNoTicketAchievement(IndianaBrones __instance)
        {
            if (Main.CanUsePatch)
            {
                // From method 'IndianaBrones.AnimateMelee()'
                Traverse t = Traverse.Create(__instance);
                TestVanDammeAnim nearbyMook = t.GetFieldValue<TestVanDammeAnim>("nearbyMook");
                if (nearbyMook != null && nearbyMook.CanBeThrown() && t.GetFieldValue<int>("meleeFrame") == 2 && t.GetFieldValue<bool>("highFive"))
                {
                    t.Method("CancelMelee").GetValue();
                    t.Method("ThrowBackMook", new object[] { nearbyMook }).GetValue();

                    Transform parentedToTransform = t.GetFieldValue<TestVanDammeAnim>("nearbyMook").GetParentedToTransform();
                    // Changed the 'if' condition so it works
                    if (parentedToTransform != null && parentedToTransform.name.ToUpper().Contains("BOSS"))
                    {
                        SteamController.UnlockAchievement(SteamAchievement.noticket);
                    }
                }
            }
        }
    }
}

