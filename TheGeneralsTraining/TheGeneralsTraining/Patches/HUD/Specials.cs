using HarmonyLib;
using RocketLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining.Patches.HUD.Specials
{
    [HarmonyPatch(typeof(PlayerHUD), "SetGrenadeMaterials", new Type[] { typeof(HeroType) })]
    static class AddTearGasIcon_Patch
    {
        static void Prefix(PlayerHUD __instance, HeroType type)
        {
            if (Main.CanUsePatch && Main.settings.fifthBondSpecial)
            {
                if (type == HeroType.DoubleBroSeven && __instance.doubleBroGrenades.Length < 5)
                {
                    List<Material> tempList = __instance.doubleBroGrenades.ToList();
                    tempList.Add(ResourcesController.GetMaterial("sharedtextures:GrenadeTearGas"));
                    __instance.doubleBroGrenades = tempList.ToArray();
                }
            }
        }
        static void Postfix(PlayerHUD __instance, HeroType type)
        {
            if (type == HeroType.CaseyBroback)
            {
                Texture2D texture = ResourcesController.GetTexture("pigGrenade.png");
                for (int i = 0; i < __instance.grenadeIcons.Length; i++)
                {
                    __instance.grenadeIcons[i].GetComponent<Renderer>().material.mainTexture = texture;
                }
            }
        }
    }

    // Multiple pocketed special icon on huds
    [HarmonyPatch(typeof(BroBase), "SetPlayerHUDAmmo")]
    static class BroBase_MultiplePocketedSpecial_Patch
    {
        static bool Prefix(BroBase __instance)
        {
            if (Main.CantUsePatch || !Main.settings.multiplePockettedSpecial) return true;

            try
            {
                if (__instance.player == null)
                    return false;

                PlayerHUD hud = __instance.player.hud;

                if (__instance.pockettedSpecialAmmo.Count > 0)
                {
                    int pocketedSpecialStartIndex = 0;
                    if (__instance.pockettedSpecialAmmo.Count > 6)
                    {
                        pocketedSpecialStartIndex = __instance.pockettedSpecialAmmo.Count - 6;
                    }
                    List<Material> materials = new List<Material>();
                    for (int i = pocketedSpecialStartIndex; i < __instance.pockettedSpecialAmmo.Count; i++)
                    {
                        if (i < __instance.pockettedSpecialAmmo.Count)
                        {
                            materials.Add(hud.GetPockettedMaterial(__instance.pockettedSpecialAmmo[i]));
                        }
                    }
                    hud.SetGrenadesMaterials(materials.ToArray());
                }
                else
                {
                    hud.SetGrenadeMaterials(__instance.heroType);
                    hud.SetGrenades(__instance.SpecialAmmo);
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
