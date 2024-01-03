using System.Collections.Generic;
using HarmonyLib;
using RocketLib;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(BrondleFly))]
    public class SethBrondlePatches
    {
        public static float range = 80f;
        public static float openSpotRange = 32f;
        public static int hangingRow = 9;
        public static float flyingSpeedMultiplier = 0.97f;

        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AddCustomComponent(BrondleFly __instance)
        {
            if (Main.CanUsePatch && Main.settings.betterTeleportation)
                __instance.gameObject.GetOrAddComponent<SethBrondle_Comp>();
        }

        [HarmonyPatch(typeof(TestVanDammeAnim), "CoverInAcidRPC")]
        [HarmonyPrefix]
        private static bool SacrificeAmmoToAvoidBeingCoverInAcid(TestVanDammeAnim __instance)
        {
            if (!Main.CanUsePatch || !Main.settings.noAcidCoverIfSpecial)
                return true;

            if (__instance is BrondleFly && __instance.SpecialAmmo > 0)
            {
                // If 'betterTeleportation' is enable do the better teleportation
                if (Main.settings.betterTeleportation && __instance.GetComponent<SethBrondle_Comp>() != null)
                {
                    __instance.GetComponent<SethBrondle_Comp>().Teleport(Vector3.zero, null);
                    __instance.SpecialAmmo--;
                    return false;
                }

                // Original teleportation
                BrondleFly bro = __instance as BrondleFly;
                SpriteSM spriteSM = UnityEngine.Object.Instantiate<SpriteSM>(bro.teleportOutAnimation);
                spriteSM.transform.position = bro.transform.position;

                SpriteSM spriteSM2 = UnityEngine.Object.Instantiate<SpriteSM>(bro.teleportInAnimation);
                spriteSM2.transform.parent = bro.transform;
                spriteSM2.transform.localPosition = -Vector3.forward;

                __instance.SpecialAmmo--;
                return false;
            }
            return true;
        }

        [HarmonyPatch("TeleFrag")]
        [HarmonyPrefix]
        public static bool TeleFragWithBetterTeleportation(BrondleFly __instance)
        {
            if (Main.CantUsePatch || !Main.settings.betterTeleportation)
                return true;

            __instance.SetFieldValue("teleportPos", __instance.transform.position);
            __instance.SetFieldValue("teleportCamLerp", 0f);

            Vector3 direction = (__instance.left ? Vector3.left : Vector3.zero) +
                (__instance.right ? Vector3.right : Vector3.zero) +
                (__instance.up ? Vector3.up : Vector3.zero) +
                (__instance.down ? Vector3.down : Vector3.zero);

            if (direction != Vector3.zero)
            {
                direction.Normalize();
            }
            else
            {
                direction = new Vector3(__instance.transform.localScale.x, 0f, 0f);
            }

            Vector3 vector = __instance.transform.position + direction * range;

            List<Unit> unitsInRange = Map.GetUnitsInRange(range, range, __instance.X, __instance.Y, false);
            float distance = 50f;
            Unit closestUnit = null;
            foreach (Unit unit in unitsInRange)
            {
                if (unit.IsEnemy && !(unit is Tank))
                {
                    Vector3 from = unit.transform.position - __instance.transform.position;
                    float distance2 = Vector3.Angle(from, direction);
                    if (distance2 < distance)
                    {
                        distance = distance2;
                        closestUnit = unit;
                    }
                }
            }

            bool hasFoundTeleportSpot = false;
            if (closestUnit == null)
            {
                hasFoundTeleportSpot = Utilities.SearchForOpenSpot(ref vector, direction);

                unitsInRange = Map.GetUnitsInRange(openSpotRange, openSpotRange, vector.x, vector.y, false);
                if (unitsInRange.Count > 0)
                {
                    foreach (Unit unit in unitsInRange)
                    {
                        if (unit.IsEnemy && !(unit as MookArmouredGuy))
                        {
                            closestUnit = unit;
                            hasFoundTeleportSpot = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                hasFoundTeleportSpot = true;
            }

            if (closestUnit != null)
            {
                vector = closestUnit.transform.position;
            }
            else if (Map.IsBlockSolid(vector.x, vector.y))
            {
                vector = Vector3.zero;
            }

            __instance.xI = 0f;
            if (__instance.up)
            {
                __instance.SetFieldValue("jumpTime", 0f);
                __instance.yI = 120f;
            }
            else
            {
                __instance.yI = 0f;
            }

            __instance.DisConnectFaceHugger();

            // - Did the Job

            var comp = __instance.GetComponent<SethBrondle_Comp>();
            if (comp != null)
            {
                comp.Teleport(vector, closestUnit);
            }

            __instance.SetFieldValue("usingSpecial", true);

            return false;
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool IgnoreUpdateIfIsTeleportingWithBetterTeleportation(BrondleFly __instance)
        {
            if (!Main.CanUsePatch || !Main.settings.betterTeleportation)
                return true;

            var comp = __instance.GetComponent<SethBrondle_Comp>();

            return comp != null && comp.isTeleporting;
        }

        [HarmonyPatch("AnimateHanging")]
        [HarmonyPrefix]
        private static bool AlternateCeilingHangingAnimation(BrondleFly __instance)
        {
            if (Main.CantUsePatch || !Main.settings.alternateHangingAnimation)
                return true;

            if (__instance.GetBool("chimneyFlip"))
            {
                return true;
            }

            __instance.CallMethod("SetSpriteOffset", 0f, -2f);
            if (__instance.right || __instance.left)
            {
                __instance.CallMethod("DeactivateGun");
                __instance.SetFieldValue("frameRate", 0.0667f);
                int num = 11 + __instance.frame % 12;
                __instance.SetSpriteLowerLeftPixel(num, hangingRow);
                __instance.SetFieldValue("wasHangingMoving", true);
                __instance.CallMethod("SetGunSprite", 0, 1);
            }
            else
            {
                __instance.SetFieldValue("hangingOneArmed", true);
                if (__instance.GetBool("wasHangingMoving"))
                {
                    __instance.frame = 0;
                    __instance.SetFieldValue("wasHangingMoving", false);
                }
                __instance.SetFieldValue("frameRate", 0.045f);
                int num2 = 23 + Mathf.Clamp(__instance.frame, 0, 5);
                __instance.SetSpriteLowerLeftPixel(num2, hangingRow);
            }
            __instance.CallMethod("DeactivateGun");

            return false;
        }

        [HarmonyPatch("AddSpeedLeft")]
        [HarmonyPrefix]
        private static bool FlyFasterToLeft(BrondleFly __instance)
        {
            if (Main.CantUsePatch || !Main.settings.flyFaster)
                return true;

            if (__instance.health > 0 && __instance.actionState == ActionState.Jumping && __instance.GetFloat("hoverTime") > 0f)
            {
                __instance.xI -= __instance.speed * __instance.GetFloat("t");
                __instance.xI *= flyingSpeedMultiplier;
                return false;
            }
            return true;
        }

        [HarmonyPatch("AddSpeedRight")]
        [HarmonyPrefix]
        private static bool FlyFasterToRight(BrondleFly __instance)
        {
            if (Main.CantUsePatch || !Main.settings.flyFaster)
                return true;

            if (__instance.health > 0 && __instance.actionState == ActionState.Jumping && __instance.GetFloat("hoverTime") > 0f)
            {
                __instance.xI += __instance.speed * __instance.GetFloat("t");
                __instance.xI *= flyingSpeedMultiplier;
                return false;
            }
            return true;
        }
    }
}
