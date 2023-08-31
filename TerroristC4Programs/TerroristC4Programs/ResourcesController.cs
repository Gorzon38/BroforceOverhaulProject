using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using TFBGames.Systems;
using System.Xml.Linq;

namespace TerroristC4Programs
{
    public static class ResourcesController
    {
        public const string DIRECTORY_NAME = "assets";

        public static string AssetsFolder
        {
            get
            {
                return Path.Combine(Main.mod.Path, DIRECTORY_NAME);
            }
        }


        private static Dictionary<string, Material> materials = new Dictionary<string, Material>();
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public static Material GetMaterial(string resourceName)
        {
            if (resourceName.IsNullOrEmpty()) return null;

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

        public static Texture2D GetTexture(string resourceName)
        {
            if (resourceName.IsNullOrEmpty()) return null;

            Texture2D tex = null;
            textures.TryGetValue(resourceName, out tex);
            if (tex == null)
            {
                try
                {
                    if (resourceName.Contains(":"))
                    {
                        tex = LoadAssetSync<Texture2D>(resourceName);
                        textures.Add(resourceName, tex);
                        return tex;
                    }

                    var path = Path.Combine(AssetsFolder, resourceName);
                    if (File.Exists(path))
                    {
                        tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                        tex.LoadImage(File.ReadAllBytes(path));
                        tex.filterMode = FilterMode.Point;
                        textures.Add(resourceName, tex);
                    }
                }
                catch (Exception e)
                {
                    Main.Log(e);
                }
            }
            return tex;
        }

        public static T LoadAssetSync<T>(string resourceName) where T : UnityEngine.Object
        {
            return GameSystems.ResourceManager.LoadAssetSync<T>(resourceName);
        }


        public static bool FileExist(string fileName)
        {
            return File.Exists(Path.Combine(AssetsFolder, fileName));
        }
    }
}
