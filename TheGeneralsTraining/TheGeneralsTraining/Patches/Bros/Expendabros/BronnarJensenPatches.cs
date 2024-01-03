using HarmonyLib;

namespace TheGeneralsTraining.Patches.Bros.Expendabros
{
    [HarmonyPatch(typeof(BronnarJensen))]
    public class BronnarJensePatches
    {
        [HarmonyPatch("UseFire")]
        [HarmonyPrefix]
        private static bool NewUseFire(BronnarJensen __instance)
        {
            if (Main.CantUsePatch)
                return true;

            Traverse t = __instance.GetTraverse();
            if (t == null)
                return true;

            // From the original method 'BronnarJensen.UseFire()'

            if (__instance.IsMine)
            {
                // If the player is pressing down, Bronnar Jensen will launch the grenade at his feets
                if (__instance.IsDucking && __instance.down)
                {
                    // TODO: Remove all magic numbers so it can be more readable
                    t.Method("FireWeapon", new object[] { __instance.X + __instance.transform.localScale.x * 6f, __instance.Y + 7f, __instance.transform.localScale.x * (__instance.shootGrenadeSpeedX * 0.3f) + __instance.xI * 0.45f, 25f + ((__instance.yI <= 0f) ? 0f : (__instance.yI * 0.3f)) }).GetValue();
                }
                else
                {
                    t.Method("FireWeapon", new object[] { __instance.X + __instance.transform.localScale.x * 6f, __instance.Y + 10f, __instance.transform.localScale.x * __instance.shootGrenadeSpeedX + __instance.xI * 0.45f, __instance.shootGrenadeSpeedY + ((__instance.yI <= 0f) ? 0f : (__instance.yI * 0.3f)) }).GetValue();
                }
                t.Method("PlayAttackSound", new object[] { 0.4f }).GetValue();
            }

            Map.DisturbWildLife(__instance.X, __instance.Y, 60f, __instance.playerNum);
            __instance.fireDelay = 0.6f;

            return false;
        }
    }
}

