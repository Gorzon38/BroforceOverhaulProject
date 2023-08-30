 using System;
using UnityEngine;

namespace TheGeneralsTraining
{
    internal static class ModUI
    {
        private static GUIStyle toolTipStyle = new GUIStyle();
        private static GUIStyle titleStyle = new GUIStyle();
        private static int TabSelected;
        private static string[] tabs;
        private static Action[] actions;
        private static int playerNum;
        private static Vector2 _scrollPos;

        private static Settings Sett
        {
            get
            {
                return Main.settings;
            }
        }

        public static void Init()
        {
            toolTipStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 15
            };
            toolTipStyle.normal.textColor = Color.white;

            titleStyle = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                fontSize = 13
            };
            titleStyle.normal.textColor = Color.white;


            tabs = new string[] { "Global", "Brade", "Brochete", "Bro Dredd", "Bro Hard", "Bro Heart", /*"Broniversal Soldier",*/ "Broffy", "Casey Broback", "Chev Brolios", "Desperabro", "Dirty Brorry", "Double Bro Seven", "Rambro", "Scorpion Bro", "Seth Brondle", "Xena" };
            actions = new Action[] { Global, Brade, Brochete, BroDredd, BroHard, BroHeart, /*BroniversalSoldier,*/ Buffy, CaseyBroback, ChevBrolios, Desperabro, DirtyHarry, DoubleBroSeven, Rambro, ScorpionBro, SethBrondle, Xena };
        }

        public static void OnGUI()
        {
            TabSelected = GUILayout.SelectionGrid(TabSelected, tabs, 8);

            GUILayout.Space(25);
            Rect toolTipRect = GUILayoutUtility.GetLastRect();
            GUILayout.Space(10);
            Sett.patchInCustomsLevel = GUILayout.Toggle(Sett.patchInCustomsLevel, new GUIContent("Patch in custom levels", "Do the changes are applied in Customs Level and Editor (use at your own risk)"));
            GUILayout.Space(5);

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(250));

            GUILayout.BeginVertical();
            actions[TabSelected].Invoke();
            GUILayout.EndVertical();

            GUILayout.EndScrollView();
            GUI.Label(toolTipRect, GUI.tooltip, toolTipStyle);
        }

        private static void Global()
        {
            BeginVerticalBox();
            Sett.strongerThrow = GUILayout.Toggle(Sett.strongerThrow, new GUIContent("Stronger throw", "Depends of the bro and it's state"));
            Sett.electricThrow = GUILayout.Toggle(Sett.electricThrow, new GUIContent("Electric Throw ", "Broden and The Brolander (if ammo >= 2) make mook electric when throw"));
            Sett.rememberPockettedSpecial = GUILayout.Toggle(Sett.rememberPockettedSpecial, new GUIContent("Remember Pocketed Special", "When you swap your bro alive, you keep the special ammo (Time Slow, AirStrike, Remote Control Car, Mech Drop, ect.)"));
            Sett.grenadeExplodeIfNotVisible = GUILayout.Toggle(Sett.grenadeExplodeIfNotVisible, new GUIContent("Grenade explode if not visible", "The grenades explode if they are not visible by the camera."));
            Sett.goldenFlexBrosProjectile = GUILayout.Toggle(Sett.goldenFlexBrosProjectile, new GUIContent("Bro's projectile with golden light", "Golden Light Projectile are replace by bros default projectile."));
            Sett.mechSwapToAmerica = GUILayout.Toggle(Sett.mechSwapToAmerica, new GUIContent("Repaint Mech", "Change the textures tothe american one"));
            GUILayout.EndVertical();

            BeginVerticalBox("Animation");
            Sett.ladderAnimation = GUILayout.Toggle(Sett.ladderAnimation, new GUIContent("Ladder Animation", "Animation of climbing a ladder"));
            Sett.pushAnimation = GUILayout.Toggle(Sett.pushAnimation, new GUIContent("Push Animation", "Animation of pushing a box"));
            Sett.flexIfReviveSourceFlex = GUILayout.Toggle(Sett.flexIfReviveSourceFlex, new GUIContent("Flex if revive source is flexing"));
            GUILayout.EndVertical();

            BeginVerticalBox("HUD");
            Sett.faceHugger = GUILayout.Toggle(Sett.faceHugger, new GUIContent("Face Hugger", "Show a face hugger on the hud if you have one on you."));
            Sett.multiplePockettedSpecial = GUILayout.Toggle(Sett.multiplePockettedSpecial, new GUIContent("Multiple Pocketed Special", "Show first 6 pocketed special you have take."));
            GUILayout.EndVertical();
        }

        private static void Brade()
        {
            BeginVerticalBox("Texture");
            Sett.bradeGlaive = GUILayout.Toggle(Sett.bradeGlaive, new GUIContent("Old Brade Glaive", "Put back the old brade's glaive sprite"));
            GUILayout.EndVertical();
        }
        private static void Brochete()
        {
            BeginVerticalBox("Animation");
            Sett.alternateSpecialAnim = GUILayout.Toggle(Sett.alternateSpecialAnim, new GUIContent("Alternate special animation", ""));
            GUILayout.EndVertical();
        }

        private static void BroDredd()
        {
            BeginVerticalBox("Melee");
            Sett.lessTazerHit = GUILayout.Toggle(Sett.lessTazerHit, new GUIContent("Less taser hit", "It tooks less hit for Bro Dredd to explode an enemy with his taser."));
            GUILayout.EndVertical();
        }
        private static void BroHard()
        {
            BeginVerticalBox("Movement");
            Sett.broHardFasterWhenDucking = GUILayout.Toggle(Sett.broHardFasterWhenDucking, new GUIContent("Faster in enclosed spaces", "15% faster in enclosed spaces"));
            GUILayout.EndVertical();
        }

        private static void BroHeart()
        {
            BeginVerticalBox();
            Sett.retrieveSwordInAmmo = GUILayout.Toggle(Sett.retrieveSwordInAmmo, new GUIContent("Retrieve lost sword in ammo pickups"));
            GUILayout.EndVertical();
        }

        private static void BroniversalSoldier()
        {
            BeginVerticalBox();
            GUILayout.EndVertical();
        }

        private static void Buffy()
        {
            BeginVerticalBox("Special");
            Sett.hollywaterMookToVillager = GUILayout.Toggle(Sett.hollywaterMookToVillager, new GUIContent("Mook to villager with Holy Water", "Convert basic Mooks into villager with holy water"));
            Sett.holyWaterPanicUnits = GUILayout.Toggle(Sett.holyWaterPanicUnits, new GUIContent("Better holy water", "Panic enemies and longer invulnerability"));
            GUILayout.EndVertical();
        }

        private static void CaseyBroback()
        {
            BeginVerticalBox();
            Sett.strongerMelee = GUILayout.Toggle(Sett.strongerMelee, new GUIContent("Stronger Melee"));
            Sett.pigGrenade = GUILayout.Toggle(Sett.pigGrenade, new GUIContent("Pig Grenade", "Replace the special with a pig grenade."));
            GUILayout.EndVertical();
        }
        private static void ChevBrolios()
        {
            BeginVerticalBox("Attack");
            Sett.noRecoil = GUILayout.Toggle(Sett.noRecoil, new GUIContent("No Recoil", "No recoil on shoot"));
            GUILayout.EndVertical();

            BeginVerticalBox("Special");
            Sett.carBattery = GUILayout.Toggle(Sett.carBattery, new GUIContent("Car Battery", "During adrenaline, press the special button again to use up one of his lives to infuse chev with fire and electricity"));
            GUILayout.EndVertical();
        }

        private static void Desperabro()
        {
            BeginVerticalBox();
            Sett.mariachisPlayMusic = GUILayout.Toggle(Sett.mariachisPlayMusic, new GUIContent("Mariachis Play Music", "When they have no enemies to shoot at, they play music."));
            GUILayout.EndVertical();
        }
        private static void DirtyHarry()
        {
            BeginVerticalBox("Melee");
            Sett.reloadOnPunch = GUILayout.Toggle(Sett.reloadOnPunch, new GUIContent("Reload on punch"));
            GUILayout.EndVertical();
        }

        private static void DoubleBroSeven()
        {
            BeginVerticalBox();
            Sett.drunkSeven = GUILayout.Toggle(Sett.drunkSeven, new GUIContent("Don't drink", "007 is less accurate when he has drink more than 3 cocktails"));
            GUILayout.EndVertical();

            BeginVerticalBox("Special");
            Sett.fifthBondSpecial = GUILayout.Toggle(Sett.fifthBondSpecial, new GUIContent("Fifth Bond's Special", "Add the fifth special of 007, a tear gas"));
            GUILayout.EndVertical();
        }

        private static void Rambro()
        {
            BeginVerticalBox();
            GUILayout.EndVertical();
        }

        private static void ScorpionBro()
        {
            BeginVerticalBox("Special");
            Sett.stealthier = GUILayout.Toggle(Sett.stealthier, new GUIContent("Better Special", "Can use melee in stealth mode and become invisible if player is ducking."));
            GUILayout.EndVertical();
        }

        private static void SethBrondle()
        {
            BeginVerticalBox();
            Sett.noAcidCoverIfSpecial = GUILayout.Toggle(Sett.noAcidCoverIfSpecial, new GUIContent("No acid cover if has special", "Seth Brondle will not be cover by acid if he has at least one special left."));
            Sett.flyFaster = GUILayout.Toggle(Sett.flyFaster, new GUIContent("Fly faster", ""));
            GUILayout.EndVertical();

            BeginVerticalBox("Animation");
            Sett.alternateHangingAnimation = GUILayout.Toggle(Sett.alternateHangingAnimation, new GUIContent("Alternate hanging animation", ""));
            GUILayout.EndVertical();

            BeginVerticalBox("Special");
            Sett.betterTeleportation = GUILayout.Toggle(Sett.betterTeleportation, new GUIContent("Better teleportation", ""));
            GUILayout.EndVertical();
        }

        private static void Xena()
        {
            BeginVerticalBox("Special");
            Sett.betterChakram = GUILayout.Toggle(Sett.betterChakram, new GUIContent("Better chakram"));
            GUILayout.EndVertical();
        }

        //Utility
        private static void BeginVerticalBox(string name = "")
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical(new GUIContent(name), GUI.skin.box);
            if (name.IsNotNullOrEmpty())
                GUILayout.Space(10);
        }
    }
}