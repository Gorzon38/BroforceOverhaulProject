using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros.Desperabro0
{
    [HarmonyPatch(typeof(Desperabro), "Update")]
    static class Update_Patch
    {
        static void Postfix(Desperabro __instance)
        {
            if (Main.CantUsePatch) return;

            try
            {
                if(Main.settings.mariachisPlayMusic && __instance.mariachiBroType != Desperabro.MariachiBroType.Desperabro && !__instance.GetFieldValue<Desperabro>("desperabro").GetBool("isSerenading"))
                {
                    if (__instance.GetBool("isSerenading"))
                    {
                        int direction = __instance.GetInt("mariachiDirection");
                        if (Map.IsEnemyUnitNearby(__instance.playerNum, __instance.X, __instance.Y + 6f, direction, 160f, 16f, false, true))
                        {
                            __instance.ForceFaceDirection(direction);
                            __instance.SetFieldValue("gunFightFireTimer", 1.5f);
                            __instance.CallMethod("StopSerenading");
                        }
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
            catch (Exception e)
            {
                Main.ExceptionLog(e);
            }

        }
    }

    // Commented because Animation is skiped and buggy
    /*[HarmonyPatch(typeof(Desperabro), "AnimateRolling")]
    static class AnimateRolling_Patch
    {
        static void Postfix(Desperabro __instance)
        {
            if (Main.CantUsePatch) return;

            try
            {
                if (__instance.mariachiBroType == Desperabro.MariachiBroType.Desperabro)
                {
                    Map.KnockUnits(__instance, DamageType.Knock, 10, 10, __instance.X, __instance.Y, 100, 50, false, true);
                }
            }
            catch (Exception e)
            {
                Main.ExceptionLog(e);
            }
        }
    }*/
}
