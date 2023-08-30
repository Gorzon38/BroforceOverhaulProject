using System;
using System.Collections.Generic;
using System.IO;
using TFBGames.Systems;
using UnityEngine;

namespace TheGeneralsTraining
{
    public static class ResourcesController
    {
        public static string AssetsFolder
        {
            get
            {
                return Path.Combine(Main.mod.Path, "assets");
            }
        }

        private static Dictionary<string, Material> materials = new Dictionary<string, Material>();
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static Material GetMaterial(string resourceName)
        {
            Material result = null;
            if (materials.ContainsKey(resourceName))
            {
                return materials[resourceName];
            }
            else
            {
                if (resourceName.Contains(":"))
                {
                    result = LoadAssetSync<Material>(resourceName);
                }

                if (result != null)
                {
                    materials.Add(resourceName, result);
                }
            }
            return result;
        }

        public static Texture2D GetTexture(string name)
        {
            Texture2D tex = null;
            textures.TryGetValue(name, out tex);
            if (tex == null)
            {
                try
                {
                    if (name.Contains(":"))
                    {
                        tex = LoadAssetSync<Texture2D>(name);
                        textures.Add(name, tex);
                        return tex;
                    }

                    var path = GetFilePath(name);
                    if (File.Exists(path))
                    {
                        tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                        tex.LoadImage(File.ReadAllBytes(path));
                        tex.filterMode = FilterMode.Point;
                        textures.Add(name, tex);
                    }
                }
                catch (Exception e)
                {
                    Main.Log(e);
                }
            }
            return tex;
        }

        public static T LoadAssetSync<T>(string name) where T : UnityEngine.Object
        {
            return GameSystems.ResourceManager.LoadAssetSync<T>(name);
        }

        private static string GetFilePath(string imageName)
        {
            return Path.Combine(AssetsFolder, imageName);
        }
    }
}
