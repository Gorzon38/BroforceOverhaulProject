using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGeneralsTraining
{
    public class Mod : MonoBehaviour
    {
        public static bool CantUsePatch
        {
            get
            {
                return !CanUsePatch;
            }
        }
        public static bool CanUsePatch
        {
            get
            {
                if (Main.enabled)
                {
                    if (LevelEditorGUI.IsActive || Map.isEditing || LevelSelectionController.IsCustomCampaign())
                    {
                        return Main.settings.patchInCustomsLevel;
                    }
                    return true;
                }
                return false;
            }
        }

        public static Mod Instance;

        void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            try
            {
                ModUI.Init();
                Dresser.Intitialize();
            }
            catch (Exception e)
            {
                Main.ExceptionLog(e);
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(scene.name == LevelSelectionController.CampaignSceneDefault)
            {
                Cutscenes.Initialize();
            }
        }
    }
}
