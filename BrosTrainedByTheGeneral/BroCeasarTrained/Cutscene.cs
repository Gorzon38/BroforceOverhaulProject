using HarmonyLib;
using System;

namespace BroCeasarTrained
{
    public static class Cutscene
    {
        private static bool willLoadCutscene = false;

        public static CutsceneIntroData Clone(this CutsceneIntroData data)
        {
            var result = new CutsceneIntroData()
            {
                heading = data.heading,
                subtitle1 = data.subtitle1,
                subtitle2 = data.subtitle2,
                headingScale = data.headingScale,
                subtitleScale = data.subtitleScale,
                spriteTexture = data.spriteTexture,
                spriteSize = data.spriteSize,
                spriteRect = data.spriteRect,
                spriteAnimRateFramesWidth = data.spriteAnimRateFramesWidth,
                animClip = data.animClip,
                bark = data.bark,
                introFanfare = data.introFanfare,
            };
            return result;
        }

        public static CutsceneIntroData broCaesar;

        public static void Initialize()
        {
            broCaesar = CreateCutscene(CutsceneName.HaleTheBro);
        }

        public static CutsceneIntroData CreateCutscene(CutsceneName cutsceneName)
        {
            // Copy the cutscene data of Rambro
            var result = ResourcesController.LoadAssetSync<CutsceneIntroData>("cutscenes:Intro_Bro_Rambro").Clone();
            // Try to get the cutscene sprite for the bro.
            var tex = ResourcesController.GetTexture("BroCeasar_Cutscene.png");
            if (tex != null)
                result.spriteTexture = tex;
            result.heading = "Bro Ceasar";
            // Get audio name of the bro
            result.bark = ResourcesController.GetAudioClip("scenesshared:Bro Caesar1");

            return result;
        }


        [HarmonyPatch(typeof(CutsceneIntroRoot))]
        static class CutsceneIntroRoot_Patch
        {
            [HarmonyPatch("OnLoadComplete", typeof(string), typeof(object))]
            [HarmonyPrefix]
            static void OnLoadComplete(CutsceneIntroRoot __instance, ref string resourceName, ref object asset)
            {
                if (Cutscene.willLoadCutscene)
                {
                    asset = Cutscene.broCaesar;
                    willLoadCutscene = false;
                }
            }

            [HarmonyPatch("StartCutscene", new Type[] { typeof(CutsceneName) })]
            [HarmonyPrefix]
            static bool StartCutscene(CutsceneIntroRoot __instance, ref CutsceneName cutscene, ref bool __result)
            {
                if (!Mod.CanUsePatch)
                    return true;
                if (cutscene == CutsceneName.HaleTheBro)
                    willLoadCutscene = true;
                return true;
            }
        }

    }
}
