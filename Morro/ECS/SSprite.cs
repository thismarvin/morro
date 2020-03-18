﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class SSprite : DrawSystem
    {
        private IComponent[] sprites;
        private IComponent[] positions;
        private IComponent[] transforms;

        private static readonly SpriteBatch spriteBatch;

        static SSprite()
        {
            spriteBatch = SpriteManager.SpriteBatch;
        }

        public SSprite(Scene scene) : base(scene)
        {
            Require(typeof(CSprite), typeof(CPosition), typeof(CTransform));
        }

        public override void DrawEntity(int entity, Camera camera)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Camera camera)
        {
            sprites = scene.GetData<CSprite>();
            positions = scene.GetData<CPosition>();
            transforms = scene.GetData<CTransform>();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.Transform);
            {
                CSprite sprite;
                CPosition position;
                CTransform transform;

                foreach (int entity in Entities)
                {
                    sprite = (CSprite)sprites[entity];
                    position = (CPosition)positions[entity];
                    transform = (CTransform)transforms[entity];

                    spriteBatch.Draw(sprite.SpriteSheet, new Vector2(position.X, position.Y), sprite.Source, Color.White, transform.Rotation, transform.RotationOffset, new Vector2(transform.Scale.X, transform.Scale.Y), sprite.SpriteEffect, 0);
                }
            }
            spriteBatch.End();
        }
    }
}
