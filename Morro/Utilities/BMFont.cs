﻿using Morro.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Utilities
{
    class BMFont
    {
        public string FontFace { get; set; }
        public int Size { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }

        private readonly Dictionary<int, string> pages;
        private readonly Dictionary<int, BMFontCharacter> characters;

        public BMFont()
        {
            pages = new Dictionary<int, string>();
            characters = new Dictionary<int, BMFontCharacter>();
        }

        public void ParseInfo(string info)
        {
            string[] data = info.Split(',');

            FontFace = data[0].ToString();
            Size = int.Parse(data[1]);
            Bold = data[2] == "1" ? true : false;
            Italic = data[3] == "1" ? true : false;
        }

        public void AddPage(string page)
        {
            string[] data = page.Split(',');

            int id = int.Parse(data[0]);
            string file = data[1].Split('.')[0];

            if (!pages.ContainsKey(id))
            {
                pages.Add(id, file);

                AssetManager.LoadImage
                (
                    file,
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", "Assets/Fonts/", file)
                );
            }
        }

        public void AddCharacter(string character)
        {
            string[] data = character.Split(',');

            int id = int.Parse(data[0]);
            int x = int.Parse(data[1]);
            int y = int.Parse(data[2]);
            int width = int.Parse(data[3]);
            int height = int.Parse(data[4]);
            int xOffset = int.Parse(data[5]);
            int yOffset = int.Parse(data[6]);
            int xAdvance = int.Parse(data[7]);
            int page = int.Parse(data[8]);

            if (!characters.ContainsKey(id))
            {
                characters.Add(id, new BMFontCharacter(id, xOffset, yOffset, xAdvance));

                SpriteManager.AddSpriteData
                (
                    string.Format(CultureInfo.InvariantCulture, "{0} {1}", FontFace, id),
                    x,
                    y,
                    width,
                    height,
                    pages[page]
                );
            }
        }

        public BMFontCharacter GetCharacterData(char character)
        {
            if (!characters.ContainsKey(character))
            {
                //throw new MorroException("That character code was not included in the BMFont file.", new KeyNotFoundException());
                return new BMFontCharacter(0, 0, 0, Size);
            }

            return characters[character];
        }
    }
}
