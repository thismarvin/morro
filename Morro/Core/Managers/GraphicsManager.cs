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
        public static RasterizerState ScissorRasterizerState { get; private set; }
        public static RasterizerState DebugRasterizerState { get; private set; }
        public static Texture2D SimpleTexture { get; private set; }
        public static BasicEffect BasicEffect { get; private set; }

        public static RasterizerState RasterizerState { get => DebugManager.ShowWireFrame ? DebugRasterizerState : DefaultRasterizerState; }

        static GraphicsManager()
        {
            DefaultRasterizerState = new RasterizerState();
            ScissorRasterizerState = new RasterizerState() { ScissorTestEnable = true };
            DebugRasterizerState = new RasterizerState
            {
                FillMode = FillMode.WireFrame,
                CullMode = CullMode.None
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
            DefaultRasterizerState.Dispose();
            ScissorRasterizerState.Dispose();
            DebugRasterizerState.Dispose();
        }
    }
}

