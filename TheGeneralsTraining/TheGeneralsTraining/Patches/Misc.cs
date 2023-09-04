using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining.Patches
{

    [HarmonyPatch(typeof(TestVanDammeAnim), "InseminateRPC")]
    static class TestVanDammeAnim_InseminateRPC_Patch
    {
        static void ShowFaceHuggerAvatar2(PlayerHUD hud)
        {
            if (!Main.enabled) return;

            hud.showFaceHugger = true;
            hud.faceHugger2.SetSize(Traverse.Create(hud).Field("avatarFacingDirection").GetValue<int>() * hud.faceHugger2.width, hud.faceHugger2.height);
            hud.secondAvatar.SetLowerLeftPixel(new Vector2(hud.faceHugger2.lowerLeftPixel.x, 1f));
            hud.faceHugger2.gameObject.SetActive(true);
        }
        static void Postfix(TestVanDammeAnim __instance)
        {
            if (!Main.enabled || !(__instance as BoondockBro)) return;
            var boondock = __instance as BoondockBro;

            if (boondock == boondock.trailingBro)
            {
                ShowFaceHuggerAvatar2(__instance.player.hud);
            }
            else if (!boondock.isLeadBro)
            {
                __instance.player.hud.HideFaceHugger();
            }

        }
    }
}
