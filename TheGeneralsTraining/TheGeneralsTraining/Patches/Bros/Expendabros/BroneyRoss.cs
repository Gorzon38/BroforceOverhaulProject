using System;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace TheGeneralsTraining.Patches.Bros.Expendabros.BroneyRoss0
{
    // Fix attacks sound
    [HarmonyPatch(typeof(BroneyRoss), "Awake")]
    static class BroneyRoss_FixAttackSounds_Patch
    {
        static void Postfix(BroneyRoss __instance)
        {
            if (Main.CanUsePatch)
            {
                try
                {
                    TestVanDammeAnim broHard = HeroController.GetHeroPrefab(HeroType.BroHard);
                    __instance.soundHolder.attackSounds = broHard.soundHolder.attackSounds;

                }
                catch (Exception ex)
                {
                    Main.ExceptionLog("Failed to patch Broney Ross", ex);
                }
            }
        }
    }
    // Throw grenades at feet
    [HarmonyPatch(typeof(BroneyRoss), "UseSpecial")]
    static class UseSpecial_Patch
    {
        static Grenade ThrowGrenade(BroneyRoss broney, int grenadeNumber)
        {
            float xI = 0f;
            float yI = 0f;
            switch (grenadeNumber)
            {
                case 2:
                    xI = 200f;
                    yI = 155f;
                    break;
                case 1:
                    xI = 160f;
                    yI = 130f;
                    break;
                case 0:
                default:
                    xI = 115f;
                    yI = 105f;
                    break;
            }

            if (broney.IsDucking && broney.GetBool("down") && broney.IsOnGround())
            {
                xI *= 0.5f;
                yI *= 0.6f;
            }

            return ProjectileController.SpawnGrenadeOverNetwork(
                broney.specialGrenade,
                broney,
                broney.X + Mathf.Sign(broney.transform.localScale.x) * 8f,
                broney.Y + 8f,
                0.001f,
                0.011f,
                Mathf.Sign(broney.transform.localScale.x) * xI,
                yI,
                broney.playerNum
            );
        }
        static bool Prefix(BroneyRoss __instance)
        {
            return true;
            if (Main.CantUsePatch || __instance.SpecialAmmo < 1) return true;

            try
            {
                __instance.PlayThrowLightSound(0.4f);
                int grenadesThrown = __instance.GetInt("grenadeThrown");
                if (grenadesThrown == 2)
                {
                    __instance.SpecialAmmo--;
                }
                if (__instance.IsMine)
                {
                    Main.Log(grenadesThrown);
                    Grenade grenade = ThrowGrenade(__instance, grenadesThrown);
                    if (grenade != null)
                    {
                        GrenadeSticky component = grenade.GetComponent<GrenadeSticky>();
                        if (component != null)
                        {
                            component.stickGrenadeSwarmIndex = grenadesThrown;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Main.ExceptionLog(ex);
            }
            return true;
        }
    }
}

