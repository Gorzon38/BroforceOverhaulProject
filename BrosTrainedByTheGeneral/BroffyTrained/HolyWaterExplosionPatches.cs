using Effects;
using HarmonyLib;
using System;
using System.Collections.Generic;
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

        // Burn Hell Units in Holy Water
        private static bool HitHellUnits(HolyWaterExplosion __instance, List<FlashBangPoint> persistentPoints, int i)
        {
            return Map.HitHellUnits(
                __instance.GetFieldValue<MonoBehaviour>("firedBy"),
                __instance.playerNum,
                __instance.GetFieldValue<int>("holyWaterDamage"),
                DamageType.Fire,
                HSettings.hitHellUnitsRange, // range
                Map.GetBlocksX(persistentPoints[i].collumn) + 8f, Map.GetBlocksY(persistentPoints[i].row) + 8f, // x, y
                0f, 0f, // xI, yI
                true, false, false, // penetrates, knock, ignoreDeadUnits
                canHeadshot: false
                );
        }

        private static bool CanSwapUnitToVillager(Unit unit)
        {
            return HSettings.mookToVillager && (unit is MookTrooper || unit is MookRiotShield || unit is MookSuicide || unit is ScoutMook || unit is MookBazooka);
        }
        private static void SwapUnitToVillager(Unit unit, int playerNum)
        {
            int randomIndex = Map.Instance.activeTheme.villager1.RandomIndex();
            TestVanDammeAnim villagerChoosed = Map.Instance.activeTheme.villager1[randomIndex];
            Villager villager = MapController.SpawnVillager_Networked(
                villagerChoosed.GetComponent<Villager>(), // villagerPrefab
                unit.X, unit.Y, // x, y
                0f, 0f, // xI, yI
                tumble: false,
                useParachuteDelay: false,
                useParachute: false,
                onFire: false,
                isAlert: true,
                playerNum
                );
            villager.Panic(0.3f, true);
            unit.DestroyNetworked();
        }

        private static bool CanSwapUnitToPig(Unit unit)
        {
            return HSettings.dogsToPigs && ( (unit is MookDog && !unit.As<MookDog>().isMegaDog) && !(unit is Alien) && !(unit is HellDog) && !(unit as MookDog).isMegaDog);
        }
        private static void SwapUnitToPig(Unit unit)
        {
            GameObject rottenPig = Map.Instance.activeTheme.animals[2];
            TestVanDammeAnim tvda = MapController.SpawnTestVanDamme_Networked(
                rottenPig.GetComponent<TestVanDammeAnim>(), // vanDamPrefab
                unit.X, unit.Y, // x, y
                0f, 0f, // xI, yI
                tumble: false,
                useParachuteDelay: false,
                useParachute: false,
                onFire: false
            );

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
                    if (CanSwapUnitToVillager(unit))
                    {
                        SwapUnitToVillager(unit, playerNum);
                    }
                    else if (CanSwapUnitToPig(unit))
                    {
                        SwapUnitToPig(unit);
                    }
                }
            }
        }

        private static void DoTheLoop(HolyWaterExplosion holyWaterExplosion, List<FlashBangPoint> persistentPoints, float burnTimer, float invulnerabilityTimer)
        {
            for (int i = 0; i < persistentPoints.Count; i++)
            {
                // 'brunTimer' and 'invulnerableTimer' values are changed in 'HolyWaterExplosionPatches::NewUpdate()'

                // also use 'burnTimer' for swapping
                if (burnTimer >= 0.5f)
                {
                    HitHellUnits(holyWaterExplosion, persistentPoints, i);
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
                // Get all variables values that the method need
                List<FlashBangPoint> persistentPoints = t.GetFieldValue<List<FlashBangPoint>>("persistentPoints");
                float counter = t.GetFloat("counter");
                float burnTimer = t.GetFloat("burnTimer");
                float invulnerabilityTimer = t.GetFloat("invulnerabilityTimer");
                float frameRate = t.GetFloat("frameRate");

                float maxTime = t.GetFloat("maxTime");
                float startTime = t.GetFloat("startTime");

                __instance.FirstUpdateFromPool();

                // Sprite animation frame rate
                float counterIncrement = Mathf.Clamp(Time.deltaTime, 0f, 0.0334f);
                counter += counterIncrement;
                if (counter >= frameRate)
                {
                    counter -= frameRate;
                    t.CallMethod("RunPoints");
                }

                // Times run out ? Yes: it dies
                if (Time.time - startTime > maxTime)
                {
                    __instance.EffectDie();
                }

                burnTimer += Time.deltaTime;
                invulnerabilityTimer += Time.deltaTime;

                // Custom Method to swap units to handles the swapping and hell unit burner
                DoTheLoop(__instance, persistentPoints, burnTimer, invulnerabilityTimer);

                if (burnTimer >= 0.5f)
                    burnTimer -= 0.5f;
                if (invulnerabilityTimer >= 0.2f)
                    invulnerabilityTimer -= 0.2f;

                // Delete useless point
                for (int i = persistentPoints.Count - 1; i >= 0; i--)
                {
                    if (!Map.IsBlockSolid(persistentPoints[i].collumn, persistentPoints[i].row - 1))
                    {
                        persistentPoints.RemoveAt(i);
                    }
                }

                // Set private & protected variables values that we changed
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
