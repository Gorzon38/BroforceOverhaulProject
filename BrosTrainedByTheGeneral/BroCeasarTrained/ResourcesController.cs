using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using TFBGames.Systems;

namespace BroCeasarTrained
{
    public static class ResourcesController
    {
        private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

        /// <summary>
        /// Creates a Texture2D from an image or asset file.
        /// Loads Texture2D from cache if created previously.
        /// </summary>
        /// <param name="path">Path to an image or asset</param>
        /// /// <param name="fileName">Name of an image or asset file</param>
        /// <returns></returns>
        public static Texture2D GetTexture(string path, string fileName)
        {
            return GetTexture(Path.Combine(path, fileName));
        }

        /// <summary>
        /// Creates a Texture2D from an image or asset file.
        /// Loads Texture2D from cache if created previously.
        /// </summary>
        /// <param name="filePath">Path to an image or asset file</param>
        /// <returns></returns>
        public static Texture2D GetTexture(string filePath)
        {
            Texture2D tex = null;
            textures.TryGetValue(filePath, out tex);
            if (tex != null)
                return tex;

            if (File.Exists(filePath))
            {
                tex = CreateTexture(filePath);
            }
            else if (filePath.Contains(":"))
            {
                try
                {
                    tex = LoadAssetSync<Texture2D>(filePath);
                }
                catch (Exception ex)
                {
                    Main.Log(ex);
                }
            }
            else
                tex = CreateTexture(Path.Combine(Main.mod.Path, filePath));

            if (tex != null)
                textures.Add(filePath, tex);
            return tex;
        }

        /// <summary>
        /// Creates a Texture2D from an image or asset file.
        /// The Texture2D is not cached, use GetTexture if caching is desired.
        /// </summary>
        /// <param name="filePath">Path to an image file</param>
        /// <returns></returns>
        public static Texture2D CreateTexture(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            return CreateTexture(File.ReadAllBytes(filePath));
        }

        /// <summary>
        /// Creates a Texture2D from a byte array.
        /// The Texture2D is not cached, use GetTexture if caching is desired.
        /// </summary>
        /// <param name="imageBytes">Byte array to load image from</param>
        /// <returns></returns>
        public static Texture2D CreateTexture(byte[] imageBytes)
        {
            if (imageBytes.IsNullOrEmpty())
                throw new ArgumentException("Is null or empty", nameof(imageBytes));

            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.LoadImage(imageBytes);
            tex.filterMode = FilterMode.Point;
            tex.anisoLevel = 1;
            tex.mipMapBias = 0;
            tex.wrapMode = TextureWrapMode.Repeat;
            return tex;
        }

        /// <summary>
        /// Creates an AudioClip from an audio file.
        /// Loads AudioClip from cache if created previously.
        /// </summary>
        /// <param name="path">Path to an audio file</param>
        /// <param name="fileName">Name of an audio file</param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string path, string fileName)
        {
            // Get full path converts / to \ which is needed because WWW doesn't like /
            return GetAudioClip(Path.GetFullPath(Path.Combine(path, fileName)));
        }

        /// <summary>
        /// Creates an AudioClip from an audio file.
        /// Loads AudioClip from cache if created previously.
        /// </summary>
        /// <param name="filePath">Path to an audio file</param>
        /// <returns></returns>
        public static AudioClip GetAudioClip(string filePath)
        {
            AudioClip result = null;
            if (audioClips.ContainsKey(filePath))
            {
                return audioClips[filePath];
            }

            if (File.Exists(filePath))
            {
                result = CreateAudioClip(filePath);
            }
            else if (filePath.Contains(":"))
            {
                result = LoadAssetSync<AudioClip>(filePath);
            }
            else
            {
                result = CreateAudioClip(filePath);
            }

            if (result != null)
            {
                audioClips.Add(filePath, result);
            }
            return result;
        }

        /// <summary>
        /// Creates an AudioClip from an audio file.
        /// The AudioClip is not cached, use GetAudioClip is caching is desired.
        /// </summary>
        /// <param name="filePath">Path to an audio file</param>
        /// <returns></returns>
        public static AudioClip CreateAudioClip(string filePath)
        {
            WWW getClip = new WWW("file:////" + filePath);

            while ( !getClip.isDone )
            {
            };


            AudioClip result = getClip.GetAudioClip(false, true);
            result.name = Path.GetFileNameWithoutExtension(filePath);

            return result;
        }

        /// <summary>
        /// Loads an object from an asset file.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="name">Name of the asset file</param>
        /// <returns></returns>
        public static T LoadAssetSync<T>(string name) where T : UnityEngine.Object
        {
            return GameSystems.ResourceManager.LoadAssetSync<T>(name);
        }
    }
}


