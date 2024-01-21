using Rogueforce;
using System.Collections.Generic;
using UnityEngine;

namespace BrommandoTrained
{
    public class TrainedBrommando : MonoBehaviour
    {
        public Brommando brommando;

        public static TrainedSettings TSettings
        {
            get => Main.settings.mod;
        }
        public static MeleeSettings MSettings
        {
            get => TSettings.melee;
        }
        public static VanillaSettings VSettings
        {
            get => Main.settings.vanilla;
        }
        // Settings
        public static Vector2Int firstPunchAnimationPosition
        {
            get { return TSettings.melee.firstPunchAnimationPosition.ToVector2Int(); }
        }
        public static Vector2Int secondPunchAnimationPosition
        {
            get { return TSettings.melee.secondPunchAnimationPosition.ToVector2Int(); }
        }
        public static Vector2Int smashingAnimationPosition
        {
            get { return TSettings.melee.smashingAnimationPosition.ToVector2Int(); }
        }

        // Bronan Variables
        public int punchCount { get; private set; } = 0;
        public bool smashing { get; private set; } = false;
        public bool willShootAtHisFeet { get; private set; } = false;
        public bool ShootingNewBarage { get; set; } = false;

        private int _bronanPunchAnimationRow = 0;
        private int _bronanPunchAnimationColumn = 0;
        private float _smashingTime = 0f;
        private bool _hasShot = false;
        private bool _shootingNewBarage = false;

        private void Awake()
        {
            brommando = GetComponent<Brommando>();
            if (brommando == null)
                Destroy(this);

            brommando.meleeType = BroBase.MeleeType.Smash;
        }

        // Bronan Methods
        public void StartCustomMelee() // Called First
        {
            if (punchCount % 2 == 0)
            {
                _bronanPunchAnimationColumn = firstPunchAnimationPosition.y;
                _bronanPunchAnimationRow = firstPunchAnimationPosition.x;
            }
            else
            {
                _bronanPunchAnimationColumn = secondPunchAnimationPosition.y;
                _bronanPunchAnimationRow = secondPunchAnimationPosition.x;
            }
            punchCount++;

            if (!brommando.GetBool("doingMelee") || brommando.frame > 4)
            {
                brommando.frame = 0;
                brommando.counter = -0.05f;
                brommando.CallMethod("AnimateMelee");
            }
            else
            {
                brommando.SetFieldValue("meleeFollowUp", true);
            }

            brommando.CallMethod("StartMeleeCommon");
            Sound.GetInstance().PlaySoundEffectAt(brommando.soundHolder.missSounds, 0.3f, brommando.transform.position);
            brommando.SetFieldValue("hasPlayedMissSound", true);
            smashing = brommando.GetBool("jumpingMelee");

            if (smashing)
            {
                _smashingTime = Time.time;
                brommando.actionState = ActionState.Jumping;
                float xRange = TSettings.melee.searchSmashingRange.x;
                float yRange = TSettings.melee.searchSmashingRange.y;
                float x = brommando.X + xRange * (float)brommando.Direction - 6f;
                float y = brommando.Y - yRange;
                List<Unit> unitsInRange = Map.GetUnitsInRange(xRange, yRange, x, y, true);

                brommando.SetFieldValue("meleeChosenUnit", null);
                foreach (Unit unit in unitsInRange)
                {
                    float distanceX = Mathf.Abs(unit.X - brommando.X);
                    float distanceY = Mathf.Abs(unit.Y - brommando.Y);
                    RaycastHit raycastHit;
                    if (distanceX * 1.5f < distanceY && unit.Y < brommando.Y && Physics.Raycast(brommando.transform.position, unit.transform.position, out raycastHit, distanceY + 16f, Map.groundLayer))
                    {
                        if (raycastHit.distance > Mathf.Abs(distanceY) - 20f)
                        {
                            brommando.SetFieldValue("meleeChosenUnit", unit);
                        }
                        break;
                    }
                }
            }
        }


        public void AnimateCustomMelee()
        {
            if (!smashing)
            {
                AnimateBronanPunch();
                return;
            }
            // Smashing animation
            brommando.CallMethod("AnimateMeleeCommon");
            int row = smashingAnimationPosition.x;
            int column = smashingAnimationPosition.y + Mathf.Clamp(brommando.frame, 0, TSettings.melee.maximumSmashingFrame);

            // the fourth frame is the last when in the air.
            // it wait to hit the ground before show the next frame
            if (brommando.frame == TSettings.melee.lastSmashingAirFrame)
            {
                brommando.counter -= 0.3f;
            }
            if (brommando.GetBool("highFive") && brommando.frame > TSettings.melee.lastSmashingAirFrame && !brommando.IsOnGround())
            {
                brommando.frame = TSettings.melee.lastSmashingAirFrame;
            }

            brommando.SetSpriteLowerLeftPixel(column, row);
            if (brommando.frame >= TSettings.melee.maximumSmashingFrame - 1) // Last frame
            {
                brommando.CallMethod("CancelMelee");
            }
        }

        public void RunCustomMeleeMovement()
        {
            if (!smashing)
            {
                brommando.CallMethod("RunPunchMovement");
                return;
            }

            // Smashing
            if (!brommando.IsOnGround())
            {
                brommando.CallMethod("ApplyFallingGravity");
            }

            if (brommando.frame > 2 && !brommando.IsOnGround())
            {
                brommando.maxFallSpeed = brommando.GetFloat("originalMaxFallSpeed") * 1.3f;
                brommando.yI -= TSettings.melee.smashingFallingSpeed * brommando.GetFloat("t");
            }
            if (brommando.yI < brommando.maxFallSpeed)
            {
                brommando.yI = brommando.maxFallSpeed;
            }
            Unit meleeChosenUnit = brommando.GetFieldValue<Unit>("meleeChosenUnit");
            if (meleeChosenUnit == null)
            {
                brommando.xI *= 1f - brommando.GetFloat("t") * 6f;
            }
            else
            {
                float num = meleeChosenUnit.X - brommando.X;
                brommando.xI = num / 0.1f;
                brommando.xI = Mathf.Clamp(brommando.xI, -brommando.speed * 1.7f, brommando.speed * 1.7f);
            }
            if (brommando.frame == 4 && !brommando.GetBool("meleeHasHit"))
            {
                brommando.CallMethod("PerformSmashAttack");
            }
            if (brommando.IsOnGround())
            {
                if (brommando.frame < TSettings.melee.lastSmashingAirFrame + 1)
                {
                    brommando.frame = TSettings.melee.lastSmashingAirFrame + 1;
                    brommando.CallMethod("AnimateMelee");
                    brommando.counter = 0f;
                }
                if (Time.time - _smashingTime > 0.36f)
                {
                    brommando.SetInvulnerable(0.2f, true, false);
                    MakeSmashBlast(brommando.X, brommando.Y, true);
                    brommando.counter = -0.2f;
                    brommando.SetFieldValue("stunTime", 0.06f);
                    _smashingTime = Time.time;
                }
            }
        }


        public void AnimateBronanPunch()
        {
            brommando.CallMethod("AnimateMeleeCommon");

            int row = _bronanPunchAnimationRow;
            int column = _bronanPunchAnimationColumn + Mathf.Clamp(brommando.frame, 0, MSettings.maximumPunchFrame);
            brommando.SetSpriteLowerLeftPixel(column, row);

            if (brommando.frame == 4)
            {
                brommando.counter = -0.066f;
            }
            if (brommando.frame >= 3 && brommando.frame < 6 && !brommando.GetBool("meleeHasHit"))
            {
                PerformUpperCut(true, true);
            }
            if (brommando.frame >= 7)
            {
                brommando.frame = 0;
                brommando.CallMethod("CancelMelee");
            }
            if (!brommando.GetBool("meleeHasHit") && !brommando.GetBool("hasPlayedMissSound"))
            {
                brommando.SetFieldValue("hasPlayedMissSound", true);
            }
        }

        public void PerformUpperCut(bool shouldTryHitTerrain, bool playMissSound)
        {
            Map.DamageDoodads(MSettings.punchDoodadDamage, DamageType.Knifed,
                brommando.X + (float)(brommando.Direction * 4),
                brommando.Y,
                0f, 0f,
                MSettings.punchRange,
                brommando.playerNum,
                out bool hitImpenetrableDoodad
            );
            brommando.CallMethod("KickDoors", 24f);

            List<Unit> list = new List<Unit>();
            if (Map.HitUnits(brommando, brommando.playerNum, MSettings.punchUnitsDamage, DamageType.Melee,
                    MSettings.punchRange,
                    brommando.X + (float)(brommando.Direction * 8),
                    brommando.Y + 8f,
                    brommando.xI + (float)(brommando.Direction * MSettings.unitPunchedVelocity.x),
                    brommando.yI + MSettings.unitPunchedVelocity.y,
                    false, false, false, list, false, true
                )
            )
            {
                Sound.GetInstance().PlaySoundEffectAt(brommando.soundHolder.alternateMeleeHitSound, 0.5f, brommando.transform.position, 1f, true, false, false, 0f);
                brommando.SetFieldValue("meleeHasHit", true);
            }
            else if (playMissSound)
            { }

            brommando.SetFieldValue("meleeChosenUnit", null);
            if (shouldTryHitTerrain && brommando.CallMethod<bool>("TryMeleeTerrain", 0, 2))
            {
                brommando.SetFieldValue("meleeHasHit", true);
            }
        }

        public void MakeSmashBlast(float xPoint, float yPoint, bool groundWave)
        {
            Map.ExplodeUnits(brommando,
                MSettings.smashingDamages, // Damage
                DamageType.Crush,
                MSettings.smashingDamageRange, // range
                MSettings.smashingKillRange, // kill range
                xPoint, yPoint,
                300f, // force
                240f, //yI
                brommando.playerNum, false, false);
            MapController.DamageGround(brommando, ValueOrchestrator.GetModifiedDamage(15, brommando.playerNum), DamageType.Explosion, 25f, xPoint, yPoint);
            EffectsController.CreateWhiteFlashPop(xPoint, yPoint);
            brommando.PlaySpecial3Sound(0.25f);
            if (groundWave)
            {
                EffectsController.CreateGroundWave(xPoint, yPoint + 1f, MSettings.groundWaveRange);
                Map.ShakeTrees(brommando.X, brommando.Y, MSettings.smashingShakeTreesRange.x, MSettings.smashingShakeTreesRange.y, MSettings.smashingShakeTreesForce);
            }
            Map.DisturbWildLife(brommando.X, brommando.Y, MSettings.smashingDisturbWildLifeRange, brommando.playerNum);
        }

        void Update()
        {
            if (willShootAtHisFeet)
            {
                brommando.CallMethod("DeactivateGun");
            }
        }

        public void PrepareFeetShoot()
        {
            brommando.CallMethod("DeactivateGun");
            willShootAtHisFeet = true;
            _hasShot = false;
        }

        public void StopFeetShoot()
        {
            brommando.CallMethod("ActivateGun");
            willShootAtHisFeet = false;
            _hasShot = false;
        }

        public void FireWeaponToFeet()
        {
            if (_hasShot)
                return;

            Vector2 spawnOffset = TSettings.projectileSpawnOffsetFeet;
            Vector2 projectileSpeed = TSettings.projectileSpeedAtFeet;
            brommando.CallMethod("FireWeapon",
                brommando.X + spawnOffset.x,
                brommando.Y - brommando.transform.localScale.y  - spawnOffset.y,
                projectileSpeed.x,
                brommando.transform.localScale.y * projectileSpeed.y
                );
            brommando.yI = TSettings.fireAtFeetYBlast;
            _hasShot = true;
        }

        public void NewBarrage()
        {
            if (!ShootingNewBarage)
                return;

            float barageCounter = brommando.GetFloat("barageCounter");
            barageCounter -= brommando.GetFloat("t");
            if (barageCounter <= 0f)
            {
                barageCounter = 0.1333f;
                int barageCount = brommando.GetInt("barageCount");
                barageCount--;
                if (barageCount >= 0)
                {
                    ProjectileController.SpawnProjectileOverNetwork(brommando.barageProjectile, brommando,
                        brommando.X + brommando.transform.localScale.x * VSettings.projectileSpawnOffset.x,
                        brommando.Y + VSettings.projectileSpawnOffset.y,
                        Mathf.Sign(brommando.transform.localScale.x) * 150f,
                        0f, false, brommando.playerNum, false, false, 0f);
                    brommando.CallMethod("PlayAttackSound");
                }
                else
                {
                    ShootingNewBarage = false;
                }
                brommando.SetFieldValue("barageCount", barageCount);
            }
            brommando.SetFieldValue("barageCounter", barageCounter);
        }
    }
}