using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    struct VertexTransform : IVertexType
    {
        public Matrix Transform { get; private set; }

        public VertexTransform(Matrix transform)
        {
            Transform = transform;
        }

        public VertexDeclaration VertexDeclaration { get { return vertexDeclaration; } }
        private static readonly VertexDeclaration vertexDeclaration;

        static VertexTransform()
        {
            vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Position, 1),
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Position, 2),
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Position, 3),
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.Position, 4)
            );
        }
    }
}
