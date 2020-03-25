using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.Maths;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    class SCollision : UpdateSystem
    {
        private IComponent[] positions;
        private IComponent[] dimensions;
        private IComponent[] transforms;
        private IComponent[] kinetics;
        private IComponent[] colliders;

        private SBinPartitioner binPartitioner;

        private readonly int queryBuffer;
        private readonly float leeway;

        public SCollision(Scene scene, uint tasks, int targetFPS) : base(scene, tasks, targetFPS)
        {
            Require(typeof(CPosition), typeof(CDimension), typeof(CTransform), typeof(CKinetic), typeof(CBoxCollider));
            Depend(typeof(SPhysics), typeof(SBinPartitioner));

            queryBuffer = 4;
            leeway = 0.01f;
        }

        public override void UpdateEntity(int entity)
        {
            CPosition position = (CPosition)positions[entity];
            CDimension dimension = (CDimension)dimensions[entity];
            CTransform transform = (CTransform)transforms[entity];
            CKinetic kinetic = (CKinetic)kinetics[entity];
            CBoxCollider collider = (CBoxCollider)colliders[entity];

            List<int> queryResult = binPartitioner.Query(CreateQueryBounds());

            if (queryResult.Count == 0)
                return;

            CollisionInformation collisionInformation = collider.GetCollisionInformation(position, dimension, transform);
            Shape shape = new Shape(collisionInformation);

            CollisionInformation theirCollisionInformation;
            Shape theirShape;
            CPosition theirPosition;
            CDimension theirDimension;
            CTransform theirTransform;
            CBoxCollider theirCollider;

            for (int i = 0; i < queryResult.Count; i++)
            {
                if (!scene.EntityContains(queryResult[i], typeof(CTransform), typeof(CBoxCollider)))
                    continue;

                if (entity == queryResult[i])
                    continue;

                theirPosition = (CPosition)positions[queryResult[i]];
                theirDimension = (CDimension)dimensions[queryResult[i]];
                theirTransform = (CTransform)transforms[queryResult[i]];
                theirCollider = (CBoxCollider)colliders[queryResult[i]];

                theirCollisionInformation = theirCollider.GetCollisionInformation(theirPosition, theirDimension, theirTransform);
                theirShape = new Shape(theirCollisionInformation);

                ResolveCollisionTypes(CollisionHelper.GetCollisionTypesBetween(shape, theirShape, kinetic.Velocity));
            }

            RectangleF CreateQueryBounds()
            {
                return new RectangleF(position.X - queryBuffer, position.Y - queryBuffer, dimension.Width + queryBuffer * 2, dimension.Height + queryBuffer * 2);
            }

            void ResolveCollisionTypes(List<CollisionType> collisionTypes)
            {
                for (int i = 0; i < collisionTypes.Count; i++)
                {
                    switch (collisionTypes[i])
                    {
                        case CollisionType.Left:
                            position.X = theirShape.Bounds.Left - shape.Bounds.Width - leeway;
                            break;

                        case CollisionType.Right:
                            position.X = theirShape.Bounds.Right + leeway;
                            break;

                        case CollisionType.Top:
                            position.Y = theirShape.Bounds.Top - shape.Bounds.Height - leeway;
                            break;

                        case CollisionType.Bottom:
                            position.Y = theirShape.Bounds.Bottom + leeway;
                            break;
                    }
                }
            }
        }

        public override void Update()
        {
            if (binPartitioner == null)
            {
                binPartitioner = scene.GetSystem<SBinPartitioner>();
            }

            positions = scene.GetData<CPosition>();
            dimensions = scene.GetData<CDimension>();
            transforms = scene.GetData<CTransform>();
            kinetics = scene.GetData<CKinetic>();
            colliders = scene.GetData<CBoxCollider>();

            base.Update();
        }

        private class Shape : IShape
        {
            public RectangleF Bounds { get; set; }
            public Vector2[] Vertices { get; set; }
            public LineSegment[] LineSegments { get; set; }

            public Shape(CollisionInformation collisionInformation)
            {                
                Vertices = collisionInformation.Vertices;
                LineSegments = collisionInformation.LineSegments;

                CreateBounds();
            }

            private void CreateBounds()
            {
                float xMin = VertexFinder(Vertices, "x", "minimum");
                float xMax = VertexFinder(Vertices, "x", "maximum");
                float yMin = VertexFinder(Vertices, "y", "minimum");
                float yMax = VertexFinder(Vertices, "y", "maximum");

                float width = xMax - xMin;
                float height = yMax - yMin;

                Bounds = new RectangleF(xMin, yMin, width, height);
            }

            private float VertexFinder(Vector2[] vertices, string dimension, string qualifier)
            {
                float result = GetValueOf(0);

                for (int i = 1; i < vertices.Length; i++)
                {
                    if (Valid(GetValueOf(i)))
                    {
                        result = GetValueOf(i);
                    }
                }
                return result;

                float GetValueOf(int index)
                {
                    string formatted = dimension.ToLowerInvariant();

                    if (formatted == "x")
                    {
                        return vertices[index].X;
                    }
                    else if (formatted == "y")
                    {
                        return vertices[index].Y;
                    }

                    throw new ArgumentException();
                }

                bool Valid(float value)
                {
                    string formatted = qualifier.ToLowerInvariant();

                    if (formatted == "minimum")
                    {
                        return value < result;
                    }
                    else if (formatted == "maximum")
                    {
                        return value > result;
                    }

                    throw new ArgumentException();
                }
            }
        }
    }
}
