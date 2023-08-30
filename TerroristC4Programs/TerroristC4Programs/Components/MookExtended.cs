using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TerroristC4Programs.Components
{
    public class MookExtended : MonoBehaviour
    {
        public Mook mook;
        public GibHolder decapitationGib;

        protected virtual void Awake()
        {
            decapitationGib = Map.Instance.activeTheme.mook.As<MookTrooper>().decapitationGib;
        }

        /*protected virtual void Update()
        {
            if (RunDecapitationUpdate())
            {
                float decapitationCounter = mook.GetFloat("decapitationCounter");
                decapitationCounter -= Time.deltaTime;
                if (decapitationCounter <= 0f)
                {
                    mook.Y += 2f;
                    mook.Damage(mook.health + 15, DamageType.Bullet, 0f, 12f, (int)Mathf.Sign(-mook.transform.localScale.x), mook, mook.X, mook.Y + 8f);
                }
                mook.SetFieldValue("decapitationCounter", decapitationCounter);
            }
        }*/

        protected virtual bool RunDecapitationUpdate()
        {
            return mook.NotAs<SkinnedMook>() && mook.NotAs<MookTrooper>() && mook.NotAs<MookSuicide>() && mook.NotAs<MookGrenadier>() && mook.NotAs<MookBigGuy>() && mook.GetBool("decapitated") && mook.IsAlive();
        }
    }
}
