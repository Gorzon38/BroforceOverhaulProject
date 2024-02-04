using UnityEngine;
using UnityEngine.SceneManagement;

namespace BroCeasarTrained
{
    public class Mod : MonoBehaviour
    {
        public const string GUID = "com.gorzontrok.broceasartrained";
        public const string VERSION = "1.0.1";
        public const string NAME = "BroCeasarTrained";

        public static bool CanUsePatch
        {
            get
            {
                if (Main.enabled)
                {
                    if (LevelEditorGUI.IsActive || Map.isEditing || LevelSelectionController.IsCustomCampaign())
                    {
                        return Main.settings.mod.patchInCustomsLevel;
                    }
                    return true;
                }
                return false;
            }
        }
        public static bool CantUsePatch
        {
            get => !CanUsePatch;
        }
        void Awake()
        {
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == LevelSelectionController.CampaignSceneDefault)
            {
                Cutscene.Initialize();
            }
        }
    }
}
