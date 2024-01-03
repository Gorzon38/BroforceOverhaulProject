using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(Brochete))]
    public class BrochetePatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void IsEnablingAlternateSpecialAnimation(Brochete __instance)
        {
            if (Main.CanUsePatch)
            {
                __instance.SetFieldValue("test6Frames", Main.settings.alternateSpecialAnim);
            }
        }

        // TODO : Fix pushing
        [HarmonyPatch("AnimatePushing")]
        [HarmonyPostfix]
        private static void FixPushingAnimation(Brochete __instance)
        {
            /*if (Main.enabled)
            {
                try
                {
                    __instance.gunSprite.transform.localPosition = TFP_Utility.GetBroGunVector3PositionWhilePushing(HeroType.Brochete);
                    float pushingTime = Traverse.Create(__instance).Field("pushingTime").GetValue<float>();
                    if (__instance.fire || pushingTime <= 0)
                    {
                        __instance.gunSprite.transform.localScale = new Vector3(-1f, 1f, 1f);
                        __instance.gunSprite.transform.localPosition = TFP_Utility.GetBroGunVector3PositionWhenFinishPushing(__instance.heroType);
                    }

                }
                catch (Exception ex)
                {
                    Main.bmod.logger.ExceptionLog("Failed to patch Brochete pushing", ex);
                }
            }*/
        }
    }
}

