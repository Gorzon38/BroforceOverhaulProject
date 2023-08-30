using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerroristC4Programs.Components;
using TerroristC4Programs.Extensions;
using UnityEngine;

namespace TerroristC4Programs.PatchesGlobal
{
    [HarmonyPatch(typeof(Mook), "Awake")]
    static class Awake_Patch
    {
        static void Postfix(Mook __instance)
        {
            __instance.gameObject.AddComponent<MookExtended>();
        }
    }

    [HarmonyPatch(typeof(TestVanDammeAnim), "CalculateZombieInput")]
    static class MookDanceOnBroFlex_Patch
    {
        public static float dancingTime = 0.1f;
        static void Postfix(TestVanDammeAnim __instance)
        {
            if (Mod.CanUsePatch && Mod.Sett.zombiesDanceOnFlex && __instance as Mook)
            {
                var reviveSource = __instance.GetFieldValue<TestVanDammeAnim>("reviveSource");
                if (reviveSource == null) return;

                if (reviveSource.IsGesturing())
                    __instance.Dance(dancingTime);
                else
                    __instance.Dance(0f);
            }
        }
    }

    [HarmonyPatch(typeof(TestVanDammeAnim), "ReplaceWithSkinnedInstance")]
    static class BetterSkinlessSprites_Patch
    {
        static void Prefix(TestVanDammeAnim __instance, ref Unit skinnedInstance)
        {
            if(Mod.CantUsePatch && !Mod.Sett.betterSkinlessSprite) return;

            var tex = TextureManager.GetTexture($"{__instance.GetType()}_skinless.png");
            if (tex == null)
            {
                string name = __instance.As<Mook>().GetSkinnedName();
                if (name.IsNotNullOrEmpty())
                    tex = TextureManager.GetTexture(name);
            }
            if (tex != null)
                skinnedInstance.GetComponent<SpriteSM>().SetTexture(tex);
        }
    }

    [HarmonyPatch(typeof(Unit), "IsHeavy")]
    static class Unit_IsHeavy_Patch
    {
        static bool Prefix(Unit __instance, ref bool __result)
        {
            if(Mod.CanUsePatch && __instance is MookGrenadier)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Unit), "HeadShot")]
    static class Unit_HeadShot_Patch
    {
        static bool Prefix(Unit __instance, int damage, DamageType damageType, float xI, float yI, int direction, float xHit, float yHit, MonoBehaviour damageSender)
        {
            if (Mod.CantUsePatch) return true;

            var mook = __instance as Mook;
            if (mook == null) return true;
            if (mook.As<SkinnedMook>() || mook.As<MookTrooper>() || mook.As<MookSuicide>() || mook.As<MookGrenadier>() || mook.As<MookBigGuy>() || mook.As<MookDog>()) return true;

            try
            {
                if (mook.GetBool("decapitated"))
                {
                    mook.Damage(damage, damageType, xI, yI, direction, damageSender, xHit, yHit);
                }
                else if ((damageType == DamageType.Bullet || damageType == DamageType.SilencedBullet || damageType == DamageType.Melee || damageType == DamageType.Knifed || damageType == DamageType.Normal || damageType == DamageType.Blade) && mook.health > 0 && damage * 3 >= mook.health)
                {
                    mook.Decapitate(damage, damageType, xI, yI, direction, xHit, yHit, damageSender);
                }
                else
                {
                    mook.Damage(damage, damageType, xI, yI, direction, damageSender, xHit, yHit);
                }
                return false;
            }
            catch (Exception e)
            {
                Main.Log(e);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Mook), "Update")]
    static class Mook_Update_Patch
    {
        static void Postfix(Mook __instance)
        {
            if (Mod.CantUsePatch) return;

            if (__instance.As<SkinnedMook>() || __instance.As<MookTrooper>() || __instance.As<MookSuicide>() || __instance.As<MookGrenadier>() ||
                __instance.As<MookBigGuy>() || __instance.As<MookDog>() || !__instance.GetBool("decapitated") || !__instance.IsAlive()
            ) return;


            float decapitationCounter = __instance.GetFloat("decapitationCounter");
            decapitationCounter -= Time.deltaTime;
            if (decapitationCounter <= 0f)
            {
                __instance.Y += 2f;
                __instance.Damage(__instance.health + 1, DamageType.Bullet, 0f, 12f, (int)Mathf.Sign(-__instance.transform.localScale.x), __instance, __instance.X, __instance.Y + 8f);
            }
            __instance.SetFieldValue("decapitationCounter", decapitationCounter);
        }
    }
}
