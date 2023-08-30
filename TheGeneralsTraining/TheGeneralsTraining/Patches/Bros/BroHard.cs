using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros.BroHard0
{
    [HarmonyPatch(typeof(TestVanDammeAnim), "GetSpeed", MethodType.Getter)]
    static class TestVanDammeAnim_FasterBroHardWhenDucking_Patch
    {
        public const float ONE = 1f;
        public static float duckingSpeedMultiplier = 1.1f;
        public static float barbedWireSlow = 0.5f;
        static bool Prefix(TestVanDammeAnim __instance, ref float __result)
        {
            if(Main.CanUsePatch && __instance is BroHard && Main.settings.broHardFasterWhenDucking)
            {
                try
                {
                    bool surroundedByBarbedWire =__instance.CallMethod<bool>("IsSurroundedByBarbedWire");
                    if (__instance.player == null)
                    {
                        __result = __instance.speed * ((!surroundedByBarbedWire) ? ONE : barbedWireSlow) * (__instance.IsDucking ? duckingSpeedMultiplier : ONE);
                    }
                    __result = __instance.player.ValueOrchestrator.GetModifiedFloatValue(Rogueforce.ValueOrchestrator.ModifiableType.MovementSpeed, __instance.speed) * ((!surroundedByBarbedWire) ? ONE : barbedWireSlow) * (__instance.IsDucking ? duckingSpeedMultiplier : ONE);
                    return false;
                }
                catch(Exception e)
                {
                    Main.Log(e);
                }
            }
            return true;
        }
    }
}
