﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Debug;
using Morro.Graphics;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Utilities
{
    /// <summary>
    /// A handful of default <see cref="BMFont"/>'s included with Morro.
    /// </summary>
    public enum FontType
    {
        /// <summary>
        /// An 8px tall bitmap font.
        /// </summary>
        Probity,
        /// <summary>
        /// A 16px tall bitmap font.
        /// </summary>
        Sparge,        
    }

    class Text : MorroObject, IDisposable, IDebugable
    {
        public string Content { get; private set; }
        public Vector2 Scale { get; private set; }
        public float Rotation { get; private set; }

        private readonly BMFont font;
        private readonly BMFontShader shader;
        private Sprite[] sprites;
        private int spriteIndex;
        private Matrix transform;

        private readonly MAABB literalBounds;
        private readonly MQuad exactBounds;
        private readonly MAABB broadBounds;

        private readonly SpriteCollection spriteCollection;

        public Text(float x, float y, string content, string fontName) : base(x, y, 1, 1)
        {
            Content = content;
            font = AssetManager.GetFont(fontName);
            Scale = new Vector2(1, 1);
            transform = Matrix.Identity;

            shader = new BMFontShader(Color.White, Color.Black, Color.Transparent);

            literalBounds = new MAABB(X, Y, Width, Height) { Color = PICO8.GrassGreen };
            exactBounds = new MQuad(X, Y, Width, Height) { Color = PICO8.BloodRed };
            broadBounds = new MAABB(X, Y, Width, Height) { Color = PICO8.FleshPink };

            spriteCollection = new SpriteCollection();

            CreateText();
        }

        public Text(float x, float y, string content, FontType fontType) : this(x, y, content, $"Morro_{fontType.ToString()}")
        {

        }

        public override void SetPosition(float x, float y)
        {
            if (X == x && Y == y)
                return;

            base.SetPosition(x, y);

            UpdateText();
        }

        public void SetContent(string content)
        {
            if (Content == content)
                return;

            Content = content;

            CreateText();
        }

        public void SetScale(float xScale, float yScale)
        {
            if (Scale.X == xScale && Scale.Y == yScale)
                return;

            Scale = new Vector2(xScale, yScale);

            UpdateText();
        }

        public void SetRotation(float rotation)
        {
            if (Rotation == rotation)
                return;

            Rotation = rotation;

            transform =
                Matrix.CreateTranslation(-Center.X, -Center.Y, 0) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(Center.X, Center.Y, 0) *
                Matrix.Identity;

            UpdateText();
        }

        public void SetStyling(Color textColor, Color outlineColor, Color aaColor)
        {
            shader.SetTextColor(textColor);
            shader.SetOutlineColor(outlineColor);
            shader.SetAAColor(aaColor);
        }

        private void CreateText()
        {
            if (Content.Length <= 0)
                return;

            sprites = new Sprite[Content.Length];
            spriteIndex = 0;

            float xFinal = X;
            char character;
            BMFontCharacter characterData;

            for (int i = 0; i < Content.Length; i++)
            {
                character = Content.Substring(i, 1).ToCharArray()[0];
                characterData = font.GetCharacterData(character);

                sprites[spriteIndex++] = new Sprite(xFinal + characterData.XOffset * Scale.X, Y + characterData.YOffset * Scale.Y, font.FontFace + " " + (int)character)
                {
                    Effect = shader.Effect,
                    Scale = Scale,
                    Rotation = Rotation
                };

                xFinal += characterData.XAdvance * Scale.X;
            }

            spriteCollection.SetCollection(sprites);
            SetBounds(X, Y, (int)Math.Ceiling(xFinal - X), (int)Math.Ceiling(font.Size * Scale.Y));

            exactBounds.SetBounds(X, Y, Width, Height);
            literalBounds.SetBounds(X, Y, Width, Height);
        }

        private void UpdateText()
        {
            if (Content.Length <= 0)
                return;

            float xFinal = X;
            char character;
            BMFontCharacter characterData;
            Vector2 result;

            for (int i = 0; i < Content.Length; i++)
            {
                character = Content.Substring(i, 1).ToCharArray()[0];
                characterData = font.GetCharacterData(character);

                sprites[i].SetPosition(xFinal + characterData.XOffset * Scale.X, Y + characterData.YOffset * Scale.Y);
                sprites[i].Scale = Scale;
                sprites[i].Rotation = Rotation;

                if (Rotation != 0)
                {
                    result = Vector2.Transform(sprites[i].Position, transform);
                    sprites[i].SetPosition(result.X, result.Y);
                }

                xFinal += characterData.XAdvance * Scale.X;
            }

            spriteCollection.SetCollection(sprites);
            SetBounds(X, Y, (int)Math.Ceiling(xFinal - X), (int)Math.Ceiling(font.Size * Scale.Y));

            exactBounds.SetBounds(X, Y, Width, Height);
            literalBounds.SetBounds(X, Y, Width, Height);

            HandleRotation();
        }

        private void HandleRotation()
        {
            if (Rotation == 0)
                return;

            exactBounds.RotationOffset = new Vector2(Width / 2, Height / 2);
            exactBounds.Rotation = Rotation;

            CollisionInformation collisionInformation = exactBounds.GetCollisionInformation();

            float xMin = VertexFinder(collisionInformation.Vertices, "x", "minimum");
            float xMax = VertexFinder(collisionInformation.Vertices, "x", "maximum");
            float yMin = VertexFinder(collisionInformation.Vertices, "y", "minimum");
            float yMax = VertexFinder(collisionInformation.Vertices, "y", "maximum");
            float width = xMax - xMin;
            float height = yMax - yMin;

            broadBounds.SetBounds(xMin, yMin, width, height);
        }

        private float VertexFinder(Vector2[] vertices, string dimension, string qualifier)
        {
            float result = GetValueOf(0);

            for (int i = 1; i < vertices.Length; i++)
            {
                if (Valid(GetValueOf(i)))
                {
                    result = GetValueOf(i);
                }
            }
            return result;

            float GetValueOf(int index)
            {
                string formatted = dimension.ToLowerInvariant();

                if (formatted == "x")
                {
                    return vertices[index].X;
                }
                else if (formatted == "y")
                {
                    return vertices[index].Y;
                }
                throw new ArgumentException();
            }

            bool Valid(float value)
            {
                string formatted = qualifier.ToLowerInvariant();

                if (formatted == "minimum")
                {
                    return value < result;
                }
                else if (formatted == "maximum")
                {
                    return value > result;
                }
                throw new ArgumentException();
            }
        }

        public void Debug(SpriteBatch spriteBatch, Camera camera)
        {
            exactBounds.Draw(spriteBatch, camera);
            broadBounds.Draw(spriteBatch, camera);
            literalBounds.Draw(spriteBatch, camera);
        }

        public void Draw(Camera camera)
        {
            spriteCollection.Draw(camera);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    shader.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Text()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
