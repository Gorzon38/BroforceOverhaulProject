using HarmonyLib;
using System;
using RocketLib;
using UnityEngine;

namespace LeeBroxmasTrained
{
    [HarmonyPatch(typeof(LeeBroxmas))]
    public class LeeBroxmasPatches
    {
        public static TrainedSettings TSettings
        {
            get => Main.settings.mod;
        }
        public static VanillaSettings VSettings
        {
            get => Main.settings.vanilla;
        }

        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        public static void AddCustomComponent(LeeBroxmas __instance)
        {
            __instance.GetOrAddComponent<TrainedLeeBroxmas>();

            __instance.useNewPushingFrames = VSettings.usePushingAnimation;
            __instance.useNewLadderClimbingFrames = VSettings.useLadderClimbingAnimation;

            __instance.originalSpecialAmmo = VSettings.maxAmmo;

            if (TSettings.hasHalo)
            {
                var halo = HeroController.GetHeroPrefab(HeroType.Broffy).halo;
                __instance.halo = UnityEngine.Object.Instantiate(halo, halo.transform.localPosition, Quaternion.identity);
                __instance.halo.transform.parent = __instance.transform;
            }

            TestVanDammeAnim blade = HeroController.GetHeroPrefab(HeroType.Blade);
            if (blade == null)
                return;

            Texture bladeKnifeTex = (blade as Blade).throwingKnife.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            if (bladeKnifeTex != null)
            {
                __instance.projectile.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = bladeKnifeTex;
                __instance.macheteSprayProjectile.gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = bladeKnifeTex; // Don't work
            }
        }

        [HarmonyPatch("AnimatePushing")]
        [HarmonyPostfix]
        private static void FixPushing(LeeBroxmas __instance)
        {
            if (Mod.CantUsePatch)
                return;

            __instance.CallMethod("SetGunPosition", -10f, 0f);
            __instance.gunSprite.transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        [HarmonyPatch("PressSpecial")]
        [HarmonyPostfix]
        private static void ThrowOnKnifeIfNoAmmo(LeeBroxmas __instance)
        {
            if (Mod.CantUsePatch)
                return;
            if (__instance.SpecialAmmo <= 0)
            {
                __instance.CallMethod("UseSpecial");
            }
        }

        // BroBase Patches for melee
        [HarmonyPatch(typeof(BroBase), "StartCustomMelee")]
        [HarmonyPrefix]
        private static bool StartCustomMelee(BroBase __instance)
        {
            if (Mod.CantUsePatch || !__instance.Is<LeeBroxmas>() || Main.settings.mod.useCustomMelee == Its.No)
                return true;

            TrainedLeeBroxmas customMelee = __instance.GetOrAddComponent<TrainedLeeBroxmas>();
            if (customMelee != null)
                customMelee.StartCustomMelee();

            return false;
        }

        [HarmonyPatch(typeof(BroBase), "AnimateCustomMelee")]
        [HarmonyPrefix]
        private static bool AnimateCustomMelee(BroBase __instance)
        {
            if (Mod.CantUsePatch || !__instance.Is<LeeBroxmas>())
                return true;

            TrainedLeeBroxmas customMelee = __instance.GetOrAddComponent<TrainedLeeBroxmas>();
            if (customMelee != null)
                customMelee.AnimateCustomMelee();

            return false;
        }

        [HarmonyPatch(typeof(BroBase), "RunCustomMeleeMovement")]
        [HarmonyPrefix]
        private static bool RunCustomMeleeMovement(BroBase __instance)
        {
            if (Mod.CantUsePatch || !__instance.Is<LeeBroxmas>())
                return true;

            TrainedLeeBroxmas customMelee = __instance.GetOrAddComponent<TrainedLeeBroxmas>();
            if (customMelee != null)
                customMelee.RunCustomMeleeMovement();

            return false;
        }

    }
}
