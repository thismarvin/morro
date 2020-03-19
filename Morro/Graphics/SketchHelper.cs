﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    static class SketchHelper
    {

        private static readonly SpriteBatch spriteBatch;

        static SketchHelper()
        {
            spriteBatch = GraphicsManager.SpriteBatch;
        }

        public static void ApplyGaussianBlur(RenderTarget2D renderTarget, int passes)
        {
            for (int i = 0; i < passes * 2; i++)
                Sketch.AttachEffect(new Blur(Engine.RenderTarget, i % 2 == 0 ? new Vector2(1, 0) : new Vector2(0, 1), WindowManager.Scale * 0.75f));

            Sketch.Begin();
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                {
                    spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
                }
                spriteBatch.End();

                renderTarget.Dispose();
            }
            Sketch.End();
        }
    }
}
