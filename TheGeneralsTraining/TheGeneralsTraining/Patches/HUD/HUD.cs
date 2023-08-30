using HarmonyLib;
using RocketLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining.Patches.HUD
{
    [HarmonyPatch(typeof(PlayerHUD), "ShowFaceHugger")]
    static class Avatar_FaceHugger_Patch
    {
        static void Prefix(PlayerHUD __instance)
        {
            if (Main.CanUsePatch && Main.settings.faceHugger)
            {
                __instance.showFaceHugger = true;
                __instance.faceHugger1.SetSize(__instance.GetFieldValue<int>("avatarFacingDirection") * __instance.faceHugger1.width, __instance.faceHugger1.height);
                __instance.avatar.SetLowerLeftPixel(new Vector2(__instance.faceHugger1.lowerLeftPixel.x, 1f));
                __instance.faceHugger1.gameObject.SetActive(true);
            }
        }
    }
    [HarmonyPatch(typeof(PlayerHUD), "Awake")]
    static class Awake_Patch
    {
        static void Postfix(PlayerHUD __instance)
        {
            if (Main.CanUsePatch && Main.settings.fifthBondSpecial)
            {
                List<Material> tempList = __instance.doubleBroGrenades.ToList();
                tempList.Add(ResourcesController.GetMaterial("sharedtextures:GrenadeTearGas"));
                __instance.doubleBroGrenades = tempList.ToArray();
            }
        }
    }
    [HarmonyPatch(typeof(PlayerHUD), "Start")]
    static class Start_Patch
    {
        static void Postfix(PlayerHUD __instance)
        {
            if (Main.CantUsePatch || !Main.settings.faceHugger) return;

            var shader = __instance.avatar.MeshRenderer.material.shader;
            if (shader != null)
            {
                __instance.faceHugger1.meshRender.material.shader = shader;
                __instance.faceHugger1.meshRender.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));

                __instance.faceHugger2.meshRender.material.shader = shader;
                __instance.faceHugger2.meshRender.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, 0.5f));
            }
        }
    }

    [HarmonyPatch(typeof(PlayerHUD), "SetupIcons")]
    static class SetupsIcons_Patch
    {
        static float spacingMultiplier = 15f;
        static void Postfix(PlayerHUD __instance, ref SpriteSM[] icons, ref int direction, ref bool doubleAvatar)
        {
            if (Main.enabled)
            {
                try
                {
                    for (int i = 0; i < icons.Length; i++)
                    {
                        icons[i].transform.localPosition = new Vector3(((doubleAvatar ? direction * 6 : 0) + direction * spacingMultiplier * i + direction * 18), -0.1f, 2f);
                    }
                }
                catch (Exception ex)
                {
                    Main.ExceptionLog(ex);
                }
            }
        }
    }


}
