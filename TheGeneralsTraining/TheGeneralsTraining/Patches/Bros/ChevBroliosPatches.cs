using HarmonyLib;
using TheGeneralsTraining.Components.Bros;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(ChevBrolios))]
    public class ChevBroliosPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void AddCustomComponent(ChevBrolios __instance)
        {
            if(Main.CanUsePatch && Main.settings.carBattery)
            {
                __instance.gameObject.AddComponent<ChevBrolios_Comp>();
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void BurnBlocksAroundIfPoisoned(ChevBrolios __instance)
        {
            if (Main.CanUsePatch && Main.settings.carBattery)
            {
                ChevBrolios_Comp c = __instance.GetComponent<ChevBrolios_Comp>();
                if (c!= null && c.IsOnFire() && __instance.IsAlive() && __instance.GetFieldValue<bool>("isPoisoned") )
                {
                    Map.BurnBlocksAround(0, __instance.collumn, __instance.row, true);
                }
            }
        }

        [HarmonyPatch("UseSpecial")]
        [HarmonyPrefix]
        private static void CallComponentOnSpecial(ChevBrolios __instance)
        {
            if (Main.CanUsePatch && Main.settings.carBattery)
            {
                ChevBrolios_Comp c = __instance.GetComponent<ChevBrolios_Comp>();
                if (c != null && __instance.SpecialAmmo > 0 && __instance.GetFieldValue<bool>("isPoisoned"))
                {
                    c.UseSpecial();
                    __instance.SpecialAmmo--;
                }
            }
        }

        [HarmonyPatch("UseFire")]
        [HarmonyPostfix]
        private static void NoRecoil(ChevBrolios __instance)
        {
            if (Main.CanUsePatch && Main.settings.noRecoil)
            {
                __instance.xIBlast = 0;
            }
        }

    }
}
