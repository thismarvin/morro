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

        public ShapeData(List<Vector3> vertices, List<short> indices)
        {
            Geometry = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexPosition), vertices.Count, BufferUsage.WriteOnly);
            Geometry.SetData(vertices.ToArray());

            Indices = new IndexBuffer(Engine.Graphics.GraphicsDevice, typeof(short), indices.Count, BufferUsage.WriteOnly);
            Indices.SetData(indices.ToArray());
        }
    }
}
