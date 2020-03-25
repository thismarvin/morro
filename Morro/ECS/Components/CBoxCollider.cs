using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.Graphics;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    struct CBoxCollider : IComponent
    {
 
    }

    static class CBoxColliderHelper
    {
        public static ShapeData ShapeData { get; private set; }

        static CBoxColliderHelper()
        {
            ShapeData = GeometryManager.GetShapeData(ShapeType.Square);
        }

        public static CollisionInformation GetCollisionInformation(this CBoxCollider collider, CPosition position, CDimension dimension, CTransform transform)
        {
            Matrix vertexTransform = CreateVertexTransform();
            Vector2[] vertices = CreateVertices();
            LineSegment[] lineSegments = CreateLineSegments();

            return new CollisionInformation(vertices, lineSegments);

            Matrix CreateVertexTransform()
            {
                return
                    Matrix.CreateScale(dimension.Width * transform.Scale.X, dimension.Height * transform.Scale.Y, 1 * transform.Scale.Z) *

                   Matrix.CreateTranslation(-new Vector3(transform.RotationOffset.X, transform.RotationOffset.Y, 0)) *
                   Matrix.CreateRotationZ(transform.Rotation) *

                   Matrix.CreateTranslation(position.X + transform.Translation.X + transform.RotationOffset.X, position.Y + transform.Translation.Y + transform.RotationOffset.Y, transform.Translation.Z) *

                   Matrix.Identity;
            }

            Vector2[] CreateVertices()
            {
                Vector2[] result = new Vector2[ShapeData.TotalVertices];

                for (int i = 0; i < ShapeData.TotalVertices; i++)
                {
                    result[i] = Vector2.Transform(new Vector2(ShapeData.Vertices[i].X, ShapeData.Vertices[i].Y), vertexTransform);
                }

                return result;
            }

            LineSegment[] CreateLineSegments()
            {
                int totalVertices = ShapeData.TotalVertices;
                LineSegment[] result = new LineSegment[totalVertices];

                result[0] = new LineSegment(vertices[totalVertices - 1].X, vertices[totalVertices - 1].Y, vertices[0].X, vertices[0].Y);

                for (int i = 1; i < totalVertices; i++)
                {
                    result[i] = new LineSegment(vertices[i - 1].X, vertices[i - 1].Y, vertices[i].X, vertices[i].Y);
                }

                return result;
            }
        }
    }
}
