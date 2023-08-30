using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Mooks
{
    [HarmonyPatch(typeof(MookArmouredGuy), "ActuallyPilot")]
    static class ActuallyPilot_Mech_Patch
    {
        static void Postfix(MookArmouredGuy __instance, Unit PilotUnit)
        {
            if (Main.CantUsePatch || !Main.settings.mechSwapToAmerica) return;

            if (PilotUnit != null && PilotUnit.playerNum >= 0)
            {
                Material sharedMaterial = __instance.americaOriginalMaterial;
                __instance.gameObject.GetComponent<Renderer>().sharedMaterial = sharedMaterial;
                __instance.SetFieldValue("originalMaterial", sharedMaterial);
                __instance.hurtMaterial = __instance.americaHurtMaterial;
            }
        }
    }
}
