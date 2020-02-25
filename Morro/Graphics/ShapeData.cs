using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class ShapeData
    {
        public VertexBuffer Geometry { get; private set; }
        public IndexBuffer Indices { get; private set; }
        public int TotalTriangles { get => Indices.IndexCount / 3; }

        public ShapeData(Vector3[] vertices, short[] indices)
        {
            Geometry = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPosition), vertices.Length, BufferUsage.WriteOnly);
            Geometry.SetData(vertices);

            Indices = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly);
            Indices.SetData(indices);
        }
    }
}
