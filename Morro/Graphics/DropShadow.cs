﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class DropShadow : FX
    {
        public Texture2D Texture { get; private set; }
        public Vector2 Direction { get; private set; }
        public Color Color { get; private set; }
        public float Size { get; private set; }

        public DropShadow(Texture2D texture) : this(texture, new Vector2(1, 1), 1, new Color(Color.Black, 75))
        {

        }

        public DropShadow(Texture2D texture, Vector2 direction, float size) : this(texture, direction, size, new Color(Color.Black, 75))
        {

        }

        public DropShadow(Texture2D texture, Vector2 direction, float size, Color color ) : base(EffectType.DropShadow)
        {
            Texture = texture;
            Direction = direction;
            Color = color;
            Size = size;

            Initialize();
        }

        public void SetTexture(Texture2D texture)
        {
            Texture = texture;

            Initialize();
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;

            Initialize();
        }

        public void SetColor(Color color)
        {
            Color = color;

            Initialize();
        }

        public void SetSize(float size)
        {
            Size = size;

            Initialize();
        }

        protected override void Initialize()
        {
            Effect.Parameters["TextureWidth"].SetValue((float)Texture.Width);
            Effect.Parameters["TextureHeight"].SetValue((float)Texture.Height);
            Effect.Parameters["Direction"].SetValue(Direction);
            Effect.Parameters["Size"].SetValue(Size);
            Effect.Parameters["Color"].SetValue(Color.ToVector3());
            Effect.Parameters["Transparency"].SetValue(1 - (Color.A / 255f));
        }
    }
}
