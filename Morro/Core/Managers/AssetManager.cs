using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class AssetManager
    {
        private static Dictionary<string, Texture2D> textures;
        private static Dictionary<string, Effect> effects;
        private static Dictionary<string, SoundEffect> soundEffects;

        public static void Initialize()
        {
            textures = new Dictionary<string, Texture2D>();
            effects = new Dictionary<string, Effect>();
            soundEffects = new Dictionary<string, SoundEffect>();
        }
                  
        #region Handle Images
        public static void LoadImage(string name, string path)
        {
            string formattedName = FormatName(name);
            if (textures.ContainsKey(formattedName))
                throw new MorroException("An Image with that name already exists; try a different name.", new ArgumentException("An item with the same key has already been added."));

            textures.Add(formattedName, Engine.Instance.Content.Load<Texture2D>(path));
        }

        public static Texture2D GetImage(string name)
        {
            string formattedName = FormatName(name);
            VerifyImage(formattedName);

            return textures[formattedName];
        }

        public static void RemoveImage(string name)
        {
            string formattedName = FormatName(name);
            VerifyImage(formattedName);

            textures[formattedName].Dispose();
            textures.Remove(formattedName);
        }
        #endregion

        #region Handle Effects
        public static void LoadEffect(string name, string path)
        {
            string formattedName = FormatName(name);
            if (effects.ContainsKey(formattedName))
                throw new MorroException("An Effect with that name already exists; try a different name.", new ArgumentException("An item with the same key has already been added."));

            effects.Add(formattedName, Engine.Instance.Content.Load<Effect>(path));
        }

        public static Effect GetEffect(string name)
        {
            string formattedName = FormatName(name);
            VerifyEffect(formattedName);

            return effects[formattedName];
        }

        public static void RemoveEffect(string name)
        {
            string formattedName = FormatName(name);
            VerifyEffect(formattedName);

            effects[formattedName].Dispose();
            effects.Remove(formattedName);
        }
        #endregion

        #region Handle Sound Effects
        public static void LoadSoundEffect(string name, string path)
        {
            string formattedName = FormatName(name);
            if (soundEffects.ContainsKey(formattedName))
                throw new MorroException("An Effect with that name already exists; try a different name.", new ArgumentException("An item with the same key has already been added."));

            soundEffects.Add(formattedName, Engine.Instance.Content.Load<SoundEffect>(path));
        }

        public static SoundEffect GetSoundEffect(string name)
        {
            string formattedName = FormatName(name);
            VerifySoundEffect(formattedName);

            return soundEffects[formattedName];
        }

        public static void RemoveSoundEffect(string name)
        {
            string formattedName = FormatName(name);
            VerifySoundEffect(formattedName);

            soundEffects[formattedName].Dispose();
            soundEffects.Remove(formattedName);
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

            foreach (KeyValuePair<string, SoundEffect> pair in soundEffects)
            {
                pair.Value.Dispose();
            }
        }

        private static string FormatName(string name)
        {
            return name.ToLowerInvariant();
        }

        private static void VerifyImage(string name)
        {
            if (!textures.ContainsKey(name))
                throw new MorroException("An image with that name has not been loaded.", new KeyNotFoundException());
        }

        private static void VerifyEffect(string name)
        {
            if (!effects.ContainsKey(name))
                throw new MorroException("An effect with that name has not been loaded.", new KeyNotFoundException());
        }

        private static void VerifySoundEffect(string name)
        {
            if (!soundEffects.ContainsKey(name))
                throw new MorroException("A sound effect with that name has not been loaded.", new KeyNotFoundException());
        }
    }
}
