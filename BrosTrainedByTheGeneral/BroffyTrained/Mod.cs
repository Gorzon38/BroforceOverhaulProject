using UnityEngine;

namespace BroffyTrained
{
    public static class Mod
    {
        public const string NAME = "Broffy Trained";

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

        public static bool IsOnAnimal(TestVanDammeAnim character)
        {
            LayerMask platformLayer = character.GetFieldValue<LayerMask>("platformLayer");
            RaycastHit raycastHit;
            return (Physics.Raycast(new Vector3(character.X, character.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer) ||
                Physics.Raycast(new Vector3(character.X + 4f, character.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer) ||
                Physics.Raycast(new Vector3(character.X - 4f, character.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer)) &&
                raycastHit.collider.GetComponentInParent<Animal>() != null;
        }
    }
}
