using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class SpriteGroup
    {
        public SamplerState SharedSamplerState { get; private set; }
        public Effect SharedEffect { get; private set; }
        public Sprite[] Sprites { get; private set; }
        public int Capacity { get; private set; }

        private int spriteIndex;

        public SpriteGroup(SamplerState sharedSamplerState, Effect sharedEffect, int capacity)
        {
            SharedSamplerState = sharedSamplerState;
            SharedEffect = sharedEffect;
            Capacity = capacity;
            Sprites = new Sprite[Capacity];
        }

        public bool Add(Sprite sprite)
        {
            if (spriteIndex >= Capacity)
                return false;

            if (sprite.SamplerState == SharedSamplerState && sprite.Effect == SharedEffect)
            {
                Sprites[spriteIndex++] = sprite;
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch, CameraType cameraType)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SharedSamplerState, null, GraphicsManager.DefaultRasterizerState, SharedEffect, CameraManager.GetCamera(cameraType).Transform);
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
