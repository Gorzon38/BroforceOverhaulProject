using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheGeneralsTraining.Patches.Doodads
{
    [HarmonyPatch(typeof(Impaler), "ImpaleUnitRPC")]
    static class Impaler_ImpaleUnitRPC_Patch
    {
        static bool Prefix(Impaler __instance, TestVanDammeAnim unit)
        {
            if (Mod.CantUsePatch) return true;

            try
            {
                if ( unit.invulnerable ||
                    (unit as Brominator && (unit as Brominator).brominatorMode) ||
                    (unit as BronanTheBrobarian && unit.As<BronanTheBrobarian>().UsingSpecial) ||
                    (unit as BroCeasar && unit.As<BroCeasar>().GetBool("readyForBlast"))
                )
                {
                    __instance.Death();
                    UnityEngine.Object.Destroy(__instance.gameObject);
                    return false;
                }
            }
            catch (Exception e)
            {
                Main.ExceptionLog(e);
            }
            return true;
        }
    }
}
