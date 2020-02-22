using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum VertexInformation
    {
        /// <summary>
        /// Dynamic vertex information creates a DynamicVertexBuffer and DynamicIndexBuffer every frame.
        /// This is useful for any polygons that will constantly change every frame, but at the expense of more memory usage and frequent garbage collection.
        /// </summary>
        Dynamic,
        /// <summary>
        /// Static vertex information reuses a single VertexBuffer and IndexBuffer.
        /// This is the most efficient technique, but each buffer can only hold a finite amount of information.
        /// Make sure to only use this for polygons that will never be modified after initialization.
        /// </summary>
        Static
    }

    static class GraphicsManager
    {
        private const int MaximumVertices = 1000;
        private const int MaximumIndices = 1000;

        public static RasterizerState DefaultRasterizerState { get; private set; }
        public static RasterizerState DebugRasterizerState { get; private set; }
        public static Texture2D SimpleTexture { get; private set; }
        public static BasicEffect BasicEffect { get; private set; }
        public static VertexBuffer StaticVertexBuffer { get; private set; }
        public static IndexBuffer StaticIndexBuffer { get; private set; }
        public static bool BuffersEnabled { get; private set; }

        private static readonly List<VertexPositionColor> vertices;
        private static readonly List<short> indices;
        private static readonly Dictionary<string, int> vertexLookup;
        private static readonly Dictionary<string, int> indexLookup;

        private static int verticesIndex;
        private static int indicesIndex;
        private static bool staticVertexBufferChanged;
        private static bool staticIndexBufferChanged;

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

            BuffersEnabled = true;
            StaticVertexBuffer = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPositionColor), MaximumVertices, BufferUsage.WriteOnly);
            StaticIndexBuffer = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof(short), MaximumIndices, BufferUsage.WriteOnly);
            vertices = new List<VertexPositionColor>(MaximumVertices);
            indices = new List<short>(MaximumIndices);
            vertexLookup = new Dictionary<string, int>();
            indexLookup = new Dictionary<string, int>();
        }

        public static int GetStartVertexOf(string key)
        {
            return vertexLookup[key];
        }

        public static int GetStartIndexOf(string key)
        {
            return indexLookup[key];
        }

        public static void AddToVertexBuffer(string key, VertexPositionColor[] vertices)
        {
            if (vertices == null || vertexLookup.ContainsKey(key))
                return;

            vertexLookup.Add(key, verticesIndex);
            verticesIndex += vertices.Length;

            foreach (VertexPositionColor vertex in vertices)
            {
                staticVertexBufferChanged = true;
                GraphicsManager.vertices.Add(vertex);
            }
        }

        public static void AddToIndexBuffer(string key, short[] indices)
        {
            if (indices == null || indexLookup.ContainsKey(key))
                return;

            indexLookup.Add(key, indicesIndex);
            indicesIndex += indices.Length;

            foreach (short index in indices)
            {
                staticIndexBufferChanged = true;
                GraphicsManager.indices.Add(index);
            }
        }

        public static void UnloadContent()
        {
            SimpleTexture.Dispose();
            BasicEffect.Dispose();
            StaticVertexBuffer.Dispose();
            StaticIndexBuffer.Dispose();
        }

        private static void UpdateBuffers()
        {
            if (staticVertexBufferChanged)
            {
                StaticVertexBuffer.SetData(vertices.ToArray());
                staticVertexBufferChanged = false;
            }
            if (staticIndexBufferChanged)
            {
                StaticIndexBuffer.SetData(indices.ToArray());
                staticIndexBufferChanged = false;
            }
        }

        public static void Update()
        {
            UpdateBuffers();
        }
    }
}

