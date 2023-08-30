using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using TFBGames.Systems;

namespace TerroristC4Programs
{
    public static class TextureManager
    {
        public static string AssetsPath
        {
            get
            {
                return Path.Combine(Main.mod.Path, directoryName);
            }
        }

        public static string directoryName = "assets";

        public static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

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
                        tex = GameSystems.ResourceManager.LoadAssetSync<Texture2D>(name);
                        textures.Add(name, tex);
                        return tex;
                    }

                    var path = Path.Combine(AssetsPath, name);
                    if (File.Exists(path))
                    {
                        tex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                        tex.LoadImage(File.ReadAllBytes(path));
                        tex.filterMode = FilterMode.Point;
                        textures.Add(name, tex);
                    }

                }
                catch(Exception e)
                {
                    Main.Log(e);
                }
            }
            return tex;
        }

        public static bool FileExist(string fileName)
        {
            return File.Exists(Path.Combine(AssetsPath, fileName));
        }
    }
}
