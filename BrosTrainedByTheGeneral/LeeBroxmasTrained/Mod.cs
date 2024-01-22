
namespace LeeBroxmasTrained
{
    public static class Mod
    {
        public const string GUID = "com.gorzontrok.leebroxmastrained";
        public const string VERSION = "1.0.0";
        public const string NAME = "LeeBroxmasTrained";

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
