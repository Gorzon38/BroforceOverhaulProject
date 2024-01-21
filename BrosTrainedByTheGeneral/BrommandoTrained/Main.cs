using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace BrommandoTrained
{
    internal static class Main
    {
        public static UnityModManager.ModEntry mod;
        public static bool enabled;
        public static Settings settings;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;

            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnToggle = OnToggle;
            settings = Settings.Load<Settings>(modEntry);

            try
            {
                var harmony = new Harmony(modEntry.Info.Id);
                var assembly = Assembly.GetExecutingAssembly();
                harmony.PatchAll(assembly);
            }
            catch (Exception ex)
            {
                mod.Logger.Log("Failed to Patch Harmony\n" + ex.ToString());
            }

            DresserMod.StorageRoom.AddSubscriber(mod.Path);

            return true;
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);

            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset Vanilla Settings To Defaults", GUILayout.ExpandWidth(false)))
                settings.vanilla = new VanillaSettings();
            if (GUILayout.Button("Reset Mod Settings To Defaults", GUILayout.ExpandWidth(false)))
                settings.mod = new TrainedSettings();
            GUILayout.EndHorizontal();
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            enabled = value;
            return true;
        }

        public static void Log(object str)
        {
            mod.Logger.Log(str.ToString());
        }

        public static Vector2Int ToVector2Int(this Vector2 vector2)
        {
            return new Vector2Int((int)vector2.x, (int)vector2.y);
        }
    }
}
