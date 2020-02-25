using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    struct VertexTransformColor : IVertexType
    {
        public Matrix Transform { get; private set; }
        public Color Color { get; private set; }

        public VertexTransformColor(Matrix transform, Color color)
        {
            Transform = transform;
            Color = color;
        }

        public VertexDeclaration VertexDeclaration { get { return vertexDeclaration; } }
        private static readonly VertexDeclaration vertexDeclaration;

        static VertexTransformColor()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Position, 2),
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Position, 3),
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Position, 4),
                new VertexElement(64, VertexElementFormat.Color, VertexElementUsage.Color, 5)
            );
        }
    }
}
