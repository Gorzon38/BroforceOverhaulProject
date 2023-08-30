using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TFBGames.Systems;
using UnityEngine;

namespace TheGeneralsTraining.Patches.Cutscenes
{
    [HarmonyPatch(typeof(CutsceneIntroRoot), "StartCutscene", typeof(string))]
    static class StartCutscene_Patch
    {
        static bool Prefix(CutsceneIntroRoot __instance, ref bool __result, string introName)
        {
            if (Main.CantUsePatch) return true;
            __result = false;
            try
            {
                if (!introName.StartsWith("TGT_"))
                    return true;

                __instance.SetFieldValue("_curIntroResourceName", introName);
                var _curIntroData = __instance.GetFieldValue<CutsceneIntroData>("_curIntroData");
                if (_curIntroData != null)
                {
                    __result = false;
                    return true;
                }
                if (__instance.GetFieldValue<GameObject>("cutsceneRoot") == null)
                {
                    __result = false;
                    return true;
                }

                var cutscene = TheGeneralsTraining.Cutscenes.GetTGTCutscene(introName);
                if(cutscene != null)
                {
                    __instance.CallMethod("OnLoadComplete", introName, (object)cutscene);
                    __result = true;
                    return false;
                }
                __result = false;
                return true;
            }
            catch (Exception e)
            {
                Main.ExceptionLog(e);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(CutsceneIntroRoot), "EndCutscene")]
    static class EndCutscene_Patch
    {
        static bool Prefix(CutsceneIntroRoot __instance)
        {
            if (Main.CantUsePatch) return true;

            try
            {
                if (__instance == null)
                    return true;

                var _curIntroResourceName = __instance.GetFieldValue<string>("_curIntroResourceName");
                if (_curIntroResourceName == null || !_curIntroResourceName.StartsWith("TGT"))
                    return true;

                var _curIntroData = __instance.GetFieldValue<CutsceneIntroData>("_curIntroData");
                if (_curIntroData == null)
                {
                    return false;
                }
                if(__instance.cutsceneRoot != null)
                    __instance.cutsceneRoot.SetActive(false);
                __instance.headingMesh.TextString = null;
                __instance.spriteRenderer.material.mainTexture = __instance.GetFieldValue<Texture2D>("_oldTex");
                AnimatedTexture component = __instance.spriteRenderer.gameObject.GetComponent<AnimatedTexture>();
                if (component != null)
                {
                    component.enabled = false;
                }
                __instance.anim.RemoveClip(_curIntroData.animClip);
                __instance.anim.clip = null;
                __instance.barkSource.clip = null;
                if (__instance.fanfareSource != null && _curIntroData.introFanfare != null)
                {
                    __instance.fanfareSource.clip = null;
                }
                if (__instance.subtitle1Mesh != null && !string.IsNullOrEmpty(_curIntroData.subtitle1))
                {
                    __instance.subtitle1Mesh.TextString = null;
                }
                if (__instance.subtitle2Mesh != null && !string.IsNullOrEmpty(_curIntroData.subtitle2))
                {
                    __instance.subtitle2Mesh.TextString = null;
                }

                _curIntroData = null;
                return false;
            }
            catch (Exception e)
            {
                Main.ExceptionLog(e);
            }
            return true;
        }
    }
}
