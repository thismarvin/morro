using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    class AssetManager
    {
        public static Texture2D Sprites { get; private set; }

        public static Texture2D FontProbity { get; private set; }
        public static Texture2D FontSparge { get; private set; }       

        public static void LoadContent(ContentManager Content)
        {
            Sprites = Content.Load<Texture2D>("Assets/Sprites/Sprites");

            FontProbity = Content.Load<Texture2D>("Assets/Fonts/Probity");
            FontSparge = Content.Load<Texture2D>("Assets/Fonts/Sparge");

            EffectManager.LoadContent(Content);
        }

        public static void UnloadContent()
        {
            Sprites.Dispose();
            FontProbity.Dispose();
            FontSparge.Dispose();

            EffectManager.UnloadContent();
        }
    }
}
