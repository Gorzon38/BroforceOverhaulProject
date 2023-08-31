using HarmonyLib;
using RocketLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TheGeneralsTraining.Components;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Bros.Buffy
{
    [HarmonyPatch(typeof(BroBase), "Awake")]
    static class BroBase_Awake_Patch
    {
        static void Prefix(BroBase __instance)
        {
            if (Main.CantUsePatch) return;

            if (__instance is Broffy)
            {
                __instance.soundHolder = HeroController.GetHeroPrefab(HeroType.Nebro).soundHolder;
                __instance.GetOrAddComponent<Buffy_Comp>();
            }
        }
    }

    [HarmonyPatch(typeof(BroBase), "CancelMelee")]
    static class BorBase_CancelMelee_Patch
    {
        static void Postfix(BroBase __instance)
        {
            if (Mod.CanUsePatch)
            {
                var comp = __instance.GetComponent<Buffy_Comp>();
                if (comp != null)
                {
                    comp.StopFlyingKick();
                }
            }
        }
    }

    [HarmonyPatch(typeof(Broffy), "PerformKnifeMeleeAttack")]
    static class Broffy_PerformKnifeMeleeAttack_Patch
    {
        static bool Prefix(Broffy __instance, bool shouldTryHitTerrain, bool playMissSound)
        {
            if (Main.CantUsePatch || !Main.settings.betterKick) return true;

            Traverse t = __instance.GetTraverse();
            Sound sound = t.GetFieldValue<Sound>("sound");

            DamageType damageType = t.GetFieldValue<bool>("dashingMelee") ? DamageType.Melee : DamageType.SilencedBullet;
            if (Utilities.IsOnAnimal(__instance))
                damageType = DamageType.Knifed;
            Map.DamageDoodads(3, DamageType.Knifed, __instance.X + (float)(__instance.Direction * 4), __instance.Y, 0f, 0f, 6f, __instance.playerNum, out bool flag, __instance);
            t.CallMethod("KickDoors", 24f);
            bool knock = false;
            float xI = 0f;
            float yI = 0f;
            int damage = 4;
            if (damageType == DamageType.Melee)
            {
                xI = (__instance.Direction * 700f);
                yI = 400f;
                damage = 6;
            }
            if (damageType == DamageType.Knifed)
            {
                knock = true;
                xI = (__instance.Direction * 200f);
                yI = 700f;
                damage = 2;
            }
            if (Map.HitClosestUnit(__instance, __instance.playerNum, damage, damageType, 14f, 24f, __instance.X + __instance.transform.localScale.x * 8f, __instance.Y + 8f, xI, yI, knock, false, __instance.IsMine, false, (damageType == DamageType.Knifed || Buffy_Comp.doingFlyingKick)))
            {
                if (damageType == DamageType.Melee)
                    sound.PlaySoundEffectAt(__instance.soundHolder.special2Sounds, 1f, __instance.transform.position);
                else
                    sound.PlaySoundEffectAt(__instance.soundHolder.meleeHitSound, 1f, __instance.transform.position);
                t.SetFieldValue("meleeHasHit", true);
            }
            else if (playMissSound)
            {
                sound.PlaySoundEffectAt(__instance.soundHolder.missSounds, 0.3f, __instance.transform.position);
            }
            t.SetFieldValue<Unit>("meleeChosenUnit", null);
            if (shouldTryHitTerrain && t.Method("TryMeleeTerrain", 0, 2).GetValue<bool>())
            {
                t.SetFieldValue("meleeHasHit", true);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(Broffy), "AnimateKnifeMelee")]
    static class AnimateKnifeMelee_Patch
    {
        static TwoInt kick = new TwoInt(6, 17);

        static bool Prefix(Broffy __instance)
        {
            if (Mod.CantUsePatch) return true;

            __instance.CallMethod("AnimateMeleeCommon");
            bool dashingMelee = __instance.GetBool("dashingMelee");
            int frameCol = 25 + Mathf.Clamp(__instance.frame, 0, 6);
            int frameRow = 7;

            var comp = __instance.GetComponent<Buffy_Comp>();
            if (__instance.GetBool("standingMelee") && Utilities.IsOnAnimal(__instance))
            {
                frameRow = 1;
            }
            else if (__instance.GetBool("jumpingMelee"))
            {
                frameCol = Buffy_Comp.flyingKickCol + Mathf.Clamp(__instance.frame, 0, Buffy_Comp.flyingKickMaxFrame);
                frameRow = Buffy_Comp.flyingKickRow;
                comp.StartFlyingKick();
            }
            else if (dashingMelee)
            {
                if (__instance.dashing)
                {
                    frameCol = Buffy_Comp.flyingKickCol + Mathf.Clamp(__instance.frame, 0, Buffy_Comp.flyingKickMaxFrame);
                    frameRow = Buffy_Comp.flyingKickRow;
                    comp.StartFlyingKick();
                }
                else
                {
                    frameCol = kick.y + Mathf.Clamp(__instance.frame, 0, 9);
                    frameRow = kick.x;
                    if (__instance.frame == 4)
                    {
                        __instance.counter -= 0.0334f;
                    }
                    else if (__instance.frame == 5)
                    {
                        __instance.counter -= 0.0334f;
                    }
                }
            }

            __instance.SetSpriteLowerLeftPixel(frameCol, frameRow);

            int hitFrame = dashingMelee ? 5 : 3;
            if (__instance.frame == hitFrame || (comp != null && comp.FlyingKickDoesDamage()))
            {
                __instance.counter -= 0.066f;
                __instance.CallMethod("PerformKnifeMeleeAttack", true, true);
            }
            else if (__instance.frame > hitFrame && !__instance.GetBool("meleeHasHit"))
            {
                __instance.CallMethod("PerformKnifeMeleeAttack", false, false);
            }
            int lastFrame = dashingMelee ? 9 : 6;
            if (Buffy_Comp.doingFlyingKick)
                lastFrame = Buffy_Comp.flyingKickMaxFrame;
            if (__instance.frame >= lastFrame)
            {
                __instance.frame = 0;
                comp.StopFlyingKick();
                __instance.CallMethod("CancelMelee");
            }
            return false;
        }
    }
}
