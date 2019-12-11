using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class SpriteGroup
    {
        public BlendState SharedBlendState { get; private set; }
        public SamplerState SharedSamplerState { get; private set; }
        public Effect SharedEffect { get; private set; }
        public Sprite[] Sprites { get; private set; }
        public int Capacity { get; private set; }

        private int spriteIndex;

        public SpriteGroup(BlendState sharedBlendState, SamplerState sharedSamplerState, Effect sharedEffect, int capacity)
        {
            SharedBlendState = sharedBlendState;
            SharedSamplerState = sharedSamplerState;
            SharedEffect = sharedEffect;
            Capacity = capacity;
            Sprites = new Sprite[Capacity];
        }

        public bool Add(Sprite sprite)
        {
            if (spriteIndex >= Capacity)
                return false;

            if (sprite.BlendState == SharedBlendState && sprite.SamplerState == SharedSamplerState && sprite.Effect == SharedEffect)
            {
                Sprites[spriteIndex++] = sprite;
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, SharedBlendState, SharedSamplerState, null, GraphicsManager.DefaultRasterizerState, SharedEffect, camera.Transform);
            for (int i = 0; i < Capacity; i++)
            {
                if (Sprites[i] != null)
                {
                    Sprites[i].ManagedDraw(spriteBatch);
                }
            }
            spriteBatch.End();
        }
    }
}
