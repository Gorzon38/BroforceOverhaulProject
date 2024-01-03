using HarmonyLib;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Bros
{
    [HarmonyPatch(typeof(ScorpionBro))]
    public class ScorpionBroPatches
    {
        [HarmonyPatch("StartMelee")]
        [HarmonyPrefix]
        private static bool MeleeInStealthMode(ScorpionBro __instance)
        {
            if (Main.CantUsePatch || !Main.settings.stealthier)
                return true;

            Traverse t = __instance.GetTraverse();
            if (/*!t.Field("isInScorpionMode").GetValue<bool>() ||*/ (!__instance.IsHanging() && !t.GetFieldValue<bool>("WallDrag")))
            {
                __instance.counter = 0f;
                t.SetFieldValue("currentMeleeType", __instance.meleeType);

                RaycastHit raycastHit;
                LayerMask platformLayer = t.GetFieldValue<LayerMask>("platformLayer");
                if ((Physics.Raycast(new Vector3(__instance.X, __instance.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer) || Physics.Raycast(new Vector3(__instance.X + 4f, __instance.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer) || Physics.Raycast(new Vector3(__instance.X - 4f, __instance.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer)) && raycastHit.collider.GetComponentInParent<Animal>() != null)
                {
                    t.SetFieldValue("currentMeleeType", BroBase.MeleeType.Knife);
                }
                switch (t.GetFieldValue<BroBase.MeleeType>("currentMeleeType"))
                {
                    case BroBase.MeleeType.Knife:
                        t.Method("StartKnifeMelee").GetValue();
                        break;
                    case BroBase.MeleeType.Punch:
                    case BroBase.MeleeType.JetpackPunch:
                        t.Method("StartPunch").GetValue();
                        break;
                    case BroBase.MeleeType.Disembowel:
                    case BroBase.MeleeType.FlipKick:
                    case BroBase.MeleeType.Tazer:
                    case BroBase.MeleeType.Custom:
                    case BroBase.MeleeType.ChuckKick:
                    case BroBase.MeleeType.VanDammeKick:
                    case BroBase.MeleeType.ChainSaw:
                    case BroBase.MeleeType.ThrowingKnife:
                    case BroBase.MeleeType.Smash:
                    case BroBase.MeleeType.BrobocopPunch:
                    case BroBase.MeleeType.PistolWhip:
                    case BroBase.MeleeType.HeadButt:
                    case BroBase.MeleeType.TeleportStab:
                        t.Method("StartCustomMelee").GetValue();
                        break;
                }
            }
            return false;
        }
    }
}
