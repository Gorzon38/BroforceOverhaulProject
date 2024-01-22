using UnityEngine;
using UnityModManagerNet;

namespace LeeBroxmasTrained
{
    public enum Its
    {
        No = 0, Yes = 1
    }

    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Mod Settings", Box = true)]
        public TrainedSettings mod = new TrainedSettings();
        [Space(10), Draw("Vanilla Settings", Collapsible = true)]
        public VanillaSettings vanilla = new VanillaSettings();

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
        public void OnChange()
        { }
    }

    [DrawFields(DrawFieldMask.Public)]
    public class TrainedSettings
    {
        [Draw("Enabled Mod in Custom Levels")]
        public bool patchInCustomsLevel = false;

        [Draw("Has Halo")]
        public bool hasHalo = false;

        [Draw("Change Knifes Texture")]
        public bool changeKnifeTexture = true;

        [Space(10), Header("Custom Melee")]
        [Draw("Enabled", DrawType.ToggleGroup)]
        public Its useCustomMelee = Its.Yes;
        [Draw("Settings", Collapsible = true, Box = true, VisibleOn = "useCustomMelee|Yes")]
        public MeleeSettings melee = new MeleeSettings();
    }

    [DrawFields(DrawFieldMask.Public)]
    public class VanillaSettings
    {
        [Header("Special")]
        [Draw("Maximum Special Ammos", DrawType.Slider, Min = 0, Max = 6)]
        public int maxAmmo = 3;

        [Header("Animation"), Space(5)]
        [Draw("Use Pushing Animation")]
        public bool usePushingAnimation = false;
        [Draw("Use Ladder Climbing Animation")]
        public bool useLadderClimbingAnimation = false;
    }

    [DrawFields(DrawFieldMask.Public)]
    public class MeleeSettings
    {
        [Header("Animation")]
        [Draw("Ground Stab Position")]
        public Vector2 groundStabAnimationPosition = new Vector2(9, 25);
        [Draw("Air Stab Position")]
        public Vector2 airStabAnimationPosition = new Vector2(10, 25);
        [Draw("Maximum Stab Frames", DrawType.Slider, Min = 0, Max = 32)]
        public int maximumStabFrame = 8;

        [Space(10), Header("Stab")]
        [Draw("Range", DrawType.Slider, Min = 0.001f, Max = 10f)]
        public float stabRange = 6f;
        [Draw("Doodad Damage")]
        public int stabDoodadDamage = 3;
        [Draw("Units Damage")]
        public int stabUnitsDamage = 4;
        [Draw("Kick Doors Range")]
        public float kickDoorsRange = 24f;
        [Draw("Hit Suicide Mooks")]
        public bool hitSuicide = true;
        [Draw("Hit Only Living Units")]
        public bool onlyLiving = true;
        [Draw("Damage Type", DrawType.PopupList)]
        public DamageType damageType = DamageType.SilencedBullet;
        [Draw("Knife Position")]
        public Vector2 knifePosition = new Vector2(7f, 8f);
    }
}
