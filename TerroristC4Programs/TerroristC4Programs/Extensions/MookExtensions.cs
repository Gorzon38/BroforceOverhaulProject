using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerroristC4Programs.Components;
using UnityEngine;

namespace TerroristC4Programs.Extensions
{
    public static class MookExtensions
    {
        public static bool IsStrong(this Mook self)
        {
            return self != null && self.gameObject.name.Contains("Strong");
        }
        public static bool IsScientist(this Mook self)
        {
            return self != null && self.gameObject.name.Contains("Scientist");
        }
        public static bool IsTreasure(this Mook self)
        {
            return self != null && self.gameObject.name.Contains("Treasure");
        }

        public static bool IsSnake(this Mook self)
        {
            return self as AlienFaceHugger && self.As<AlienFaceHugger>().gameObject.name.Contains("Snake");
        }

        public static string GetSkinnedName(this Mook self)
        {
            if (self.skinnedPrefab == null)
                return string.Empty;

            if (self as MookJetpack)
                return nameof(MookJetpack);
            if (self as UndeadTrooper)
                return nameof(UndeadTrooper);
            if (self as MookTrooper || self as MookSuicide || self as MookRiotShield || self as ScoutMook)
                return nameof(Mook);

            return string.Empty;
        }
        public static bool HasCustomDecapitation(this Mook mook)
        {
            return TextureManager.FileExist(mook.GetType().Name);
        }
        public static string GetDecapitationName(this Mook self)
        {
            string result = string.Empty;

            if (self.IsScientist())
                result = "Scientist";
            else if (self.IsTreasure())
                result = "MookTreasure";
            else if (self as MookJetpack)
                result = nameof(MookJetpack);
            else if (self as UndeadTrooper)
                result = nameof(UndeadTrooper);
            else if (self as MookTrooper || self as MookSuicide || self as MookRiotShield || self as ScoutMook)
                result = nameof(Mook);

            if (self.IsStrong())
                result += "_Strong";

            return result;
        }

        public static void Decapitate(this Mook mook, int damage, DamageType damageType, float xI, float yI, int direction, float xHit, float yHit, MonoBehaviour damageSender)
        {
            mook.SetFieldValue("decapitated", true);
            mook.SetFieldValue("assasinated",  false);
            mook.SetFieldValue("disemboweled", false);
            mook.canBeAssasinated = false;
            mook.health = 1;
            EffectsController.CreateBloodParticles(mook.bloodColor, mook.X, mook.Y + 16f, 10, 3f, 2f, 50f, xI * 0.5f + (float)(direction * 50), yI * 0.4f + 80f);
            mook.CreateGibs(xI, yI);
            mook.CallMethod("PlayDecapitateSound");
            mook.CallMethod("DeactivateGun");


            var tex = TextureManager.GetTexture($"{mook.GetType()}_decapitated.png");
            if (tex == null)
                tex = TextureManager.GetTexture($"{mook.GetDecapitationName()}_decapitated.png");
            if (tex != null)
                mook.SetRendererTexture(tex);

            if (UnityEngine.Random.value > 0f)
            {
                mook.Panic(UnityEngine.Random.Range(0, 2) * 2 - 1, 2.5f, false);
                mook.SetFieldValue("decapitationCounter", 0.3f + UnityEngine.Random.value * 0.4f);
            }
            else
            {
                mook.Damage(mook.health, damageType, xI, yI, direction, damageSender, xHit, yHit);
            }
            if (yI > 400f)
            {
                mook.Knock(DamageType.Knock, xI, yI, false);
                mook.yI = Mathf.Max(mook.yI, yI * 0.5f);
                mook.BackSomersault(true);
            }
            mook.CallMethod("TryAssignHeroThatKilledMe", damageSender);
        }

        public static void CreateGibs(this Mook mook, float xI, float yI, float xForce = 100f, float yForce = 100f)
        {
            var ext = mook.GetComponent<MookExtended>();
            if (ext != null)
                EffectsController.CreateGibs(ext.decapitationGib, mook.GetComponent<Renderer>().sharedMaterial, mook.X, mook.Y, xForce, yForce, xI * 0.5f, yI * 0.4f + 60f);
        }
    }
}
