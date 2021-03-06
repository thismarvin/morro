﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum StageType
    {
        Setup,
        Begin,
        End,
        Post,
    }

    static class SketchManager
    {
        private static readonly List<RenderTarget2D> renderTargets;
        private static readonly List<StageType> completedStages;
        private static FX postProcessing;

        static SketchManager()
        {
            renderTargets = new List<RenderTarget2D>();
            completedStages = new List<StageType>();
        }

        internal static void RegisterStage(StageType stage)
        {
            if (!completedStages.Contains(stage))
                completedStages.Add(stage);
        }

        public static bool VerifyQueue(params StageType[] expectedOrder)
        {
            if (expectedOrder.Length != completedStages.Count)
                return false;

            for (int i = 0; i < expectedOrder.Length; i++)
            {
                if (completedStages[i] != expectedOrder[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reset the stage queue, but leave the life cycle of the layer up to the user.
        /// </summary>
        public static void GiveUpControl()
        {
            completedStages.Clear();
        }

        public static void AddSketch(RenderTarget2D renderTarget)
        {
            renderTargets.Add(renderTarget);

            // A Sketch has been completed successfully; reset the stage queue.
            completedStages.Clear();
        }

        public static void AttachEffect(FX postProcessing)
        {
            SketchManager.postProcessing = postProcessing;
        }

        internal static void Draw(SpriteBatch spriteBatch)
        {
            if (postProcessing != null)
            {
                // Initialize a RenderTarget2D to accumulate all RenderTargets.
                RenderTarget2D accumulation = new RenderTarget2D(spriteBatch.GraphicsDevice, WindowManager.WindowWidth, WindowManager.WindowHeight);

                // Setup the GraphicsDevice with the new accumulation RenderTarget2D.
                spriteBatch.GraphicsDevice.SetRenderTarget(accumulation);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                // Draw all the saved RenderTargets onto the accumulation RenderTarget2D.
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                {
                    for (int i = 0; i < renderTargets.Count; i++)
                    {
                        spriteBatch.Draw(renderTargets[i], Vector2.Zero, Color.White);
                    }
                }
                spriteBatch.End();

                // Reset the GraphicsDevice's RenderTarget.
                spriteBatch.GraphicsDevice.SetRenderTarget(null);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                // Apply the shader.
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, postProcessing.Effect, null);
                {
                    spriteBatch.Draw(accumulation, Vector2.Zero, Color.White);
                }
                spriteBatch.End();

                // Dispose of all RenderTargets.
                accumulation.Dispose();
                for (int i = 0; i < renderTargets.Count; i++)
                {
                    renderTargets[i].Dispose();
                }
                renderTargets.Clear();

                // Dispose of shader.
                postProcessing.Dispose();
                postProcessing = null;
            }
            else
            {
                // Draw all the saved RenderTargets.
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                {
                    for (int i = 0; i < renderTargets.Count; i++)
                    {
                        spriteBatch.Draw(renderTargets[i], Vector2.Zero, Color.White);
                        //spriteBatch.Draw(
                        //    renderTargets[i],
                        //    new Vector2(WindowManager.WindowWidth / 2, WindowManager.WindowHeight / 2),
                        //    new Microsoft.Xna.Framework.Rectangle(0, 0, WindowManager.WindowWidth, WindowManager.WindowHeight),
                        //    Color.White,
                        //    0,
                        //    new Vector2(WindowManager.WindowWidth / 2, WindowManager.WindowHeight / 2),
                        //    1,
                        //    SpriteEffects.None,
                        //    0
                        //    );
                    }
                }
                spriteBatch.End();
            }

            // Dispose of all RenderTargets.
            for (int i = 0; i < renderTargets.Count; i++)
            {
                renderTargets[i].Dispose();
            }
            renderTargets.Clear();
        }
    }
}

