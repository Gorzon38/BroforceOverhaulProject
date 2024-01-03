using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using RocketLib;
using TheGeneralsTraining.Components;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(Broffy))]
    public class BroffyPatches
    {
        public static AudioClip[] kickClips;
        // Key: Collumn | Value: Row
        public static KeyValuePair<int, int> kickAnimationFirstFrame = new KeyValuePair<int, int>(6, 17);

        [HarmonyPatch(typeof(BroBase), "Awake")]
        [HarmonyPrefix]
        private static void BroffyAwake(BroBase __instance)
        {
            if (Mod.CantUsePatch)
                return;

            if (__instance is Broffy)
            {
                __instance.GetOrAddComponent<Buffy_Comp>();
                //Store Nebro punch sound, to replace Broffy kick sound
                if (kickClips == null)
                    kickClips = HeroController.GetHeroPrefab(HeroType.Nebro).soundHolder.special2Sounds;
            }
        }

        [HarmonyPatch(typeof(BroBase), "CancelMelee")]
        [HarmonyPostfix]
        private static void StopFlyingKick(BroBase __instance)
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

        [HarmonyPatch("PerformKnifeMeleeAttack")]
        [HarmonyPrefix]
        private static bool NewPerformKnifeMeleeAttack(Broffy __instance, bool shouldTryHitTerrain, bool playMissSound)
        {
            if (Main.CantUsePatch || !Main.settings.betterKick)
                return true;

            // From method 'Broffy.PerformKnifeMeleeAttack(bool,bool)'

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
                // Change the hit sound if Broffy is kicking an opponent
                if (damageType == DamageType.Melee)
                    sound.PlaySoundEffectAt(kickClips, 1f, __instance.transform.position);
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

        [HarmonyPatch("AnimateKnifeMelee")]
        [HarmonyPrefix]
        private static bool BetterMelee(Broffy __instance)
        {
            if (Mod.CantUsePatch)
                return true;

            // From method 'Broffy.AnimateKnifeMelee()'

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
                // If jumping and melee, do flying kick
                frameCol = Buffy_Comp.flyingKickCol + Mathf.Clamp(__instance.frame, 0, Buffy_Comp.flyingKickMaxFrame);
                frameRow = Buffy_Comp.flyingKickRow;
                comp.StartFlyingKick();
            }
            // dashingMelee is set to true if the player goes left or right
            else if (dashingMelee)
            {
                // If player is running do Flying Kick, else normal kick
                if (__instance.dashing)
                {
                    frameCol = Buffy_Comp.flyingKickCol + Mathf.Clamp(__instance.frame, 0, Buffy_Comp.flyingKickMaxFrame);
                    frameRow = Buffy_Comp.flyingKickRow;
                    comp.StartFlyingKick();
                }
                else
                {
                    frameCol = kickAnimationFirstFrame.Value + Mathf.Clamp(__instance.frame, 0, 9);
                    frameRow = kickAnimationFirstFrame.Key;
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
