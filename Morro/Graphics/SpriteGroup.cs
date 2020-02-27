using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class SpriteGroup : DrawGroup<Sprite>
    {
        private readonly BlendState sharedBlendState;
        private readonly SamplerState sharedSamplerState;
        private readonly Effect sharedEffect;

        static readonly SpriteBatch spriteBatch;
        static SpriteGroup()
        {
            spriteBatch = SpriteManager.SpriteBatch;
        }

        public SpriteGroup(BlendState sharedBlendState, SamplerState sharedSamplerState, Effect sharedEffect, int capacity) : base(capacity)
        {
            this.sharedBlendState = sharedBlendState;
            this.sharedSamplerState = sharedSamplerState;
            this.sharedEffect = sharedEffect;
        }

        protected override bool ConditionToAdd(Sprite sprite)
        {
            return sprite.BlendState == sharedBlendState && sprite.SamplerState == sharedSamplerState && sprite.Effect == sharedEffect;
        }

        public override void Draw(Camera camera)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, sharedBlendState, sharedSamplerState, null, GraphicsManager.DefaultRasterizerState, sharedEffect, camera.Transform);
            {
                for (int i = 0; i < groupIndex; i++)
                {
                    group[i]?.ManagedDraw();
                }
            }
            spriteBatch.End();
        }
    }
}
