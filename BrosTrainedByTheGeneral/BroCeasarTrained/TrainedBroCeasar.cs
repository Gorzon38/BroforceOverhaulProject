using Rogueforce;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroCeasarTrained
{
    public class TrainedBroCeasar : MonoBehaviour
    {
        public BroCeasar broCeasar;

        public static TrainedSettings TSettings
        {
            get => Main.settings.mod;
        }
        public static MeleeSettings MSettings
        {
            get => TSettings.melee;
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

        private int _bronanPunchAnimationRow = 0;
        private int _bronanPunchAnimationColumn = 0;
        private float _smashingTime = 0f;

        private void Awake()
        {
            broCeasar = GetComponent<BroCeasar>();
            if (broCeasar == null)
                Destroy(this);

            broCeasar.meleeType = BroBase.MeleeType.Smash;
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

            if (!broCeasar.GetBool("doingMelee") || broCeasar.frame > 4)
            {
                broCeasar.frame = 0;
                broCeasar.counter = -0.05f;
                broCeasar.CallMethod("AnimateMelee");
            }
            else
            {
                broCeasar.SetFieldValue("meleeFollowUp", true);
            }

            broCeasar.CallMethod("StartMeleeCommon");
            Sound.GetInstance().PlaySoundEffectAt(broCeasar.soundHolder.missSounds, 0.3f, broCeasar.transform.position);
            broCeasar.SetFieldValue("hasPlayedMissSound", true);
            smashing = broCeasar.GetBool("jumpingMelee");

            if (smashing)
            {
                _smashingTime = Time.time;
                broCeasar.actionState = ActionState.Jumping;
                float xRange = TSettings.melee.searchSmashingRange.x;
                float yRange = TSettings.melee.searchSmashingRange.y;
                float x = broCeasar.X + xRange * (float)broCeasar.Direction - 6f;
                float y = broCeasar.Y - yRange;
                List<Unit> unitsInRange = Map.GetUnitsInRange(xRange, yRange, x, y, true);

                broCeasar.SetFieldValue("meleeChosenUnit", null);
                foreach (Unit unit in unitsInRange)
                {
                    float distanceX = Mathf.Abs(unit.X - broCeasar.X);
                    float distanceY = Mathf.Abs(unit.Y - broCeasar.Y);
                    RaycastHit raycastHit;
                    if (distanceX * 1.5f < distanceY && unit.Y < broCeasar.Y && Physics.Raycast(broCeasar.transform.position, unit.transform.position, out raycastHit, distanceY + 16f, Map.groundLayer))
                    {
                        if (raycastHit.distance > Mathf.Abs(distanceY) - 20f)
                        {
                            broCeasar.SetFieldValue("meleeChosenUnit", unit);
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
            broCeasar.CallMethod("AnimateMeleeCommon");
            int row = smashingAnimationPosition.x;
            int column = smashingAnimationPosition.y + Mathf.Clamp(broCeasar.frame, 0, TSettings.melee.maximumSmashingFrame);

            // the fourth frame is the last when in the air.
            // it wait to hit the ground before show the next frame
            if (broCeasar.frame == TSettings.melee.lastSmashingAirFrame)
            {
                broCeasar.counter -= 0.3f;
            }
            if (broCeasar.GetBool("highFive") && broCeasar.frame > TSettings.melee.lastSmashingAirFrame && !broCeasar.IsOnGround())
            {
                broCeasar.frame = TSettings.melee.lastSmashingAirFrame;
            }

            broCeasar.SetSpriteLowerLeftPixel(column, row);
            if (broCeasar.frame >= TSettings.melee.maximumSmashingFrame - 1) // Last frame
            {
                broCeasar.CallMethod("CancelMelee");
            }
        }

        public void RunCustomMeleeMovement()
        {
            if (!smashing)
            {
                broCeasar.CallMethod("RunPunchMovement");
                return;
            }

            // Smashing
            if (!broCeasar.IsOnGround())
            {
                broCeasar.CallMethod("ApplyFallingGravity");
            }

            if (broCeasar.frame > 2 && !broCeasar.IsOnGround())
            {
                broCeasar.maxFallSpeed = broCeasar.GetFloat("originalMaxFallSpeed") * 1.3f;
                broCeasar.yI -= TSettings.melee.smashingFallingSpeed * broCeasar.GetFloat("t");
            }
            if (broCeasar.yI < broCeasar.maxFallSpeed)
            {
                broCeasar.yI = broCeasar.maxFallSpeed;
            }
            Unit meleeChosenUnit = broCeasar.GetFieldValue<Unit>("meleeChosenUnit");
            if (meleeChosenUnit == null)
            {
                broCeasar.xI *= 1f - broCeasar.GetFloat("t") * 6f;
            }
            else
            {
                float num = meleeChosenUnit.X - broCeasar.X;
                broCeasar.xI = num / 0.1f;
                broCeasar.xI = Mathf.Clamp(broCeasar.xI, -broCeasar.speed * 1.7f, broCeasar.speed * 1.7f);
            }
            if (broCeasar.frame == 4 && !broCeasar.GetBool("meleeHasHit"))
            {
                broCeasar.CallMethod("PerformSmashAttack");
            }
            if (broCeasar.IsOnGround())
            {
                if (broCeasar.frame < TSettings.melee.lastSmashingAirFrame + 1)
                {
                    broCeasar.frame = TSettings.melee.lastSmashingAirFrame + 1;
                    broCeasar.CallMethod("AnimateMelee");
                    broCeasar.counter = 0f;
                }
                if (Time.time - _smashingTime > 0.36f)
                {
                    broCeasar.SetInvulnerable(0.2f, true, false);
                    MakeSmashBlast(broCeasar.X, broCeasar.Y, true);
                    broCeasar.counter = -0.2f;
                    broCeasar.SetFieldValue("stunTime", 0.06f);
                    _smashingTime = Time.time;
                }
            }
        }


        public void AnimateBronanPunch()
        {
            broCeasar.CallMethod("AnimateMeleeCommon");

            int row = _bronanPunchAnimationRow;
            int column = _bronanPunchAnimationColumn + Mathf.Clamp(broCeasar.frame, 0, MSettings.maximumPunchFrame);
            broCeasar.SetSpriteLowerLeftPixel(column, row);

            if (broCeasar.frame == 4)
            {
                broCeasar.counter = -0.066f;
            }
            if (broCeasar.frame >= 3 && broCeasar.frame < 6 && !broCeasar.GetBool("meleeHasHit"))
            {
                PerformUpperCut(true, true);
            }
            if (broCeasar.frame >= 7)
            {
                broCeasar.frame = 0;
                broCeasar.CallMethod("CancelMelee");
            }
            if (!broCeasar.GetBool("meleeHasHit") && !broCeasar.GetBool("hasPlayedMissSound"))
            {
                broCeasar.SetFieldValue("hasPlayedMissSound", true);
            }
        }

        public void PerformUpperCut(bool shouldTryHitTerrain, bool playMissSound)
        {
            Map.DamageDoodads(MSettings.punchDoodadDamage, DamageType.Knifed,
                broCeasar.X + (float)(broCeasar.Direction * 4),
                broCeasar.Y,
                0f, 0f,
                MSettings.punchRange,
                broCeasar.playerNum,
                out bool hitImpenetrableDoodad
            );
            broCeasar.CallMethod("KickDoors", 24f);

            List<Unit> list = new List<Unit>();
            if (Map.HitUnits(broCeasar, broCeasar.playerNum, MSettings.punchUnitsDamage, DamageType.Melee,
                    MSettings.punchRange,
                    broCeasar.X + (float)(broCeasar.Direction * 8),
                    broCeasar.Y + 8f,
                    broCeasar.xI + (float)(broCeasar.Direction * MSettings.unitPunchedVelocity.x),
                    broCeasar.yI + MSettings.unitPunchedVelocity.y,
                    false, false, false, list, false, true
                )
            )
            {
                Sound.GetInstance().PlaySoundEffectAt(broCeasar.soundHolder.alternateMeleeHitSound, 0.5f, broCeasar.transform.position, 1f, true, false, false, 0f);
                broCeasar.SetFieldValue("meleeHasHit", true);
            }
            else if (playMissSound)
            { }

            broCeasar.SetFieldValue("meleeChosenUnit", null);
            if (shouldTryHitTerrain && broCeasar.CallMethod<bool>("TryMeleeTerrain", 0, 2))
            {
                broCeasar.SetFieldValue("meleeHasHit", true);
            }
        }

        public void MakeSmashBlast(float xPoint, float yPoint, bool groundWave)
        {
            Map.ExplodeUnits(broCeasar,
                MSettings.smashingDamages, // Damage
                DamageType.Crush,
                MSettings.smashingDamageRange, // range
                MSettings.smashingKillRange, // kill range
                xPoint, yPoint,
                300f, // force
                240f, //yI
                broCeasar.playerNum, false, false);
            MapController.DamageGround(broCeasar, ValueOrchestrator.GetModifiedDamage(15, broCeasar.playerNum), DamageType.Explosion, 25f, xPoint, yPoint);
            EffectsController.CreateWhiteFlashPop(xPoint, yPoint);
            broCeasar.PlaySpecial3Sound(0.25f);
            if (groundWave)
            {
                EffectsController.CreateGroundWave(xPoint, yPoint + 1f, MSettings.groundWaveRange);
                Map.ShakeTrees(broCeasar.X, broCeasar.Y, MSettings.smashingShakeTreesRange.x, MSettings.smashingShakeTreesRange.y, MSettings.smashingShakeTreesForce);
            }
            Map.DisturbWildLife(broCeasar.X, broCeasar.Y, MSettings.smashingDisturbWildLifeRange, broCeasar.playerNum);

            if (MSettings.shootOnSmashBlast)
            {
                StartCoroutine(SmashShoot());
            }
        }

        IEnumerator SmashShoot()
        {
            int max = UnityEngine.Random.Range(1, 4);
            for (int i = 0; i < max; i++)
            {
                broCeasar.CallMethod("UseFire");
                yield return new WaitForSecondsRealtime(0.06f);
            }
        }
    }
}