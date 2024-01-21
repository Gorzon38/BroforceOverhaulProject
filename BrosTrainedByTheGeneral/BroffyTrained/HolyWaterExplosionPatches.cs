using Effects;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BroffyTrained
{
    [HarmonyPatch(typeof(HolyWaterExplosion))]
    public static class HolyWaterExplosionPatches
    {
        public static TrainedSettings TSettings
        {
            get => Main.settings.mod;
        }
        public static VanillaSettings VSettings
        {
            get => Main.settings.vanilla;
        }
        public static HolyWaterSettings HSettings
        {
            get => TSettings.holyWater;
        }

        private static void SetHeroesWillComebackToLife(List<FlashBangPoint> persistentPoints, int i)
        {
            Vector2 vector = Map.GetPosition(persistentPoints[i].collumn, persistentPoints[i].row);
            HeroController.SetHeroesWillComebackToLife(vector.x, vector.y, HSettings.reviveRange, HSettings.revivePointDuration);
        }

        private static bool HitHellUnits(HolyWaterExplosion __instance, List<FlashBangPoint> persistentPoints, int i)
        {
            return Map.HitHellUnits(
                __instance.GetFieldValue<MonoBehaviour>("firedBy"),
                __instance.playerNum,
                __instance.GetFieldValue<int>("holyWaterDamage"),
                DamageType.Fire,
                HSettings.hitHellUnitsRange,
                Map.GetBlocksX(persistentPoints[i].collumn) + 8f, Map.GetBlocksY(persistentPoints[i].row) + 8f, // x y
                0f, 0f,
                true, false, false,
                canHeadshot: false
                );
        }

        private static bool CanSwapToMook(Unit unit)
        {
            return HSettings.mookToVillager && (unit is MookTrooper || unit is UndeadTrooper || unit is MookRiotShield || unit is MookSuicide || unit is ScoutMook || unit is MookBazooka);
        }
        private static void SwapMookToVillager(Unit unit, int playerNum)
        {
            Villager villager = MapController.SpawnVillager_Networked(Map.Instance.activeTheme.villager1[0].GetComponent<Villager>(), unit.X, unit.Y, 0, 0, false, false, false, false, true, playerNum);
            villager.Panic(0.3f, true);
            unit.DestroyNetworked();
        }

        private static bool CanSwapToPig(Unit unit)
        {
            return HSettings.dogsToPigs && ( (unit is MookDog && !unit.As<MookDog>().isMegaDog) && !(unit is Alien) && !(unit is HellDog) && !(unit as MookDog).isMegaDog);
        }

        private static void SwapMookToPig(Unit unit)
        {
            TestVanDammeAnim tvda = MapController.SpawnTestVanDamme_Networked(Map.Instance.activeTheme.animals[2].GetComponent<TestVanDammeAnim>(), unit.X, unit.Y, 0f, 0f, false, false, false, false);
            //tvda.Panic(0.3f, false);
            unit.DestroyNetworked();
        }

        private static void Swaper(List<FlashBangPoint> persistentPoints, int i, int playerNum)
        {
            if (!HSettings.mookToVillager && !HSettings.dogsToPigs)
                return;

            float x = Map.GetBlocksX(persistentPoints[i].collumn) + 8f;
            float y = Map.GetBlocksY(persistentPoints[i].row) + 8f;
            List<Unit> units = Map.GetUnitsInRange((int)HSettings.mookToVillagerRange, x, y, true);
            foreach (Unit unit in units)
            {
                if (unit != null && (unit as Mook) && !unit.invulnerable && unit.IsAlive())
                {
                    if (CanSwapToMook(unit))
                    {
                        SwapMookToVillager(unit, playerNum);
                    }
                    else if (CanSwapToPig(unit))
                    {
                        SwapMookToPig(unit);
                    }
                }
            }
        }

        private static void DoTheLoop(HolyWaterExplosion holyWaterExplosion, List<FlashBangPoint> persistentPoints, float burnTimer, float invulnerabilityTimer)
        {
            for (int i = 0; i < persistentPoints.Count; i++)
            {
                if (burnTimer >= 0.5f)
                {
                    if (!HitHellUnits(holyWaterExplosion, persistentPoints, i))
                        Swaper(persistentPoints, i, holyWaterExplosion.playerNum);
                }

                if (invulnerabilityTimer >= 0.2f)
                {
                    SetHeroesWillComebackToLife(persistentPoints, i);
                }
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool NewUpdate(HolyWaterExplosion __instance)
        {
            if (Mod.CantUsePatch)
                return true;

            try
            {
                var t = Traverse.Create(__instance);
                List<FlashBangPoint> persistentPoints = t.GetFieldValue<List<FlashBangPoint>>("persistentPoints");
                float counter = t.GetFloat("counter");
                float burnTimer = t.GetFloat("burnTimer");
                float invulnerabilityTimer = t.GetFloat("invulnerabilityTimer");
                float frameRate = t.GetFloat("frameRate");

                float maxTime = t.GetFloat("maxTime");
                float startTime = t.GetFloat("startTime");

                __instance.FirstUpdateFromPool();

                float num = Mathf.Clamp(Time.deltaTime, 0f, 0.0334f);
                counter += num;
                if (counter >= frameRate)
                {
                    counter -= frameRate;
                    t.CallMethod("RunPoints");
                }
                if (Time.time - startTime > maxTime)
                {
                    __instance.EffectDie();
                }

                burnTimer += Time.deltaTime;
                invulnerabilityTimer += Time.deltaTime;

                DoTheLoop(__instance, persistentPoints, burnTimer, invulnerabilityTimer);

                if (burnTimer >= 0.5f)
                    burnTimer -= 0.5f;
                if (invulnerabilityTimer >= 0.2f)
                    invulnerabilityTimer -= 0.2f;

                for (int k = persistentPoints.Count - 1; k >= 0; k--)
                {
                    if (!Map.IsBlockSolid(persistentPoints[k].collumn, persistentPoints[k].row - 1))
                    {
                        persistentPoints.RemoveAt(k);
                    }
                }

                t.SetFieldValue("persistentPoints", persistentPoints);
                t.SetFieldValue("counter", counter);
                t.SetFieldValue("burnTimer", burnTimer);
                t.SetFieldValue("invulnerabilityTimer", invulnerabilityTimer);
                t.SetFieldValue("frameRate", frameRate);

                return false;
            }
            catch (Exception ex)
            {
                Main.Log(ex);
            }
            return true;
        }
    }
}
