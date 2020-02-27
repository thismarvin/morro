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
                spriteGroups[i].Clear();
            }
        }

        public static void DrawPolygons(SpriteBatch spriteBatch, Camera camera, MPolygon[] polygons)
        {
            PolygonGroup[] polygonGroups = OrganizePolygons(polygons);
            for (int i = 0; i < polygonGroups.Length; i++)
            {
                polygonGroups[i].Draw(spriteBatch, camera);
                polygonGroups[i].Clear();
            }
        }

        private static SpriteGroup[] OrganizeSprites(Sprite[] sprites)
        {
            int totalGroups = (int)Math.Ceiling((float)sprites.Length / SpriteGroup.MaximumCapacity);
            SpriteGroup[] groups = new SpriteGroup[totalGroups];
            int groupIndex = -1;
            int remaining = sprites.Length;

            int capacity;
            for (int i = 0; i < sprites.Length; i++)
            {
                if (groupIndex == -1 || !groups[groupIndex].Add(sprites[i]))
                {
                    capacity = remaining / SpriteGroup.MaximumCapacity > 0 ? SpriteGroup.MaximumCapacity : remaining % SpriteGroup.MaximumCapacity;
                    remaining -= SpriteGroup.MaximumCapacity;

                    groupIndex++;
                    groups[groupIndex] = new SpriteGroup(sprites[i].BlendState, sprites[i].SamplerState, sprites[i].Effect, capacity);                   
                    groups[groupIndex].Add(sprites[i]);
                }
            }

            return groups;
        }

        private static PolygonGroup[] OrganizePolygons(MPolygon[] polygons)
        {
            int totalGroups = (int)Math.Ceiling((float)polygons.Length / PolygonGroup.MaximumCapacity);
            PolygonGroup[] groups = new PolygonGroup[totalGroups];
            int groupIndex = -1;
            int remaining = polygons.Length;

            int capacity;
            for (int i = 0; i < polygons.Length; i++)
            {
                if (groupIndex == -1 || !groups[groupIndex].Add(polygons[i]))
                {
                    capacity = remaining / PolygonGroup.MaximumCapacity > 0 ? PolygonGroup.MaximumCapacity : remaining % PolygonGroup.MaximumCapacity;
                    remaining -= PolygonGroup.MaximumCapacity;

                    groupIndex++;
                    groups[groupIndex] = new PolygonGroup(polygons[i].ShapeData, capacity);
                    groups[groupIndex].Add(polygons[i]);
                }
            }

            return groups;
        }
    }
}
