using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(TestVanDammeAnim))]
    public class BroHardPatches
    {
        public static float enclosedSpaceSpeedMultiplier = 1.1f;
        public static float barbedWireSpeedMultiplier = 0.5f;

        [HarmonyPatch("GetSpeed", MethodType.Getter)]
        [HarmonyPrefix]
        private static bool FasterInEnclosedSpace(TestVanDammeAnim __instance, ref float __result)
        {
            // If the current bro is Bro Hard only
            if(Main.CanUsePatch && __instance is BroHard && Main.settings.broHardFasterWhenDucking)
            {
                // From method 'TestVanDammeAnim.get_GetSpeed'
                if (__instance.player == null)
                {
                    __result = __instance.speed;
                }
                // This is from the scrapped project Rogueforce. I kept it in case a mod or an update to enable Rogueforce appear one day.
                else
                {
                    __result = __instance.player.ValueOrchestrator.GetModifiedFloatValue(Rogueforce.ValueOrchestrator.ModifiableType.MovementSpeed, __instance.speed);
                }

                if (__instance.CallMethod<bool>("IsSurroundedByBarbedWire"))
                    __result *= barbedWireSpeedMultiplier;
                // Multiply here
                else if (__instance.IsDucking)
                    __result *= enclosedSpaceSpeedMultiplier;

                return false;
            }
            return true;
        }
    }
}
