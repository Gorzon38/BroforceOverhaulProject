using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(CaseyBroback))]
    public class CaseyBrobackPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        private static void ThreeSpecialAmmo(CaseyBroback __instance)
        {
            __instance.originalSpecialAmmo = 3;
        }

        [HarmonyPatch("PerformKnifeMeleeAttack")]
        [HarmonyPrefix]
        private static bool StrongerMelee(CaseyBroback __instance, bool shouldTryHitTerrain, bool playMissSound)
        {
            if (Main.CantUsePatch || !Main.settings.caseyBroBackStrongerMelee)
                return true;

            // From method 'CaseyBroback.PerformKnifeMeleeAttack(bool,bool)'

            Traverse t = __instance.GetTraverse();
            Sound sound = t.GetValue<Sound>("sound");
            Unit unit = Map.HitClosestUnit(__instance, __instance.playerNum, 0, DamageType.Melee, 14f, 24f, __instance.X + __instance.transform.localScale.x * 8f, __instance.Y + 8f, __instance.transform.localScale.x * 400 * (float)t.GetFieldValue<int>("meleeDirection"), 750, true, false, __instance.IsMine, false, true);
            if (unit != null)
            {
                Mook mook = unit as Mook;
                if (mook != null)
                {
                    mook.PlayFallSound(0.3f);
                }
                sound.PlaySoundEffectAt(__instance.soundHolder.meleeHitSound, 1f, __instance.transform.position, 1.5f, true, false, false, 0f);
                t.SetFieldValue("meleeHasHit", true);
            }
            else if (playMissSound)
            {
                sound.PlaySoundEffectAt(__instance.soundHolder.missSounds, 0.3f, __instance.transform.position, 1f, true, false, false, 0f);
            }
            t.SetFieldValue<Unit>("meleeChosenUnit", null);
            if (shouldTryHitTerrain && t.Method("TryMeleeTerrain", new object[] { 0, 2 }).GetValue<bool>())
            {
                t.SetFieldValue("meleeHasHit", true);
            }
            t.SetFieldValue("meleeDirection", t.GetFieldValue<int>("meleeDirection") * -1);
            return false;
        }
    }
}
