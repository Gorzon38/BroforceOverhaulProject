using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(Desperabro))]
    public class DesperabroPatches
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void StartSerenadingIfNoEnemyAround(Desperabro __instance)
        {
            if (Main.CantUsePatch)
                return;
            if (__instance.mariachiBroType == Desperabro.MariachiBroType.Desperabro)
                return;

            Desperabro leadDesperabro = __instance.GetFieldValue<Desperabro>("desperabro");
            if (leadDesperabro != null && Main.settings.mariachisStartSerenadingIfNoEnemyAround && !leadDesperabro.GetBool("isSerenading"))
            {
                if (__instance.GetBool("isSerenading"))
                {
                    int direction = __instance.GetInt("mariachiDirection");
                    // Check enemy in front of the mariachi
                    if (Map.IsEnemyUnitNearby(__instance.playerNum, __instance.X, __instance.Y + 6f, direction, 160f, 16f, false, true))
                    {
                        __instance.ForceFaceDirection(direction);
                        __instance.SetFieldValue("gunFightFireTimer", 1.5f);
                        __instance.CallMethod("StopSerenading");
                    }
                    // Check enemy behind of the mariachi
                    else if (Map.IsEnemyUnitNearby(__instance.playerNum, __instance.X, __instance.Y + 6f, -direction, 160f, 16f, false, true))
                    {
                        __instance.ForceFaceDirection(-direction);
                        __instance.SetFieldValue("gunFightFireTimer", 1.5f);
                        __instance.CallMethod("StopSerenading");
                    }
                }
                else if (__instance.GetFloat("gunFightFireTimer") <= 0f)
                {
                    __instance.CallMethod("StartSerenading");
                }
            }

        }
    }
}
