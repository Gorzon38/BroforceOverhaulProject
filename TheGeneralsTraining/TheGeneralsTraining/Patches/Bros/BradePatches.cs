using UnityEngine;
using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(Blade))]
    public class BradePatches
    {
        [HarmonyPatch("ThrowKnife")]
        [HarmonyPrefix]
        private static bool ChangeKnifeToOldGlaive(Blade __instance)
        {
            if (Main.CantUsePatch || __instance.throwingKnife == null || !(__instance is Blade))
                return true;

            // From original method 'Blade.ThrowKnife()'
            __instance.SetFieldValue("knifeThrown", true);
            __instance.CallMethod("PlayAttackSound", 0.44f);
            var knife = ProjectileController.SpawnProjectileLocally(__instance.throwingKnife, __instance, __instance.X + (float)(16 * __instance.Direction), __instance.Y + 10f, __instance.xI + (float)(250 * __instance.Direction), 0f, __instance.playerNum);

            // Change the texture if it exist
            Texture2D texture = ResourcesController.GetTexture("ThrowingGlaive.png");
            if (texture != null)
            {
                knife.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
                knife.gameObject.AddComponent<Projectiles.RotateProjectile_Comp>();
            }
            return false;
        }
    }
}
