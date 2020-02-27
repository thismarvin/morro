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
        public static void DrawSprites(Sprite[] sprites, Camera camera)
        {
            SpriteCollection spriteCollection = new SpriteCollection();
            spriteCollection.SetCollection(sprites)
            .Draw(camera)
            .Clear();
        }

        public static void DrawPolygons(MPolygon[] polygons, Camera camera)
        {
            PolygonCollection polygonCollection = new PolygonCollection();
            polygonCollection.SetCollection(polygons)
            .Draw(camera)
            .Clear();
        }
    }
}
