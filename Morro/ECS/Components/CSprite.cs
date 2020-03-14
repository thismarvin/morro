using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class CSprite : IComponent
    {
        public SpriteData SpriteData { get; set; }
        public Texture2D SpriteSheet { get; set; }
        public Microsoft.Xna.Framework.Rectangle Source { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
    }

    static class CSpriteHelper
    {
        public static CSprite SetupSprite(this CSprite sprite, string name)
        {
            sprite.SpriteData = SpriteManager.GetSpriteData(name);
            sprite.SpriteSheet = AssetManager.GetImage(sprite.SpriteData.SpriteSheet);
            sprite.Source = new Microsoft.Xna.Framework.Rectangle(sprite.SpriteData.X, sprite.SpriteData.Y, sprite.SpriteData.Width, sprite.SpriteData.Height);

            return sprite;
        }
    }
}
