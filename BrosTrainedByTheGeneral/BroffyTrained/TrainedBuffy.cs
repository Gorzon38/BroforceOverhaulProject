using UnityEngine;

namespace TheGeneralsTraining.Components
{
    public class TrainedBuffy : MonoBehaviour
    {
        public Broffy broffy;
        public bool IsDashing
        {
            get
            {
                return broffy.dashing;
            }
        }

        private void Awake()
        {
            broffy = GetComponent<Broffy>();
            if (broffy == null)
                Destroy(this);
        }

        public bool doingFlyingKick = false;
        public bool FlyingKickDoesDamage()
        {
            return doingFlyingKick && broffy.frame > 1 && broffy.frame < 4;
        }

        public void StartFlyingKick()
        {
            if (doingFlyingKick)
                return;

            broffy.xIAttackExtra += 50f * broffy.Direction;
            doingFlyingKick = true;
        }

        public void StopFlyingKick()
        {
            doingFlyingKick = false;
            broffy.xIAttackExtra = 0f;
        }
    }
}
