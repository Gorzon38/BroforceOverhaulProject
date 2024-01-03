
using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros.Expendabros
{
    [HarmonyPatch(typeof(TrentBroser))]
    public class TrentBroserPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void ChangeSoundAndSpecialGrenade(TrentBroser __instance)
        {
            if (Main.CanUsePatch)
            {
                // Change the silenced sound to a normal one
                TestVanDammeAnim broDredd = HeroController.GetHeroPrefab(HeroType.BroDredd);
                if (broDredd != null)
                    __instance.soundHolder.attackSounds = broDredd.soundHolder.attackSounds;

                // Give the special greande of Brodell Wlaker to Trent Broser
                TestVanDammeAnim brodellWalker = HeroController.GetHeroPrefab(HeroType.BrodellWalker);
                if (brodellWalker != null)
                    __instance.specialGrenade = brodellWalker.specialGrenade;
            }
        }
    }
}

