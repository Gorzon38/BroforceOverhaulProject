
namespace BrommandoTrained
{
    public static class Mod
    {
        public const string GUID = "com.gorzontrok.brommandotrained";
        public const string VERSION = "1.0.0";
        public const string NAME = "BrommandoTrained";

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
            get
            {
                return !CanUsePatch;
            }
        }
    }
}
