using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    public enum SpriteType
    {
        None,

        FontProbity,
        FontSparge,

        Player,
    }

    class Sprite : MonoObject
    {
        public float Rotation { get; set; }
        public bool Show { get; set; }
        public SpriteType SpriteType { get; private set; }
        public SpriteEffects SpriteEffect { get; set; }
        public Vector2 RotationOffset { get; set; }
        public Vector2 Scale { get; set; }
        public Effect Effect { get; set; }
        public SamplerState SamplerState { get; set; }
        public Texture2D SpriteSheet { get; private set; }

        private Microsoft.Xna.Framework.Rectangle sourceRectangle;
        private Microsoft.Xna.Framework.Rectangle scissorRectangle;
        private bool customScissorRectangle;
        private int originalFrameX;
        private int originalFrameY;
        private int frameX;
        private int frameY;

        public Sprite(float x, float y, int frame, int columns, SpriteType sprite) : this(x, y, sprite)
        {
            SetFrame(frame, columns);
        }

        public Sprite(float x, float y, SpriteType sprite) : base(x, y, 1, 1)
        {
            Rotation = 0;
            Show = true;
            SpriteType = sprite;
            RotationOffset = Vector2.Zero;
            Scale = new Vector2(1, 1);
            SamplerState = SamplerState.PointClamp;

            InitializeSprite();
        }

        private void InitializeSprite()
        {
            switch (SpriteType)
            {
                #region Fonts
                case SpriteType.FontProbity:
                    SpriteSetup(0, 0, 8, 8, AssetManager.FontProbity);
                    break;
                case SpriteType.FontSparge:
                    SpriteSetup(0, 0, 16, 16, AssetManager.FontSparge);
                    break;
                #endregion

                case SpriteType.Player:
                    SpriteSetup(0, 0, 16, 32, AssetManager.Sprites);
                    break;

                case SpriteType.None:
                    SpriteSetup(0, 0, 0, 0, AssetManager.Sprites);
                    break;
            }
        }

        private void SpriteSetup(int frameX, int frameY, int width, int height, Texture2D spriteSheet)
        {
            this.SpriteSheet = spriteSheet;
            this.frameX = frameX;
            this.frameY = frameY;
            originalFrameX = frameX;
            originalFrameY = frameY;

            SetBounds(X, Y, width, height);
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

        public void SetSprite(SpriteType spriteType)
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

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void ManagedDraw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SpriteSheet, Position, sourceRectangle, Color, Rotation, RotationOffset, Scale, SpriteEffect, 0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, CameraType cameraType)
        {
            if (!Show)
                return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState, null, GraphicsManager.DefaultRasterizerState, Effect, CameraManager.GetCamera(cameraType).Transform);
            {
                if (customScissorRectangle)
                    spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

                spriteBatch.Draw(SpriteSheet, Position, sourceRectangle, Color, Rotation, RotationOffset, Scale, SpriteEffect, 0);
            }
            spriteBatch.End();
        }
    }
}
