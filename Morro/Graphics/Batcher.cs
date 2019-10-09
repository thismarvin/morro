using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    static class Batcher
    {
        public static void DrawSprites(SpriteBatch spriteBatch, List<Sprite> sprites, CameraType cameraType)
        {
            List<SpriteGroup> spriteGroups = OrganizeSprites(sprites);
            for (int i = 0; i < spriteGroups.Count; i++)
            {
                spriteGroups[i].Draw(spriteBatch, cameraType);
            }
        }

        private static List<SpriteGroup> OrganizeSprites(List<Sprite> sprites)
        {
            List<SpriteGroup> result = new List<SpriteGroup>();
            int resultIndex = -1;
            int spriteGroupCapacity = 2048;

            for (int i = 0; i < sprites.Count; i++)
            {
                if (result.Count == 0 || !result[resultIndex].Add(sprites[i]))
                {
                    result.Add(new SpriteGroup(sprites[i].SamplerState, sprites[i].Effect, spriteGroupCapacity));
                    resultIndex++;
                    result[resultIndex].Add(sprites[i]);                    
                }
            }

            return result;
        }
    }
}
