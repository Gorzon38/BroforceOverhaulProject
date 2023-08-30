using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TFBGames.Systems;
using UnityEngine;

namespace TheGeneralsTraining
{
    public static class Cutscenes
    {
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
            if (_hasInitialized) return;


            broneyRoss = CreateCutscene("BroneyRoss", CutsceneName.BroneyRoss);
            broCaesar = CreateCutscene("BroCaesar", CutsceneName.HaleTheBro);
            bronnarJensen = CreateCutscene("BronnarJensen", CutsceneName.BronnarJensen);
            leeBroxmass = CreateCutscene("LeeBroxmass", CutsceneName.LeeBroxmas);
            trentBroser = CreateCutscene("TrentBroser", CutsceneName.TrentBroser);
            tollBroad = CreateCutscene("TollBroad", CutsceneName.TollBroad);
            broc = CreateCutscene("Broctor Death", CutsceneName.Broc);

            //_hasInitialized = true;
        }

        public static CutsceneIntroData CreateCutscene(string name, CutsceneName cutsceneName)
        {
            if(_ramboCutscene == null)
                _ramboCutscene = ResourcesController.LoadAssetSync<CutsceneIntroData>("cutscenes:Intro_Bro_Rambro");

            var result = _ramboCutscene.Clone();
            var tex = ResourcesController.GetTexture($"{name}_Cutscene.png");
            if (tex != null)
                result.spriteTexture = tex;
            result.heading = name.AddSpaces();
            result.bark = ResourcesController.LoadAssetSync<AudioClip>(AudioClipName(cutsceneName));
            //result.spriteSize *= 0.7f;

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
                case "TGT_Broc":
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
