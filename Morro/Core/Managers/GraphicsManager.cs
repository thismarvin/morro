using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    static class GraphicsManager
    {
        public static RasterizerState DefaultRasterizerState { get; private set; }
        public static RasterizerState DebugRasterizerState { get; private set; }
        public static Texture2D SimpleTexture { get; private set; }
        public static BasicEffect BasicEffect { get; private set; }

        static GraphicsManager()
        {
            DefaultRasterizerState = new RasterizerState
            {
                FillMode = FillMode.Solid,
                ScissorTestEnable = true
            };
            DebugRasterizerState = new RasterizerState
            {
                FillMode = FillMode.WireFrame,
                ScissorTestEnable = true
            };

            SimpleTexture = new Texture2D(Engine.Graphics.GraphicsDevice, 1, 1);
            SimpleTexture.SetData(new[] { Color.White });

            BasicEffect = new BasicEffect(Engine.Graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
            };
        }

        public static void UnloadContent()
        {
            SimpleTexture.Dispose();
            BasicEffect.Dispose();
        }
    }
}

