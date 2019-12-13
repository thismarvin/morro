using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Morro.Core;
using Morro.Maths;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Graphics
{
    abstract class Polygon : MonoObject
    {
        public Vector2[] TransformedVertices { get; protected set; }
        public LineSegment[] LineSegments { get; protected set; }
        public float Rotation { get; private set; }
        public float LineWidth { get; private set; }
        public bool Filled { get; protected set; }

        protected VertexInformation vertexInformation;
        protected Matrix transform;
        protected VertexPositionColor[] vertices;
        protected Vector2 rotationOffset;
        protected short[] indices;
        protected int totalTriangles;
        protected int totalIndices;
        protected int totalVertices;
        protected int initialTotalVertices;
        protected string vertexKey;
        protected string indexKey;

        protected bool transformUpdated;

        public Polygon(float x, float y, int width, int height, int vertices, float lineWidth, Color color, VertexInformation vertexInformation) : base(x, y, width, height)
        {
            if (Width <= 0 || Height <= 0)
            {
                throw new MorroException("Polygons must have dimensions greater then zero.", new ArgumentException());
            }

            LineWidth = lineWidth;
            initialTotalVertices = vertices;
            totalVertices = initialTotalVertices;
            this.vertexInformation = vertexInformation;

            if (totalVertices <= 2)
            {
                throw new MorroException("Polygons require at least three vertices.", new ArgumentException());
            }

            base.SetColor(color);
        }

        public override void SetLocation(float x, float y)
        {
            if (X == x && Y == y)
                return;

            base.SetLocation(x, y);

            CreateTransform();
        }

        public override void SetBounds(float x, float y, int width, int height)
        {
            if (X == x && Y == y && Width == width && Height == height)
                return;

            base.SetBounds(x, y, width, height);

            CreateVertices();
            SendToGraphicsManager();

            CreateTransform();
        }

        public override void SetDimensions(int width, int height)
        {
            if (Width == width && Height == height)
                return;

            base.SetDimensions(width, height);

            CreateVertices();
            SendToGraphicsManager();

            CreateTransform();
        }

        public override void SetColor(Color color)
        {
            if (Color == color)
                return;

            base.SetColor(color);

            CreateVertices();
            SendToGraphicsManager();

            CreateTransform();
        }

        public virtual void SetLineWidth(float lineWidth)
        {
            if (LineWidth == lineWidth)
                return;

            LineWidth = lineWidth;

            SetupPolygonInformation();
            CreateVertices();
            CreateIndices();

            SendToGraphicsManager();

            CreateTransform();
        }

        public virtual void SetRotationOffset(float xOffset, float yOffset)
        {
            rotationOffset = new Vector2(xOffset, yOffset);

            CreateTransform();
        }

        public virtual void SetRotation(float rotation)
        {
            Rotation = rotation;

            CreateTransform();
        }

        public virtual void Rotate(float angle)
        {
            Rotation += angle;

            CreateTransform();
        }

        protected abstract void CreateKey();
        protected abstract void CreateFilledIndices();
        protected abstract void CreateHollowIndices();
        protected abstract void CreateFilledVertices();
        protected abstract void CreateHollowVertices();

        protected virtual void SetupPolygonInformation()
        {
            Filled = LineWidth == 0 ? true : false;
            if (!Filled)
            {
                totalVertices = initialTotalVertices * 2;
                totalTriangles = initialTotalVertices * 2;
                totalIndices = totalTriangles * 3;
            }
            else
            {
                totalVertices = initialTotalVertices;
                totalTriangles = totalVertices - 2;
                totalIndices = totalTriangles * 3;
            }
        }

        protected virtual void CreateIndices()
        {
            indices = new short[totalIndices];

            if (Filled)
                CreateFilledIndices();
            else
                CreateHollowIndices();
        }

        protected virtual void CreateVertices()
        {
            vertices = new VertexPositionColor[totalVertices];

            if (Filled)
                CreateFilledVertices();
            else
                CreateHollowVertices();
        }

        protected virtual void CreateTransformedVertices()
        {
            TransformedVertices = new Vector2[totalVertices];

            for (int i = 0; i < totalVertices; i++)
            {
                TransformedVertices[i] = -Vector2.Transform(new Vector2(vertices[i].Position.X, vertices[i].Position.Y), transform);
            }
        }

        protected virtual void CreateTransformedLineSegments()
        {
            LineSegments = new LineSegment[initialTotalVertices];

            if (Filled)
                CreateFilledLineSegments();
            else
                CreateHollowLineSegments();
        }

        protected virtual void CreateFilledLineSegments()
        {
            LineSegments[0] = new LineSegment(TransformedVertices[initialTotalVertices - 1].X, TransformedVertices[initialTotalVertices - 1].Y, TransformedVertices[0].X, TransformedVertices[0].Y);

            for (int i = 1; i < initialTotalVertices; i++)
            {
                LineSegments[i] = new LineSegment(TransformedVertices[i - 1].X, TransformedVertices[i - 1].Y, TransformedVertices[i].X, TransformedVertices[i].Y);
            }
        }

        protected virtual void CreateHollowLineSegments()
        {
            LineSegments[0] = new LineSegment(TransformedVertices[initialTotalVertices - 1].X, TransformedVertices[initialTotalVertices - 1].Y, TransformedVertices[initialTotalVertices].X, TransformedVertices[initialTotalVertices].Y);

            for (int i = 1; i < initialTotalVertices; i++)
            {
                LineSegments[i] = new LineSegment(TransformedVertices[(initialTotalVertices + i) - 1].X, TransformedVertices[(initialTotalVertices + i) - 1].Y, TransformedVertices[initialTotalVertices + i].X, TransformedVertices[initialTotalVertices + i].Y);
            }
        }

        protected void SendToGraphicsManager()
        {
            CreateKey();
            if (vertexInformation == VertexInformation.Static)
            {
                GraphicsManager.AddToVertexBuffer(vertexKey, vertices);
                GraphicsManager.AddToIndexBuffer(indexKey, indices);
            }
        }

        protected virtual void CreatePolygon()
        {
            SetupPolygonInformation();
            CreateIndices();
            CreateVertices();
            SendToGraphicsManager();
            CreateTransform();
        }

        protected virtual void CreateTransform()
        {
            transform =
                // Scale unit vertices by the polygon's dimensions.
                Matrix.CreateScale(Width, Height, 1) *

                // Set the origin to the polygon's top left postition instead of the bottom right.
                Matrix.CreateTranslation(-Width, -Height, 0) *

                // Translate to the polygon's axis of rotaion, rotate the polygon, and then translate back to the top left.
                Matrix.CreateTranslation(rotationOffset.X, rotationOffset.Y, 0) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateTranslation(-rotationOffset.X, -rotationOffset.Y, 0) *

                // Translate the polygon to the inverse of its position.
                Matrix.CreateTranslation(-X, -Y, 0) *
                Matrix.Identity;

            transformUpdated = true;
            //CreateTransformedVertices();
            //CreateTransformedLineSegments();
        }

        protected virtual bool Intersects(Polygon polygonA, Polygon polygonB)
        {
            //LineSegment diagonal;

            //for (int i = 0; i < polygonA.initialTotalVertices; i++)
            //{
            //    if (polygonA.Filled)
            //        diagonal = new LineSegment(polygonA.Center.X, polygonA.Center.Y, polygonA.TransformedVertices[i].X, polygonA.TransformedVertices[i].Y);
            //    else
            //        diagonal = new LineSegment(polygonA.Center.X, polygonA.Center.Y, polygonA.TransformedVertices[i + polygonA.initialTotalVertices].X, polygonA.TransformedVertices[i + polygonA.initialTotalVertices].Y);

            //    foreach (LineSegment segment in polygonB.LineSegments)
            //    {
            //        if (diagonal.Intersects(segment))
            //        {
            //            return true;
            //        }
            //    }
            //}

            foreach (LineSegment lineSegmentA in polygonA.LineSegments)
            {
                foreach (LineSegment lineSegmentB in polygonB.LineSegments)
                {
                    if (lineSegmentA.Intersects(lineSegmentB))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool Intersects(Polygon polygon)
        {
            if (!(this is IConvex) || !(polygon is IConvex))
                return false;

            SetupForCollisionTesting();
            polygon.SetupForCollisionTesting();

            return Intersects(this, polygon) || Intersects(polygon, this);
        }

        public virtual void SetupForCollisionTesting()
        {
            if (transformUpdated)
            {
                CreateTransformedVertices();
                CreateTransformedLineSegments();
                transformUpdated = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, CameraType cameraType)
        {
            Draw(spriteBatch, CameraManager.GetCamera(cameraType));
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            GraphicsManager.BasicEffect.World = transform * camera.World;
            GraphicsManager.BasicEffect.View = camera.View;
            GraphicsManager.BasicEffect.Projection = camera.Projection;

            spriteBatch.GraphicsDevice.RasterizerState = DebugManager.ShowWireFrame ? GraphicsManager.DebugRasterizerState : GraphicsManager.DefaultRasterizerState;
            spriteBatch.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            if (GraphicsManager.BuffersEnabled)
            {
                switch (vertexInformation)
                {
                    case VertexInformation.Dynamic:
                        using (var indexBuffer = new DynamicIndexBuffer(spriteBatch.GraphicsDevice, typeof(short), indices.Length, BufferUsage.WriteOnly))
                        using (var vertexBuffer = new DynamicVertexBuffer(spriteBatch.GraphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly))
                        {
                            indexBuffer.SetData(indices);
                            vertexBuffer.SetData(vertices);
                            spriteBatch.GraphicsDevice.Indices = indexBuffer;
                            spriteBatch.GraphicsDevice.SetVertexBuffer(vertexBuffer);

                            foreach (EffectPass pass in GraphicsManager.BasicEffect.CurrentTechnique.Passes)
                            {
                                pass.Apply();
                                spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, totalTriangles, 1);
                            }
                        }
                        break;

                    case VertexInformation.Static:
                        spriteBatch.GraphicsDevice.Indices = GraphicsManager.StaticIndexBuffer;
                        spriteBatch.GraphicsDevice.SetVertexBuffer(GraphicsManager.StaticVertexBuffer);
                        foreach (EffectPass pass in GraphicsManager.BasicEffect.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, GraphicsManager.GetStartVertexOf(vertexKey), GraphicsManager.GetStartIndexOf(indexKey), totalTriangles, 1);
                        }
                        break;
                }
            }
            else
            {
                foreach (EffectPass pass in GraphicsManager.BasicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, totalTriangles);
                }
            }
        }
    }
}
