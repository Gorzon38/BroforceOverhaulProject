using HarmonyLib;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Bros.Expendabros
{
    [HarmonyPatch(typeof(LeeBroxmas))]
    public class LeeBroxmasPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void ChangeKnifesTexture(LeeBroxmas __instance)
        {
            if (Main.CantUsePatch)
                return;

            // The actual knife is the Brochete glaive, so it's replaced by the throwing knife from Brade

            TestVanDammeAnim blade = HeroController.GetHeroPrefab(HeroType.Blade);
            if (blade == null)
                return;

            Texture bladeKnifeTex = (blade as Blade).throwingKnife.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
            if (bladeKnifeTex != null)
            {
                __instance.projectile.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = bladeKnifeTex;
                __instance.macheteSprayProjectile.gameObject.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = bladeKnifeTex;
            }
        }

        [HarmonyPatch("AnimatePushing")]
        [HarmonyPostfix]
        private static void FixPushingAnimation(LeeBroxmas __instance)
        {
            // TODO : Fix pushing
            /* if (Main.CantUsePatch)
             {
                 try
                 {
                     __instance.gunSprite.transform.localPosition = TFP_Utility.GetBroGunVector3PositionWhilePushing(HeroType.LeeBroxmas);
                     float pushingTime = Traverse.Create(__instance).Field("pushingTime").GetValue<float>();
                     if (__instance.fire || pushingTime <= 0)
                     {
                         __instance.gunSprite.transform.localScale = new Vector3(-1f, 1f, 1f);
                         __instance.gunSprite.transform.localPosition = TFP_Utility.GetBroGunVector3PositionWhenFinishPushing(__instance.heroType);
                     }

                 }
                 catch (Exception ex)
                 {
                     Main.ExceptionLog("Failed to patch Lee Broxmas Pushing", ex);
                 }
             }*/
        }
    }
}

