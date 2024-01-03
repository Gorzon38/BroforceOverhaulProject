using HarmonyLib;
using System;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Bros
{

    [HarmonyPatch(typeof(BroDredd))]
    public class BroDreddPatches
    {
        [HarmonyPatch("PerformTazerMeleeAttack")]
        [HarmonyPrefix]
        private static bool Prefix(BroDredd __instance, bool playMissSound, bool shouldTryHitTerrain)
        {
            if (Main.CantUsePatch || !Main.settings.lessTazerHit)
                return true;

            // Should make mook taking less tazer hit before exploding
            // From original method 'BroDredd.PerformTazerMeleeAttack(bool, bool)'
            try
            {

                Traverse t = __instance.GetTraverse();
                bool meleeHasHit = t.GetFieldValue<bool>("meleeHasHit");
                int tasedCount = t.GetFieldValue<int>("tasedCount");
                Unit previouslyTasedUnit = t.GetFieldValue<Unit>("previouslyTasedUnit");

                Map.DamageDoodads(3, DamageType.Shock, __instance.X + (float)(__instance.Direction * 4), __instance.Y, 0f, 0f, 6f, __instance.playerNum, out bool flag, null);
                Unit unit = Map.HitClosestUnit(__instance, __instance.playerNum, 1, DamageType.Shock, 13f, 24f, __instance.X + __instance.transform.localScale.x * 4f, __instance.Y + 8f, __instance.transform.localScale.x * 200f, 0f, false, true, __instance.IsMine, false, true);
                if (unit != null)
                {
                    meleeHasHit = true;
                    if (unit == previouslyTasedUnit)
                    {
                        tasedCount++;
                        if (tasedCount > 6)
                        {
                            Debug.Log("Not networked extra plasma damage");
                            unit.Damage(tasedCount / 6, DamageType.Plasma, 0f, 0f, __instance.Direction, __instance, unit.X, unit.Y + 5f);
                        }
                    }
                    else
                    {
                        tasedCount = 0;
                        previouslyTasedUnit = unit;
                    }
                }
                else if (playMissSound)
                {
                    t.GetFieldValue<Sound>("sound").PlaySoundEffectAt(__instance.soundHolder.alternateMeleeHitSound, 0.3f, __instance.transform.position, UnityEngine.Random.Range(0.9f, 1.1f), true, false, false, 0f);
                }
                if (shouldTryHitTerrain && t.Method("TryMeleeTerrain", new object[] { 0, 2 }).GetValue<bool>())
                {
                    meleeHasHit = true;
                }
                t.SetFieldValue("meleeHasHit", meleeHasHit);
                t.SetFieldValue("tasedCount", tasedCount);
                t.SetFieldValue("previouslyTasedUnit", previouslyTasedUnit);
                return false;

            }
            catch (Exception e)
            {
                Main.Log(e);
            }

            return true;
        }
    }
}
