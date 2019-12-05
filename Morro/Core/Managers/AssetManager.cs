﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class AssetManager
    {
        private static Dictionary<string, Texture2D> textures;
        private static Dictionary<string, Effect> effects;

        public static void Initialize()
        {
            textures = new Dictionary<string, Texture2D>();
            effects = new Dictionary<string, Effect>();
        }

        #region Handle Images
        public static void LoadImage(string name, string path)
        {
            textures.Add(name.ToLowerInvariant(), Engine.Instance.Content.Load<Texture2D>(path));
        }

        public static Texture2D GetImage(string name)
        {
            VerifyImage(name);
            return textures[name.ToLowerInvariant()];
        }

        public static void RemoveImage(string name)
        {
            VerifyImage(name);
            textures[name.ToLowerInvariant()].Dispose();
            textures.Remove(name.ToLowerInvariant());
        }
        #endregion

        #region Handle Effects
        public static void LoadEffect(string name, string path)
        {
            effects.Add(name.ToLowerInvariant(), Engine.Instance.Content.Load<Effect>(path));
        }

        public static Effect GetEffect(string name)
        {
            VerifyEffect(name);
            return effects[name.ToLowerInvariant()];
        }

        public static void RemoveEffect(string name)
        {
            VerifyEffect(name);
            effects[name.ToLowerInvariant()].Dispose();
            effects.Remove(name.ToLowerInvariant());
        }
        #endregion

        public static void LoadContent()
        {
            LoadImage("Probity", "Assets/Fonts/Probity");
            LoadImage("Sparge", "Assets/Fonts/Sparge");

            LoadEffect("Blur", "Assets/Effects/Blur");
            LoadEffect("ChromaticAberration", "Assets/Effects/ChromaticAberration");
            LoadEffect("Dither", "Assets/Effects/Dither");
            LoadEffect("DropShadow", "Assets/Effects/DropShadow");
            LoadEffect("Grayscale", "Assets/Effects/Grayscale");
            LoadEffect("Invert", "Assets/Effects/Invert");
            LoadEffect("Outline", "Assets/Effects/Outline");
            LoadEffect("Palette", "Assets/Effects/Palette");
            LoadEffect("Quantize", "Assets/Effects/Quantize");
        }

        public static void UnloadContent()
        {
            foreach (KeyValuePair<string, Texture2D> pair in textures)
            {
                pair.Value.Dispose();
            }

            foreach (KeyValuePair<string, Effect> pair in effects)
            {
                pair.Value.Dispose();
            }
        }

        private static void VerifyImage(string name)
        {
            if (!textures.ContainsKey(name.ToLowerInvariant()))
                throw new MorroException("An image with that name has not been loaded.", new KeyNotFoundException());
        }

        private static void VerifyEffect(string name)
        {
            if (!effects.ContainsKey(name.ToLowerInvariant()))
                throw new MorroException("An effect with that name has not been loaded.", new KeyNotFoundException());
        }
    }
}
