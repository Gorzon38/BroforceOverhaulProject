using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining
{
    public class SethBrondleTeleportation : MonoBehaviour
    {
        public static int bossDamage = 50;

        public BrondleFly brondleFly;
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

            brondleFly.invulnerable = true;
            isTeleporting = true;
            frame = 0;
            goOut = false;
            CheckMaxFrame();
            CheckRow();
            brondleFly.CallMethod("DeactivateGun");

            UpdateSprite();
        }

        protected virtual void Awake()
        {
            brondleFly = GetComponent<BrondleFly>();
            if (brondleFly == null)
                Destroy(this);

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
            brondleFly.SetSpriteLowerLeftPixel(frame, row);
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
                if (Mathf.Abs(teleportPosition.x - brondleFly.X) > 16f || Mathf.Abs(teleportPosition.y - brondleFly.Y) > 16f)
                {
                    brondleFly.SpecialAmmo--;
                }
                brondleFly.SetFieldValue("pressSpecialFacingDirection", 0);
                brondleFly.SetXY(teleportPosition.x, teleportPosition.y);
                brondleFly.SetPosition();
            }

            if (hitUnit != null)
            {
                if (hitUnit is SatanMiniboss)
                {
                    hitUnit.Damage(bossDamage, DamageType.Knock, 0f, 360f, brondleFly.Direction, brondleFly, hitUnit.X, hitUnit.Y - 2f);
                }
                else
                {
                    hitUnit.Damage(hitUnit.health, DamageType.Normal, 0f, 0f, brondleFly.Direction, brondleFly, hitUnit.X, hitUnit.Y);
                    hitUnit.GibNow(DamageType.Crush, 0f, 100f);
                }

                brondleFly.SetFieldValue("defaultMaterial", brondleFly.coveredInBloodMaterial);
                brondleFly.SetFieldValue("isBloody", true);
                brondleFly.GetComponent<Renderer>().material = brondleFly.GetFieldValue<Material>("defaultMaterial");
            }

            brondleFly.xI = 0f;
            if (brondleFly.up)
            {
                brondleFly.SetFieldValue("jumpTime", 0f);
                brondleFly.yI = 120f;
            }
            else
            {
                brondleFly.yI = 0f;
            }

            Sound.GetInstance().PlayAudioClip(brondleFly.teleportSound, brondleFly.transform.position, 0.5f, UnityEngine.Random.Range(0.8f, 1.2f));
        }

        protected virtual void FinishTeleporting()
        {
            isTeleporting = false;
            brondleFly.invulnerable = false;
            brondleFly.CallMethod("ActivateGun");
            brondleFly.DisConnectFaceHugger();
        }


        protected virtual void CheckRow()
        {
            if (brondleFly.IsPerformanceEnhanced || hitUnit != null)
            {
                row = goOut ? 10 : 9;
            }
            else
                row = goOut ? 8 : 7;
        }

        protected virtual void CheckMaxFrame()
        {
            if (brondleFly.IsPerformanceEnhanced || hitUnit != null)
            {
                maxFrame = goOut ? 11 : 8;
            }
            else
                maxFrame = 9;
        }
    }
}
