using System.Collections.Generic;
using UnityEngine;

namespace TheGeneralsTraining
{
    public static class Cutscenes
    {
        /// <summary>
        /// Return a copy of the current <see cref="CutsceneIntroData"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Add space to a string before every Upper Case
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string AddSpaces(this string self)
        {
            bool flag = false;
            List<char> list = new List<char>();
            foreach (char c in self)
            {
                if (char.IsUpper(c))
                {
                    if (flag)
                    {
                        list.Add(' ');
                    }
                    flag = true;
                }
                list.Add(c);
            }
            self = new string(list.ToArray());
            return self;
        }

        public static CutsceneIntroData broneyRoss;
        public static CutsceneIntroData broCaesar;
        public static CutsceneIntroData bronnarJensen;
        public static CutsceneIntroData leeBroxmass;
        public static CutsceneIntroData trentBroser;
        public static CutsceneIntroData tollBroad;
        public static CutsceneIntroData broc;

        private static bool _hasInitialized = false;
        private static CutsceneIntroData _ramboCutscene;

        public static void Initialize()
        {
            if (_hasInitialized)
                return;
            _hasInitialized = true;

            // Create Cutscenes for Expendabros Bros
            broneyRoss = CreateCutscene("BroneyRoss", CutsceneName.BroneyRoss);
            broCaesar = CreateCutscene("BroCaesar", CutsceneName.HaleTheBro);
            bronnarJensen = CreateCutscene("BronnarJensen", CutsceneName.BronnarJensen);
            leeBroxmass = CreateCutscene("LeeBroxmass", CutsceneName.LeeBroxmas);
            trentBroser = CreateCutscene("TrentBroser", CutsceneName.TrentBroser);
            tollBroad = CreateCutscene("TollBroad", CutsceneName.TollBroad);
            broc = CreateCutscene("BroctorDeath", CutsceneName.Broc);
        }

        public static CutsceneIntroData CreateCutscene(string name, CutsceneName cutsceneName)
        {
            if(_ramboCutscene == null)
                _ramboCutscene = ResourcesController.LoadAssetSync<CutsceneIntroData>("cutscenes:Intro_Bro_Rambro");

            // Copy the cutscene data of Rambro
            var result = _ramboCutscene.Clone();
            // Try to get the cutscene sprite for the bro.
            var tex = ResourcesController.GetTexture($"{name}_Cutscene.png");
            if (tex != null)
                result.spriteTexture = tex;
            result.heading = name.AddSpaces();
            // Get audio name of the bro
            result.bark = ResourcesController.LoadAssetSync<AudioClip>(AudioClipName(cutsceneName));

            // Add the custom cutscene to the global cutscene Dictionary
            var cutscenes = GetCutscenes();
            cutscenes[cutsceneName] = $"TGT_{name}";

            return result;
        }

        public static CutsceneIntroData GetTGTCutscene(string name)
        {
            switch (name)
            {
                case "TGT_BroneyRoss":
                    return broneyRoss;
                case "TGT_BroCaesar":
                    return broCaesar;
                case "TGT_BronnarJensen":
                    return bronnarJensen;
                case "TGT_LeeBroxmass":
                    return leeBroxmass;
                case "TGT_TrentBroser":
                    return trentBroser;
                case "TGT_TollBroad":
                    return tollBroad;
                case "TGT_BroctorDeath":
                    return broc;

                default:
                    return null;
            }
        }

        public static Dictionary<CutsceneName, string> GetCutscenes()
        {
            return CutsceneController.Instance.broIntroRoot.GetFieldValue<Dictionary<CutsceneName, string>>("_cutscenes");
        }

        private static string AudioClipName(CutsceneName cutscene)
        {
            switch (cutscene)
            {
                case CutsceneName.BroneyRoss:
                    return "scenesshared:Broney Ross" + UnityEngine.Random.Range(1, 4);
                case CutsceneName.BronnarJensen:
                    return "scenesshared:Bronar Jenson" + UnityEngine.Random.Range(1, 5);
                case CutsceneName.TrentBroser:
                    return "scenesshared:Trent Broser" + UnityEngine.Random.Range(1, 5);
                case CutsceneName.LeeBroxmas:
                    return "scenesshared:Lee Brosmas" + UnityEngine.Random.Range(1, 3);
                case CutsceneName.TollBroad:
                    return "scenesshared:Toll Broad" + UnityEngine.Random.Range(1, 3);
                case CutsceneName.Broc:
                    return "scenesshared:Broctor Death4";
                case CutsceneName.HaleTheBro:
                    return "scenesshared:Bro Caesar1";
                default:
                    return string.Empty;
            }

        }
    }
}
