using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class Sprite : MonoObject
    {
        public float Rotation { get; set; }
        public bool Show { get; set; }
        public string SpriteType { get; private set; }
        public SpriteEffects SpriteEffect { get; set; }
        public Vector2 RotationOffset { get; set; }
        public Vector2 Scale { get; set; }
        public Effect Effect { get; set; }
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public Texture2D SpriteSheet { get; private set; }

        private Microsoft.Xna.Framework.Rectangle sourceRectangle;
        private Microsoft.Xna.Framework.Rectangle scissorRectangle;
        private bool customScissorRectangle;
        private int originalFrameX;
        private int originalFrameY;
        private int frameX;
        private int frameY;

        public Sprite(float x, float y, int frame, int columns, string sprite) : this(x, y, sprite)
        {
            SetFrame(frame, columns);
        }

        public Sprite(float x, float y, string sprite) : base(x, y, 1, 1)
        {
            Rotation = 0;
            Show = true;
            SpriteType = sprite;
            RotationOffset = Vector2.Zero;
            Scale = new Vector2(1, 1);
            BlendState = BlendState.NonPremultiplied;
            SamplerState = SamplerState.PointClamp;

            InitializeSprite();
        }

        private void InitializeSprite()
        {
            SpriteSetup(SpriteManager.GetSpriteData(SpriteType));
        }

        private void SpriteSetup(SpriteData spriteData)
        {
            SpriteSheet = AssetManager.GetImage(spriteData.SpriteSheet);
            frameX = spriteData.X;
            frameY = spriteData.Y;
            originalFrameX = frameX;
            originalFrameY = frameY;

            SetDimensions(spriteData.Width, spriteData.Height);

            sourceRectangle = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, Width, Height);
        }

        public void IncrementFrameX(int pixels)
        {
            frameX += pixels;
            sourceRectangle = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, Width, Height);
        }

        public void IncrementFrameY(int pixels)
        {
            frameY += pixels;
            sourceRectangle = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, Width, Height);
        }

        public void SetFrame(int frame, int columns)
        {
            frameX = originalFrameX + frame % columns * Width;
            frameY = originalFrameY + frame / columns * Height;
            sourceRectangle = new Microsoft.Xna.Framework.Rectangle(frameX, frameY, Width, Height);
        }

        public void SetSprite(string spriteType)
        {
            if (SpriteType == spriteType)
                return;

            SpriteType = spriteType;
            InitializeSprite();
        }

        public void SetScissorRectangle(Microsoft.Xna.Framework.Rectangle scissorRectangle)
        {
            this.scissorRectangle = scissorRectangle;
            customScissorRectangle = true;
        }

        public virtual void Update()
        {

        }

        public virtual void ManagedDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SpriteSheet, Position, sourceRectangle, Color, Rotation, RotationOffset, Scale, SpriteEffect, 0);
        }

        public void Draw(SpriteBatch spriteBatch, CameraType cameraType)
        {
            Draw(spriteBatch, CameraManager.GetCamera(cameraType));
        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (!Show)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState, null, GraphicsManager.DefaultRasterizerState, Effect, camera.Transform);
            {
                if (customScissorRectangle)
                    spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

                spriteBatch.Draw(SpriteSheet, Position, sourceRectangle, Color, Rotation, RotationOffset, Scale, SpriteEffect, 0);
            }
            spriteBatch.End();
        }
    }
}
