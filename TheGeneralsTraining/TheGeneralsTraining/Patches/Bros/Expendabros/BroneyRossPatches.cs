using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros.Expendabros
{
    [HarmonyPatch(typeof(BroneyRoss))]
    public class BroneyRossPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void FixGunsSounds(BroneyRoss __instance)
        {
            if (Main.CanUsePatch)
            {
                // Replace the current shooting sound by the one from Bro Hard
                TestVanDammeAnim broHard = HeroController.GetHeroPrefab(HeroType.BroHard);
                if (broHard != null)
                    __instance.soundHolder.attackSounds = broHard.soundHolder.attackSounds;
            }
        }
    }
}

