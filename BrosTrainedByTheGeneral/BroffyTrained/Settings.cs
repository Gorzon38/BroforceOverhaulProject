using UnityEngine;
using UnityModManagerNet;
using World.Generation.MapGenV4;

namespace BroffyTrained
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

        [Header("Stab")]
        [Draw("Stab Animation Position")]
        public Vector2 stabAnimationPosition = new Vector2(25, 7);
        [Draw("Stab Animation Last Frame")]
        public int stabAnimationMaxFrame = 6;

        [Header("Kick")]
        [Draw("Kick Damage")]
        public int kickDamage = 6;
        [Draw("Kick Force")]
        public Vector2 kickForce = new Vector2(700f, 400f);
        [Draw("Kick Animation Position")]
        public Vector2 kickAnimationPosition = new Vector2(17, 6);
        [Draw("Kick Animation Last Frame")]
        public int kickAnimationMaxFrame = 9;

        [Draw("Enabled Flying Kick", DrawType.ToggleGroup)]
        public Its useFlyingKick = Its.Yes;
        [Draw(Box = true, VisibleOn = "useFlyingKick|Yes")]
        public FlyingKickSettings flyingKick = new FlyingKickSettings();

        [Draw("Holy Water Settings", Collapsible = true)]
        public HolyWaterSettings holyWater = new HolyWaterSettings();
    }

    [DrawFields(DrawFieldMask.Public)]
    public class VanillaSettings
    {
        private bool _ProcGenEnabled
        {
            get => ProcGenGameMode.UseProcGenRules;
        }
        [Header("Special")]
        [Draw("Maximum Special Ammos", DrawType.Slider, Min = 0, Max = 6)]
        public int maxAmmo = 3;

        [Header("Knife")]
        [Draw("Knife Force")]
        public Vector2 knifeForce = new Vector2(200f, 500f);

        [Header("Animation"), Space(5)]
        [Draw("Use Pushing Animation")]
        public bool usePushingAnimation = false;
        [Draw("Use Ladder Climbing Animation")]
        public bool useLadderClimbingAnimation = false;

    }

    [DrawFields(DrawFieldMask.Public)]
    public class FlyingKickSettings
    {
        [Draw("Flying Kick Animation Row")]
        public int flyingKickRow = 8;
        [Draw("Flying Kick Animation Collumn")]
        public int flyingKickCol = 0;
        [Draw("Flying Kick Last Frame")]
        public int flyingKickMaxFrame = 8;
    }

    [DrawFields(DrawFieldMask.Public)]
    public class HolyWaterSettings
    {
        [Draw("Hit Hell Units Range")]
        public float hitHellUnitsRange = 16f;
        [Draw("Revive Range")]
        public float reviveRange = 24f;
        [Draw("Revive Point Duration")]
        public float revivePointDuration = 1.7f;
        [Draw("Transform Basic Mooks into villagers")]
        public bool mookToVillager = true;
        [Draw("Transform Dogs into Pigs")]
        public bool dogsToPigs = true;
        [Draw("Transformation Range")]
        public float mookToVillagerRange = 16f;
    }
}
