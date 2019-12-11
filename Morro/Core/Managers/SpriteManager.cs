﻿using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    static class SpriteManager
    {
        private static Dictionary<string, SpriteData> spriteDataLookup;

        public static void Initialize()
        {
            spriteDataLookup = new Dictionary<string, SpriteData>();
            
            AddSpriteData("Probity", 0, 0, 8, 8, "Probity");
            AddSpriteData("Sparge", 0, 0, 16, 16, "Sparge");
        }

        public static void AddSpriteData(string name, int x, int y, int width, int height, string spriteSheet)
        {
            string formattedName = FormatName(name);
            spriteDataLookup.Add(formattedName, new SpriteData(x, y, width, height, spriteSheet));
        }

        public static SpriteData GetSpriteData(string name)
        {
            string formattedName = FormatName(name);
            if (!spriteDataLookup.ContainsKey(formattedName))
                throw new MorroException("SpriteData with that name has not been added.", new KeyNotFoundException());

            return spriteDataLookup[formattedName];
        }       

        internal static string FormatName(string name)
        {
            return name.ToLowerInvariant();
        }
    }
}
