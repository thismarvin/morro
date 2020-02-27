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
    class MPolygon
    {
        #region Properties
        public float X
        {
            get => x;
            set
            {
                x = value;
                UpdateTransform();
            }
        }
        public float Y
        {
            get => y;
            set
            {
                y = value;
                UpdateTransform();
            }
        }
        public float Width
        {
            get => width;
            set
            {
                width = value;
                UpdateTransform();
            }
        }
        public float Height
        {
            get => height;
            set
            {
                height = value;
                UpdateTransform();
            }
        }
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                UpdateTechnique();
            }
        }
        public string Shape
        {
            get => shape;
            set
            {
                shape = value;
                UpdateShape();
            }
        }

        public virtual float Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                UpdateTransform();
            }
        }

        public Vector3 RotationOffset
        {
            get => rotationOffset;
            set
            {
                rotationOffset = value;
                UpdateTransform();
            }
        }

        public Vector3 Translation
        {
            get => translation;
            set
            {
                translation = value;
                UpdateTransform();
            }
        }

        public Vector3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                UpdateTransform();
            }
        }

        public ShapeData ShapeData
        {
            get => shapeData;
            set
            {
                if (shapeData != null && !shapeData.Managed)
                {
                    shapeData.Dispose();
                }

                shapeData = value;
                dataChanged = true;
            }
        }
        #endregion

        public Matrix Transform { get; private set; }

        public Vector2 Position { get => new Vector2(X, Y); }
        public Vector2 Center { get => new Vector2(X + Width / 2, Y + Height / 2); }
        public Core.Rectangle Bounds { get => new Core.Rectangle(X, Y, Width, Height); }

        private float x;
        private float y;
        private float width;
        private float height;
        private Color color;
        private string shape;
        private float rotation;
        private Vector3 rotationOffset;
        private Vector3 translation;
        private Vector3 scale;
        private ShapeData shapeData;

        private bool dataChanged;
        protected DynamicVertexBuffer transformBuffer;
        protected VertexBufferBinding[] vertexBufferBindings;
        protected int techniqueIndex;

        protected static readonly Effect polygonShader;

        static MPolygon()
        {
            polygonShader = AssetManager.GetEffect("PolygonShader").Clone();
        }

        public MPolygon(float x, float y, float width, float height, string shape)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.shape = shape;
            color = Color.White;
            rotation = 0;
            rotationOffset = Vector3.Zero;
            translation = Vector3.Zero;
            scale = new Vector3(1);

            UpdateTransform();
            UpdateTechnique();

            if (shape != "Morro_None")
            {
                UpdateShape();
            }
        }

        public MPolygon(float x, float y, float width, float height, ShapeType shape) : this(x, y, width, height, $"Morro_{shape.ToString()}")
        {

        }

        public virtual void SetPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
            UpdateTransform();
        }

        public virtual void SetCenter(float x, float y)
        {
            SetPosition(x - Width / 2, y - Height / 2);
        }

        public virtual void SetBounds(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            UpdateTransform();
        }

        public void SetShape(ShapeType shapeType)
        {
            Shape = $"Morro_{shapeType}";
        }

        private void UpdateShape()
        {
            dataChanged = true;
            shapeData = GeometryManager.GetShapeData(Shape);
        }

        private void UpdateTransform()
        {
            dataChanged = true;
            Transform =
                Matrix.CreateScale(Width, Height, 1) *
                Matrix.CreateScale(Scale) *

                Matrix.CreateTranslation(-rotationOffset) *
                Matrix.CreateRotationZ(rotation) *
                Matrix.CreateTranslation(rotationOffset) *

                Matrix.CreateTranslation(X, Y, 0) *
                Matrix.CreateTranslation(translation) *

                Matrix.Identity;
        }

        private void UpdateTechnique()
        {
            dataChanged = true;
            techniqueIndex = Color == Color.White ? 0 : 1;
            polygonShader.CurrentTechnique = polygonShader.Techniques[techniqueIndex];
        }

        private void UpdateBuffer()
        {
            transformBuffer?.Dispose();

            switch (techniqueIndex)
            {
                case 0:
                    transformBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransform), 1, BufferUsage.WriteOnly);
                    transformBuffer.SetData(new VertexTransform[] { new VertexTransform(Transform) });
                    break;

                case 1:
                    transformBuffer = new DynamicVertexBuffer(Engine.Graphics.GraphicsDevice, typeof(VertexTransformColor), 1, BufferUsage.WriteOnly);
                    transformBuffer.SetData(new VertexTransformColor[] { new VertexTransformColor(Transform, Color) });
                    break;
            }

            vertexBufferBindings = new VertexBufferBinding[]
            {
                new VertexBufferBinding(ShapeData.Geometry),
                new VertexBufferBinding(transformBuffer, 0, 1)
            };
        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (dataChanged)
            {
                UpdateBuffer();
                dataChanged = false;
            }

            spriteBatch.GraphicsDevice.SetVertexBuffers(vertexBufferBindings);
            spriteBatch.GraphicsDevice.Indices = ShapeData.Indices;

            spriteBatch.GraphicsDevice.RasterizerState = DebugManager.ShowWireFrame ? GraphicsManager.DebugRasterizerState : GraphicsManager.DefaultRasterizerState;
            spriteBatch.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            polygonShader.Parameters["WorldViewProjection"].SetValue(camera.World * camera.View * camera.Projection);

            foreach (EffectPass pass in polygonShader.Techniques[techniqueIndex].Passes)
            {
                pass.Apply();
                spriteBatch.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, ShapeData.TotalTriangles, 1);
            }
        }
    }
}
