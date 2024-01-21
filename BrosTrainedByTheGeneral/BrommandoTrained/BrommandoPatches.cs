using HarmonyLib;
using RocketLib;
using UnityEngine;
using World.Generation.MapGenV4;

namespace BrommandoTrained
{
    [HarmonyPatch(typeof(Brommando))]
    public class BrommandoPatches
    {
        public static TrainedSettings TSettings
        {
            get => Main.settings.mod;
        }
        public static VanillaSettings VSettings
        {
            get => Main.settings.vanilla;
        }
        public static int gunShootColumnStartingFrame
        {
            get { return VSettings.gunShootColumnStartingFrame; }
        }
        public static Vector2 projectileSpawnOffset
        {
            get { return VSettings.projectileSpawnOffset; }
        }
        public static Vector2 projectileSpeed
        {
            get { return VSettings.projectileSpeed; }
        }

        public static float fireDelay
        {
            get { return VSettings.fireDelay; }
        }
        public static float fireDelayProcGen
        {
            get { return VSettings.fireDelayProcGen; }
        }
        public static Vector2Int shootAtFeetAnimationPosition
        {
            get { return TSettings.shootAtFeetAnimationPosition.ToVector2Int(); }
        }

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AddCustomComponent(Brommando __instance)
        {
            __instance.GetOrAddComponent<TrainedBrommando>();

            __instance.useNewPushingFrames = VSettings.usePushingAnimation;
            __instance.useNewLadderClimbingFrames = VSettings.useLadderClimbingAnimation;

            __instance.originalSpecialAmmo = VSettings.maxAmmo;

            DrunkRocket drunkRocket = __instance.barageProjectile as DrunkRocket;
            if (drunkRocket != null)
                drunkRocket.drunkSpeed = VSettings.rocketDrunkSpeed;

            if (TSettings.hasHalo)
            {
                var halo = HeroController.GetHeroPrefab(HeroType.Broffy).halo;
                __instance.halo = UnityEngine.Object.Instantiate(halo, halo.transform.localPosition, Quaternion.identity);
                __instance.halo.transform.parent = __instance.transform;
            }
        }

        [HarmonyPatch("UseFire")]
        [HarmonyPrefix]
        private static bool ReplaceProjectileSpawnPosition(Brommando __instance)
        {
            if (Mod.CantUsePatch)
                return true;

            if (__instance.IsMine)
            {
                if (TSettings.fireProjectileAtFeetOnDown == Its.Yes && __instance.IsPressingDown())
                {
                    TrainedBrommando rework = __instance.GetOrAddComponent<TrainedBrommando>();
                    if (rework != null)
                        rework.PrepareFeetShoot();
                }
                else
                {
                    __instance.CallMethod("FireWeapon",
                        __instance.X + __instance.transform.localScale.x * projectileSpawnOffset.x,
                        __instance.Y + projectileSpawnOffset.y,
                        __instance.transform.localScale.x * projectileSpeed.x,
                        projectileSpeed.y);
                }
            }

            if (ProcGenGameMode.isEnabled || ProcGenGameMode.ProcGenTestBuild)
            {
                int primaryFireLevel = HeroController.GetPrimaryFireLevel(__instance.playerNum);
                __instance.fireDelay = fireDelayProcGen - 0.12f * (float)primaryFireLevel;
            }
            else
            {
                __instance.fireDelay = fireDelay;
            }

            __instance.CallMethod("PlayAttackSound");
            Map.DisturbWildLife(__instance.X, __instance.Y, VSettings.disturbWildLifeRange, __instance.playerNum);

            return false;
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void NewUpdate(Brommando __instance)
        {
            if (Mod.CantUsePatch)
                return;

            TrainedBrommando rework = __instance.GetComponent<TrainedBrommando>();
            if (rework == null)
                return;

            rework.NewBarrage();

            // Shoot At Feet
            if (!rework.willShootAtHisFeet)
                return;
            int row = shootAtFeetAnimationPosition.x;
            int column = shootAtFeetAnimationPosition.y + Mathf.Clamp(__instance.frame, 0, 8); // 8 is the max of frame for the animation
            if (__instance.frame == 3)
            {
                rework.FireWeaponToFeet();
            }

            __instance.SetSpriteLowerLeftPixel(column, row);
            if (__instance.frame >= 7) // Last frame
            {
                rework.StopFeetShoot();
            }
        }

        [HarmonyPatch("UseSpecial")]
        [HarmonyPrefix]
        private static bool SpawnBarageProjectile(Brommando __instance)
        {
            if (Mod.CantUsePatch)
                return true;

            if (__instance.SpecialAmmo > 0)
            {
                __instance.SpecialAmmo--;
                HeroController.SetSpecialAmmo(__instance.playerNum, __instance.SpecialAmmo);
                if (__instance.IsMine)
                {
                    float specialX = __instance.X + __instance.transform.localScale.x * projectileSpawnOffset.x;
                    float specialY = __instance.Y + projectileSpawnOffset.y;
                    int barageDirection = (int)Mathf.Sign(__instance.transform.localScale.x);

                    ProjectileController.SpawnProjectileOverNetwork(
                        __instance.barageProjectile,
                        __instance,
                        specialX,
                        specialY,
                        (float)(barageDirection * 150f),
                        0f, false, __instance.playerNum, false, false, 0f);

                    __instance.CallMethod("PlayAttackSound");
                    __instance.SetFieldValue("firingBarage", !TSettings.useNewBarage);
                    __instance.SetFieldValue("barageCounter", 0.1333f);
                    __instance.SetFieldValue("barageCount", VSettings.barrageMax);

                    __instance.SetFieldValue("specialX", specialX);
                    __instance.SetFieldValue("specialY", specialY);
                    __instance.SetFieldValue("barageDirection", barageDirection);

                    if (TSettings.useNewBarage)
                    {
                        __instance.GetOrAddComponent<TrainedBrommando>().ShootingNewBarage = true;
                    }
                }
            }
            else
            {
                HeroController.FlashSpecialAmmo(__instance.playerNum);
                __instance.gunSprite.gameObject.SetActive(true);
            }
            __instance.SetFieldValue("pressSpecialFacingDirection", 0);

            return false;
        }

        // BroBase
        [HarmonyPatch(typeof(BroBase), "StartCustomMelee")]
        [HarmonyPrefix]
        private static bool StartCustomMelee(BroBase __instance)
        {
            if (Mod.CantUsePatch || !__instance.Is<Brommando>() || Main.settings.mod.useCustomMelee == Its.No)
                return true;

            TrainedBrommando customMelee = __instance.GetOrAddComponent<TrainedBrommando>();
            if (customMelee != null)
                customMelee.StartCustomMelee();

            return false;
        }

        [HarmonyPatch(typeof(BroBase), "AnimateCustomMelee")]
        [HarmonyPrefix]
        private static bool AnimateCustomMelee(BroBase __instance)
        {
            if (Mod.CantUsePatch || !__instance.Is<Brommando>())
                return true;

            TrainedBrommando customMelee = __instance.GetOrAddComponent<TrainedBrommando>();
            if (customMelee != null)
                customMelee.AnimateCustomMelee();

            return false;
        }

        [HarmonyPatch(typeof(BroBase), "RunCustomMeleeMovement")]
        [HarmonyPrefix]
        private static bool RunCustomMeleeMovement(BroBase __instance)
        {
            if (Mod.CantUsePatch || !__instance.Is<Brommando>())
                return true;

            TrainedBrommando customMelee = __instance.GetOrAddComponent<TrainedBrommando>();
            if (customMelee != null)
                customMelee.RunCustomMeleeMovement();

            return false;
        }

        [HarmonyPatch(typeof(TestVanDammeAnim), "AnimateActualIdleDuckingFrames")]
        [HarmonyPostfix]
        private static void ReplaceGunPositionOnDucking(TestVanDammeAnim __instance)
        {
            if (Mod.CantUsePatch || !__instance.Is<Brommando>())
                return;

            __instance.CallMethod("SetGunPosition", 2f, -2f);
        }
    }
}
