using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining.Components.Bros
{

    public class Xena_Comp : MonoBehaviour
    {
        public bool hasCallChakram;
    }

    public class DoubleBroSeven_Comp : MonoBehaviour
    {
        public Texture2D balaclavaTex;
        public Texture originalTex;

        void Awake()
        {
            balaclavaTex = ResourcesController.GetTexture("avatar_balaclava.png");
        }

        public void SetBalaclava(PlayerHUD hud)
        {
            if (balaclavaTex == null) return;

            originalTex = hud.avatar.meshRender.material.mainTexture;
            hud.avatar.meshRender.material.mainTexture = balaclavaTex;
        }

        public void RemoveBalaclava(PlayerHUD hud)
        {
            if (originalTex == null) return;

            hud.avatar.meshRender.material.mainTexture = originalTex;
        }
    }

    public class ChevBrolios_Comp : MonoBehaviour
    {
        float onFireMaxTime = 0.32f;
        float onFireTime;
        FlickerFader[] flames;
        ChevBrolios chev;

        private void Awake()
        {
            try
            {
                flames = (Map.Instance.activeTheme.mook as Mook).flames;
                chev = gameObject.GetComponent<ChevBrolios>();
            }
            catch (Exception e)
            {
                Main.Log(e);
            }
        }

        private void Update()
        {
            try
            {
                if (onFireTime > 0)
                {
                    onFireTime -= 0.01f;
                    if (flames != null)
                    {
                        EffectsController.CreateEffect(flames[UnityEngine.Random.Range(0, flames.Length)], chev.X, chev.Y + chev.height, chev.transform.position.z + 1f);
                    }
                }
            }
            catch (Exception e)
            {
                Main.Log(e);
            }
        }

        public bool IsOnFire()
        {
            return onFireTime > 0;
        }

        public void UseSpecial()
        {
            onFireTime = onFireMaxTime;
        }
    }
}
