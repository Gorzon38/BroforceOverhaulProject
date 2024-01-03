using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using TheGeneralsTraining.Components.Bros;
using RocketLib;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(Xebro))]
    public class XebroPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AddCustomComponent(Xebro __instance)
        {
            if(Main.CanUsePatch && Main.settings.betterChakram)
            {
                __instance.gameObject.GetOrAddComponent<Xena_Comp>();
            }
        }

        [HarmonyPatch("CatchChakram")]
        [HarmonyPrefix]
        public static bool CatchChakramIfAskTo(Xebro __instance, Chakram chakram)
        {
            if (Main.CanUsePatch && Main.settings.betterChakram && __instance.GetComponent<Xena_Comp>() != null)
            {
                Xena_Comp comp = __instance.GetComponent<Xena_Comp>();
                if (comp.hasCallChakram)
                {
                    __instance.meleeType = BroBase.MeleeType.Punch;
                    __instance.SpecialAmmo++;
                    List<Chakram> list = __instance.GetFieldValue<List<Chakram>>("thrownChakram");
                    list.Remove(chakram);
                    __instance.SetFieldValue("thrownChakram", list);

                }
                return false;
            }
            return true;
        }

        [HarmonyPatch("UseSpecial")]
        [HarmonyPrefix]
        public static void CallBackChakram(Xebro __instance)
        {
            if (Main.CanUsePatch && Main.settings.betterChakram)
            {
                Xena_Comp comp = __instance.GetComponent<Xena_Comp>();
                if (comp != null)
                    comp.hasCallChakram = __instance.SpecialAmmo <= 0;
            }
        }
    }
}

