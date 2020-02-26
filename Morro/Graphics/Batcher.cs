using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    static class Batcher
    {
        public static void DrawSprites(SpriteBatch spriteBatch, Camera camera, Sprite[] sprites)
        {
            SpriteGroup[] spriteGroups = OrganizeSprites(sprites);
            for (int i = 0; i < spriteGroups.Length; i++)
            {
                spriteGroups[i].Draw(spriteBatch, camera);
            }
        }

        public static void DrawPolygons(SpriteBatch spriteBatch, Camera camera, MPolygon[] polygons)
        {
            PolygonGroup[] polygonGroups = OrganizePolygons(polygons);
            for (int i = 0; i < polygonGroups.Length; i++)
            {
                polygonGroups[i].Draw(spriteBatch, camera);
            }
        }

        private static SpriteGroup[] OrganizeSprites(Sprite[] sprites)
        {
            List<SpriteGroup> result = new List<SpriteGroup>();
            int resultIndex = -1;
            int spriteGroupCapacity = 2048;

            for (int i = 0; i < sprites.Length; i++)
            {
                if (result.Count == 0 || !result[resultIndex].Add(sprites[i]))
                {
                    result.Add(new SpriteGroup(sprites[i].BlendState, sprites[i].SamplerState, sprites[i].Effect, spriteGroupCapacity));
                    resultIndex++;
                    result[resultIndex].Add(sprites[i]);
                }
            }

            return result.ToArray();
        }

        private static PolygonGroup[] OrganizePolygons(MPolygon[] polygons)
        {
            List<PolygonGroup> result = new List<PolygonGroup>();
            int resultIndex = -1;

            for (int i = 0; i < polygons.Length; i++)
            {
                if (result.Count == 0 || !result[resultIndex].Add(polygons[i]))
                {
                    result.Add(new PolygonGroup(polygons[i].ShapeData));
                    resultIndex++;
                    result[resultIndex].Add(polygons[i]);
                }
            }

            return result.ToArray();
        }
    }
}
