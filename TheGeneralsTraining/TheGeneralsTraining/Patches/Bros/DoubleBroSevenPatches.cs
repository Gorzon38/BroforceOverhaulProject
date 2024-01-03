using HarmonyLib;
using RocketLib;
using TheGeneralsTraining.Components.Bros;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Bros
{
    // Add the Fifth Special
    [HarmonyPatch(typeof(DoubleBroSeven), "Awake")]
    public class DoubleBroSevenPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        private static void AddTheTearGas(DoubleBroSeven __instance)
        {
            if (Main.CantUsePatch)
                return;

            if (Main.settings.fifthBondSpecial)
                __instance.originalSpecialAmmo = 5;

            __instance.GetOrAddComponent<DoubleBroSeven_Comp>();
        }
        [HarmonyPatch("UseSpecial")]
        [HarmonyPrefix]
        public static bool ThrowTearGasAtFeet(DoubleBroSeven __instance)
        {
            if (Mod.CantUsePatch || !Main.settings.fifthBondSpecial)
                return true;

            // From Method 'DoubleBroSeven.UseSpecial()'

            Traverse t = __instance.GetTraverse();
            if (t == null)
                return true;

            DoubleBroSevenSpecialType currentSpecialType = t.GetFieldValue<DoubleBroSevenSpecialType>("currentSpecialType");
            if (currentSpecialType == DoubleBroSevenSpecialType.TearGas)
            {
                Networking.Networking.RPC(PID.TargetAll, new RpcSignature<float>(__instance.PlayThrowLightSound), 0.5f, false);
                if (__instance.IsMine)
                {
                    if (__instance.IsDucking && __instance.down)
                    {
                        // Trow grenade at feet
                        ProjectileController.SpawnGrenadeOverNetwork(__instance.tearGasGrenade, __instance, __instance.X + Mathf.Sign(__instance.transform.localScale.x) * 6f, __instance.Y + 3f, 0.001f, 0.011f, Mathf.Sign(__instance.transform.localScale.x) * 30f, 70f, __instance.playerNum);
                    }
                    else
                    {
                        ProjectileController.SpawnGrenadeOverNetwork(__instance.tearGasGrenade, __instance, __instance.X + Mathf.Sign(__instance.transform.localScale.x) * 6f, __instance.Y + 10f, 0.001f, 0.011f, Mathf.Sign(__instance.transform.localScale.x) * 200f, 150f, __instance.playerNum);
                    }
                }

                __instance.SpecialAmmo--;
                return false;
            }
            return true;
        }

        [HarmonyPatch("UseSpecial")]
        [HarmonyPostfix]
        public static void PutBalaclavaOnAvatar(DoubleBroSeven __instance)
        {
            if (Main.CantUsePatch)
                return;

            DoubleBroSevenSpecialType currentSpecialType = __instance.GetFieldValue<DoubleBroSevenSpecialType>("currentSpecialType");
            if (currentSpecialType == DoubleBroSevenSpecialType.Balaclava)
            {
                var comp = __instance.GetComponent<DoubleBroSeven_Comp>();
                if (comp != null)
                {
                    comp.SetBalaclava(__instance.player.hud);
                }
            }
        }

        [HarmonyPatch("FireWeapon")]
        [HarmonyPrefix]
        private static bool DrunkShooting(DoubleBroSeven __instance, float x, float y, float xSpeed, float ySpeed)
        {
            if (Main.CanUsePatch && Main.settings.lessAccurateIfDrunk && __instance.GetFieldValue<int>("martinisDrunk") > 2)
            {
                // From methods 'DoubleBroSeven.UseFire()' and 'TestVanDammeAnim.FireWeapon(float,float,float,float)'

                int randomY = UnityEngine.Random.Range(-25, 25);
                __instance.gunSprite.SetLowerLeftPixel((float)(32 * 3), 32f);
                EffectsController.CreateMuzzleFlashEffect(x, y, -25f, xSpeed * 0.01f, ySpeed * 0.01f, __instance.transform);
                ProjectileController.SpawnProjectileLocally(__instance.projectile, __instance, x, y, xSpeed, ySpeed + randomY, __instance.playerNum);
                return false;
            }
            return true;
        }

        [HarmonyPatch("StopUsingSpecialRPC")]
        [HarmonyPrefix]
        public static void RemoveBalaclavaFromAvatar(DoubleBroSeven __instance)
        {
            if (Main.CantUsePatch)
                return;

            DoubleBroSevenSpecialType currentSpecialType = __instance.GetFieldValue<DoubleBroSevenSpecialType>("currentSpecialType");
            if (currentSpecialType == DoubleBroSevenSpecialType.BalaclavaRemoval)
            {
                var comp = __instance.GetComponent<DoubleBroSeven_Comp>();
                if (comp != null)
                {
                    comp.RemoveBalaclava(__instance.player.hud);
                }
            }
        }

    }
}

