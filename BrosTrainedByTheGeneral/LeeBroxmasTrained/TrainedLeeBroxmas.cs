using Rogueforce;
using System.Collections.Generic;
using UnityEngine;

namespace LeeBroxmasTrained
{
    public class TrainedLeeBroxmas : MonoBehaviour
    {
        public LeeBroxmas leeBroxmas;

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

        private void Awake()
        {
            leeBroxmas = GetComponent<LeeBroxmas>();
            if (leeBroxmas == null)
                Destroy(leeBroxmas);

            leeBroxmas.meleeType = BroBase.MeleeType.BrobocopPunch;
        }

        public void StartCustomMelee()
        {
            if (!leeBroxmas.GetBool("doingMelee") || leeBroxmas.frame > 4)
            {
                leeBroxmas.frame = 0;
                leeBroxmas.counter = -0.05f;
                leeBroxmas.CallMethod("AnimateMelee");
            }
            else
            {
                leeBroxmas.SetFieldValue("meleeFollowUp", true);
            }
            leeBroxmas.CallMethod("StartMeleeCommon");
        }

        public void AnimateCustomMelee()
        {
            leeBroxmas.CallMethod("AnimateMeleeCommon");
            int row = (int)MSettings.groundStabAnimationPosition.x;
            int collumn = (int)MSettings.groundStabAnimationPosition.y + Mathf.Clamp(leeBroxmas.frame, 0, MSettings.maximumStabFrame);

            if (leeBroxmas.GetBool("jumpingMelee"))
            {
                row = (int)MSettings.airStabAnimationPosition.x;
            }

            if (leeBroxmas.frame == 3)
            {
                leeBroxmas.counter -= 0.0334f;
            }
            leeBroxmas.SetSpriteLowerLeftPixel(collumn, row);
            if (leeBroxmas.frame == 4 && !leeBroxmas.GetBool("meleeHasHit"))
            {
                PerformBroboCopPunchAttack(true, true);
            }
            else if (leeBroxmas.frame >= MSettings.maximumStabFrame - 1)
            {
                leeBroxmas.frame = 0;
                leeBroxmas.CallMethod("CancelMelee");
            }
        }

        public void RunCustomMeleeMovement()
        {
            bool jumpingMelee = leeBroxmas.GetBool("jumpingMelee");
            Unit meleeChosenUnit = leeBroxmas.GetFieldValue<Unit>("meleeChosenUnit");

            if (jumpingMelee)
            {
                leeBroxmas.CallMethod("ApplyFallingGravity");
                if (leeBroxmas.yI < leeBroxmas.maxFallSpeed)
                {
                    leeBroxmas.yI = leeBroxmas.maxFallSpeed;
                }
            }
            else if (!jumpingMelee)
            {
                if (leeBroxmas.frame < 2)
                {
                    leeBroxmas.xI = 0f;
                    leeBroxmas.yI = 0f;
                }
                else if (leeBroxmas.frame <= 5)
                {
                    if (meleeChosenUnit != null)
                    {
                        // probably an attraction toward the Chosen unit
                        float num = (meleeChosenUnit.transform.position - Vector3.right * (float)leeBroxmas.Direction * 12f).x - leeBroxmas.X;
                        leeBroxmas.xI = num / 0.08f;
                        leeBroxmas.xI = Mathf.Clamp(leeBroxmas.xI, -leeBroxmas.speed * 1.8f, leeBroxmas.speed * 1.8f);
                    }
                    else
                    {
                        leeBroxmas.xI = leeBroxmas.speed * 1.7f * leeBroxmas.transform.localScale.x;
                    }
                }
                else if (leeBroxmas.frame <= MSettings.maximumStabFrame - 1)
                {
                    leeBroxmas.xI = 0f;
                }
                leeBroxmas.CallMethod("ApplyFallingGravity");
            }

            if (!jumpingMelee && !leeBroxmas.GetBool("dashingMelee") && meleeChosenUnit == null)
            {
                leeBroxmas.CallMethod("ApplyFallingGravity");
                if (leeBroxmas.yI < leeBroxmas.maxFallSpeed)
                {
                    leeBroxmas.yI = leeBroxmas.maxFallSpeed;
                }
                leeBroxmas.xI = 0f;
                if (leeBroxmas.Y > leeBroxmas.groundHeight + 1f)
                {
                    leeBroxmas.CallMethod("CancelMelee");
                }
            }
        }

        public void PerformBroboCopPunchAttack(bool shouldTryHitTerrain, bool playMissSound)
        {
            float range = MSettings.stabRange;
            Vector3 knifePosition = new Vector3(
                leeBroxmas.X + (float)leeBroxmas.Direction * (range + MSettings.knifePosition.x),
                leeBroxmas.Y + MSettings.knifePosition.y,
                0f);

            // Doodads
            Map.DamageDoodads(MSettings.stabDoodadDamage, DamageType.Melee,
                knifePosition.x, knifePosition.y,
                0f, 0f, // xI yI
                range,
                leeBroxmas.playerNum,
                out bool hitImpenetrableDoodads);
            leeBroxmas.CallMethod("KickDoors", MSettings.kickDoorsRange);

            // Units
            List<Unit> alreadyHitUnits = new List<Unit>();
            Unit firstUnit = Map.GetFirstUnit(leeBroxmas, leeBroxmas.playerNum, range,
                knifePosition.x, knifePosition.y,
                MSettings.onlyLiving, MSettings.hitSuicide,
                alreadyHitUnits);
            if (firstUnit)
            {
                firstUnit.Damage(MSettings.stabUnitsDamage, MSettings.damageType,
                    leeBroxmas.xI, leeBroxmas.yI,
                    (int)Mathf.Sign(leeBroxmas.xI),
                    leeBroxmas,
                    leeBroxmas.X, leeBroxmas.Y
                    );
                Sound.GetInstance().PlaySoundEffectAt(leeBroxmas.soundHolder.alternateMeleeHitSound, 0.5f, leeBroxmas.transform.position);
                leeBroxmas.SetFieldValue("meleeHasHit", true);
            }
            else
            {
                if (playMissSound && !leeBroxmas.GetBool("hasPlayedMissSound"))
                {
                    Sound.GetInstance().PlaySoundEffectAt(leeBroxmas.soundHolder.missSounds, 0.3f, leeBroxmas.transform.position);
                }
                leeBroxmas.SetFieldValue("hasPlayedMissSound", true);
            }
            leeBroxmas.SetFieldValue("meleeChosenUnit", null);
            if (!leeBroxmas.GetBool("meleeHasHit") && shouldTryHitTerrain && leeBroxmas.CallMethod<bool>("TryMeleeTerrain", 0, 2))
            {
                leeBroxmas.SetFieldValue("meleeHasHit", true);
            }
        }
    }
}