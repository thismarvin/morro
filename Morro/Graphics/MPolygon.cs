using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    class MPolygon
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Shape { get; private set; }

        public Vector2 Position { get { return new Vector2(X, Y); } }
        public Core.Rectangle Bounds { get { return new Core.Rectangle(X, Y, Width, Height); } }

        public ShapeData ShapeData { get; private set; }
        public Matrix Transform { get; private set; }

        private VertexBuffer transformBuffer;
        private VertexBufferBinding[] vertexBufferBindings;

        private static readonly Effect polygonShader;

        static MPolygon()
        {
            polygonShader = AssetManager.GetEffect("PolygonShader").Clone();
        }

        public MPolygon(float x, float y, float width, float height, string shape)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Shape = shape;

            ShapeData = Geometry.Shapes.Get(Shape);

            UpdateTransform();
        }

        public MPolygon(float x, float y, float width, float height, ShapeType shape) : this(x, y, width, height, $"Morro_{shape.ToString()}")
        {

        }

        private void UpdateTransform()
        {
            Transform =
                Matrix.CreateScale(Width, Height, 1) *
                Matrix.CreateTranslation(X, Y, 0) *
                Matrix.Identity;

            transformBuffer = new VertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransform), 1, BufferUsage.WriteOnly);
            transformBuffer.SetData(new VertexTransform[] { new VertexTransform(Transform) });

            vertexBufferBindings = new VertexBufferBinding[] {
                new VertexBufferBinding(ShapeData.Geometry),
                new VertexBufferBinding(transformBuffer, 0, 1)
            };
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            spriteBatch.GraphicsDevice.SetVertexBuffers(vertexBufferBindings);
            spriteBatch.GraphicsDevice.Indices = ShapeData.Indices;

            polygonShader.Parameters["WorldViewProjection"].SetValue(camera.World * camera.View * camera.Projection);

            foreach (EffectPass pass in polygonShader.CurrentTechnique.Passes)
            {
                pass.Apply();
                spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, ShapeData.TotalTriangles, 1);
            }
        }
    }
}
