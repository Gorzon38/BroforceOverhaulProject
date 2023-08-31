using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros.Brade
{
   /* [HarmonyPatch(typeof(Blade), "Awake")]
    static class Blade_OldGlaive_Patch
    {
        static void Postfix(Blade __instance)
        {
            if (Main.CanUsePatch && Main.settings.bradeGlaive && __instance is Blade)
            {
                try
                {
                    if (__instance.throwingKnife == null) return;

                    Texture2D texture = ResourcesController.GetTexture("ThrowingGlaive.png");
                    if(texture)
                    {
                        __instance.throwingKnife.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
                        __instance.throwingKnife.gameObject.AddComponent<Projectiles.RotateProjectile_Comp>();
                    }
                }

                catch (Exception ex)
                {
                    Main.ExceptionLog(ex);
                }
            }
        }
    }*/

    [HarmonyPatch(typeof(Blade), "ThrowKnife")]
    static class ThrowKnife_Patch
    {
        static bool Prefix(Blade __instance)
        {
            if (Main.CantUsePatch || __instance.throwingKnife == null || !(__instance is Blade)) return true;

            if (__instance.throwingKnife == null) return true;


            __instance.SetFieldValue("knifeThrown", true);
            __instance.CallMethod("PlayAttackSound", 0.44f);
            var knife = ProjectileController.SpawnProjectileLocally(__instance.throwingKnife, __instance, __instance.X + (float)(16 * __instance.Direction), __instance.Y + 10f, __instance.xI + (float)(250 * __instance.Direction), 0f, __instance.playerNum);
            Texture2D texture = ResourcesController.GetTexture("ThrowingGlaive.png");
            if (texture != null)
            {
                knife.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
                knife.gameObject.AddComponent<Projectiles.RotateProjectile_Comp>();
            }
            return false;
        }
    }
}
