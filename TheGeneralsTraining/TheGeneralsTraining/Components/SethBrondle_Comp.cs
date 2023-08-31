using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheGeneralsTraining.Components;
using UnityEngine;

namespace TheGeneralsTraining
{
    public class SethBrondle_Comp : ExtendedBroComponent<BrondleFly>
    {
        public static int bossDamage = 50;

        public bool isTeleporting = false;
        public float frameRate = 0.02f;

        protected float rate = 0f;
        protected int row = 7;
        protected int maxFrame = 7;
        protected int frame = 0;
        protected bool goOut = false;

        protected Vector3 teleportPosition = Vector3.zero;
        protected Unit hitUnit = null;

        public virtual void Teleport(Vector3 position, Unit unit)
        {
            if (isTeleporting)
                return;

            teleportPosition = position;
            hitUnit = unit;

            bro.invulnerable = true;
            isTeleporting = true;
            frame = 0;
            goOut = false;
            CheckMaxFrame();
            CheckRow();
            bro.CallMethod("DeactivateGun");

            UpdateSprite();
        }

        protected virtual void Update()
        {
            if (isTeleporting && (rate -= Time.deltaTime) <= 0)
            {
                UpdateSprite();
            }
        }

        protected virtual void UpdateSprite()
        {
            frame++;
            bro.SetSpriteLowerLeftPixel(frame, row);
            if (frame == maxFrame)
            {
                if (goOut)
                {
                    FinishTeleporting();
                }
                else
                {
                    StartGoingOut();
                }

            }
            rate = frameRate;
        }

        protected virtual void StartGoingOut()
        {
            goOut = true;
            CheckMaxFrame();
            CheckRow();
            frame = 0;

            if (teleportPosition != Vector3.zero)
            {
                if (Mathf.Abs(teleportPosition.x - bro.X) > 16f || Mathf.Abs(teleportPosition.y - bro.Y) > 16f)
                {
                    bro.SpecialAmmo--;
                }
                bro.SetFieldValue("pressSpecialFacingDirection", 0);
                bro.SetXY(teleportPosition.x, teleportPosition.y);
                bro.SetPosition();
            }

            if (hitUnit != null)
            {
                if (hitUnit is SatanMiniboss)
                {
                    hitUnit.Damage(bossDamage, DamageType.Knock, 0f, 360f, bro.Direction, bro, hitUnit.X, hitUnit.Y - 2f);
                }
                else
                {
                    hitUnit.Damage(hitUnit.health, DamageType.Normal, 0f, 0f, bro.Direction, bro, hitUnit.X, hitUnit.Y);
                    hitUnit.GibNow(DamageType.Crush, 0f, 100f);
                }

                bro.SetFieldValue("defaultMaterial", bro.coveredInBloodMaterial);
                bro.SetFieldValue("isBloody", true);
                bro.GetComponent<Renderer>().material = bro.GetFieldValue<Material>("defaultMaterial");
            }

            bro.xI = 0f;
            if (bro.up)
            {
                bro.SetFieldValue("jumpTime", 0f);
                bro.yI = 120f;
            }
            else
            {
                bro.yI = 0f;
            }

            Sound.GetInstance().PlayAudioClip(bro.teleportSound, bro.transform.position, 0.5f, UnityEngine.Random.Range(0.8f, 1.2f));
        }

        protected virtual void FinishTeleporting()
        {
            isTeleporting = false;
            bro.invulnerable = false;
            bro.CallMethod("ActivateGun");
            bro.DisConnectFaceHugger();
        }


        protected virtual void CheckRow()
        {
            if (bro.IsPerformanceEnhanced || hitUnit != null)
            {
                row = goOut ? 10 : 9;
            }
            else
                row = goOut ? 8 : 7;
        }

        protected virtual void CheckMaxFrame()
        {
            if (bro.IsPerformanceEnhanced || hitUnit != null)
            {
                maxFrame = goOut ? 11 : 8;
            }
            else
                maxFrame = 9;
        }
    }
}
