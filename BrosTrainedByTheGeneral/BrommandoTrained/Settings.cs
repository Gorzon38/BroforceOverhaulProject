using UnityEngine;
using UnityModManagerNet;
using World.Generation.MapGenV4;

namespace BrommandoTrained
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

        [Draw("Enable New Barrage")]
        public bool useNewBarage = true;

        [Space(10), Header("Custom Melee")]
        [Draw("Enabled", DrawType.ToggleGroup)]
        public Its useCustomMelee = Its.Yes;
        [Draw("Settings", Collapsible = true, Box = true, VisibleOn = "useCustomMelee|Yes")]
        public MeleeSettings melee = new MeleeSettings();

        [Space(10), Header("Fire At Feet Capacity")]
        [Draw("Enabled", DrawType.ToggleGroup)]
        public Its fireProjectileAtFeetOnDown = Its.Yes;
        [Draw("Projectile Spawn Offset", VisibleOn = "fireProjectileAtFeetOnDown|Yes")]
        public Vector2 projectileSpawnOffsetFeet = new Vector2(0f, 0f);
        [Draw("Projectile Speed", VisibleOn = "fireProjectileAtFeetOnDown|Yes")]
        public Vector2 projectileSpeedAtFeet = new Vector2(0f, -450f);
        [Draw("Upward Blast", VisibleOn = "fireProjectileAtFeetOnDown|Yes")]
        public float fireAtFeetYBlast = 350f;
        [Draw("Animation Position", VisibleOn = "fireProjectileAtFeetOnDown|Yes")]
        public Vector2 shootAtFeetAnimationPosition = new Vector2(11, 24);
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
        [Draw("Maximum Rocket")]
        public int barrageMax = 4;
        [Draw("Rocket Drunk Speed")]
        public float rocketDrunkSpeed = 10f;

        [Header("Animation"), Space(5)]
        [Draw("Use Pushing Animation")]
        public bool usePushingAnimation = false;
        [Draw("Use Ladder Climbing Animation")]
        public bool useLadderClimbingAnimation = false;

        [Header("Weapon")]
        [Draw("Disturb Wild Life Range", DrawType.Slider, Min = 0, Max = 500)]
        public float disturbWildLifeRange = 80f;
        [Draw("Fire Delay")]
        public float fireDelay = 0.6f;
        [Draw("Fire Delay in ProcGen", VisibleOn = "#_ProcGenEnabled|True")]
        public float fireDelayProcGen = 0.75f;
        [Draw("Animation Shoot Starting Frame")]
        public int gunShootColumnStartingFrame = 3;
        [Draw("Projectile Spawn Offset")]
        public Vector2 projectileSpawnOffset = new Vector2(12f, 15f);
        [Draw("Projectile Speed")]
        public Vector2 projectileSpeed = new Vector2(300f, 0f);
    }

    [DrawFields(DrawFieldMask.Public)]
    public class MeleeSettings
    {
        [Header("Punch")]
        [Header("Animation")]
        [Draw("First Punch Animation Position")]
        public Vector2 firstPunchAnimationPosition = new Vector2(9, 24);
        [Draw("Second Punch Animation Position")]
        public Vector2 secondPunchAnimationPosition = new Vector2(10, 24);
        [Draw("Maximum Punch Frame", DrawType.Slider, Min = 0, Max = 32)]
        public int maximumPunchFrame = 8;
        [Space(10), Header("Smashing")]
        [Draw("Smashing Animation Position")]
        public Vector2 smashingAnimationPosition = new Vector2(11, 24);
        [Draw("Maximum Smashing Frame", DrawType.Slider, Min = 0, Max = 32)]
        public int maximumSmashingFrame = 8;
        [Draw("Last Smashing Frame In The Air", DrawType.Slider, Min = 0, Max = 32)]
        public int lastSmashingAirFrame = 4;

        [Space(10), Header("Punch")]
        [Draw("Range", DrawType.Slider, Min = 0.001f, Max = 10f)]
        public float punchRange = 6f;
        [Draw("Doodad Damage")]
        public int punchDoodadDamage = 3;
        [Draw("Units Damage")]
        public int punchUnitsDamage = 4;
        [Draw("Unit Punched Velocity")]
        public Vector2 unitPunchedVelocity = new Vector2(10f, 750f);

        [Space(10), Header("Smashing")]
        [Draw("Search Range")]
        public Vector2 searchSmashingRange = new Vector2(24f, 128f);
        [Draw("Falling Speed")]
        public float smashingFallingSpeed = 1100f;
        [Draw("Damages")]
        public int smashingDamages = 10;
        [Draw("Damages Range")]
        public float smashingDamageRange = 64f;
        [Draw("Kill Range")]
        public float smashingKillRange = 20f;
        [Draw("Ground Wave Range")]
        public float groundWaveRange = 80f;
        [Draw("Disturb Wild Life Range")]
        public float smashingDisturbWildLifeRange = 48f;
        [Draw("Shake Trees Range")]
        public Vector2 smashingShakeTreesRange = new Vector2(64f, 32f);
        [Draw("Shake Trees Force")]
        public float smashingShakeTreesForce = 64f;
    }
}
