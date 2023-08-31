using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining.Components
{
    public class Buffy_Comp : ExtendedBroComponent<Broffy>
    {
        public bool IsDashing
        {
            get
            {
                return bro.dashing;
            }
        }

        public static int flyingKickRow = 8;
        public static int flyingKickCol = 0;
        public static int flyingKickMaxFrame = 8;

        public static bool doingFlyingKick = false;
        public bool FlyingKickDoesDamage()
        {
            return doingFlyingKick && bro.frame > 1 && bro.frame < 4;
        }

        public void StartFlyingKick()
        {
            if (doingFlyingKick) return;

            bro.xIAttackExtra += 50f * bro.Direction;
            doingFlyingKick = true;
        }

        public void StopFlyingKick()
        {
            doingFlyingKick = false;
            bro.xIAttackExtra = 0f;
        }
    }
}
