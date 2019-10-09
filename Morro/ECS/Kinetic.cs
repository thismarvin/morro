using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.Graphics;
using Morro.Maths;
using Morro.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.ECS
{
    //public enum CollisionType
    //{
    //    QuadTop,
    //    QuadBottom,
    //    QuadLeft,
    //    QuadRight,

    //    RightTriangleSlopeTop,
    //    RightTriangleSlopeBottom,
    //}

    public enum Integrator
    {
        Euler,
        VelocityVerlet
    }

    abstract class Kinetic : Entity
    {
        public float MoveSpeed { get; protected set; }
        public Vector2 Velocity { get; protected set; }
        public Vector2 Acceleration { get; protected set; }

        protected List<CollisionType> collisionTypes;

        public Kinetic(float x, float y, int width, int height, float moveSpeed) : base(x, y, width, height)
        {
            MoveSpeed = moveSpeed;
            collisionTypes = new List<CollisionType>();
        }

        protected abstract void Collision();

        protected void ApplyForce(Integrator integrator)
        {
            switch (integrator)
            {
                case Integrator.Euler:
                    SetLocation(
                        Position.X + Velocity.X * Engine.DeltaTime,
                        Position.Y + Velocity.Y * Engine.DeltaTime
                        );

                    Velocity = new Vector2(
                        Velocity.X + Acceleration.X * Engine.DeltaTime,
                        Velocity.Y + Acceleration.Y * Engine.DeltaTime
                        );
                    break;

                case Integrator.VelocityVerlet:
                    SetLocation(
                        Position.X + Velocity.X * Engine.DeltaTime + 0.5f * Acceleration.X * Engine.DeltaTime * Engine.DeltaTime,
                        Position.Y + Velocity.Y * Engine.DeltaTime + 0.5f * Acceleration.Y * Engine.DeltaTime * Engine.DeltaTime
                        );

                    Velocity = new Vector2(
                        Velocity.X + Acceleration.X * Engine.DeltaTime,
                        Velocity.Y + Acceleration.Y * Engine.DeltaTime
                        );
                    break;
            }
        }

        protected List<CollisionType> GetCollisionTypesAgainst(AABB aabb)
        {
            if (!Bounds.Intersects(aabb.Bounds))
                return new List<CollisionType>();

            List<CollisionType> result = new List<CollisionType>();

            AABB.SetupForCollisionTesting();
            aabb.SetupForCollisionTesting();

            MTV pass1 = SATResolve(AABB, aabb);
            MTV pass2 = SATResolve(aabb, AABB);

            if (pass1.Overlap == -1 || pass2.Overlap == -1)
                return result;

            if (pass1.Overlap < pass2.Overlap)
            {
                if (pass1.EdgeIndex % 2 == 0)
                {
                    if (Velocity.X < 0 && Bounds.Left > aabb.Bounds.Left)
                        result.Add(CollisionType.QuadRight);
                    if (Velocity.X > 0 && Bounds.Right < aabb.Bounds.Right)
                        result.Add(CollisionType.QuadLeft);
                }
                else
                {
                    if (Velocity.Y < 0 && Bounds.Top > aabb.Bounds.Top)
                        result.Add(CollisionType.QuadBottom);
                    if (Velocity.Y > 0 && Bounds.Bottom < aabb.Bounds.Bottom)
                        result.Add(CollisionType.QuadTop);
                }
            }
            else
            {
                if (pass2.EdgeIndex % 2 == 0)
                {
                    if (Velocity.X < 0 && Bounds.Left > aabb.Bounds.Left)
                        result.Add(CollisionType.QuadRight);
                    if (Velocity.X > 0 && Bounds.Right < aabb.Bounds.Right)
                        result.Add(CollisionType.QuadLeft);
                }
                else
                {
                    if (Velocity.Y < 0 && Bounds.Top > aabb.Bounds.Top)
                        result.Add(CollisionType.QuadBottom);
                    if (Velocity.Y > 0 && Bounds.Bottom < aabb.Bounds.Bottom)
                        result.Add(CollisionType.QuadTop);
                }
            }

            return result;
        }


        private void ResolveCollision(Polygon a, Polygon b, Vector2 velocity)
        {
            a.SetupForCollisionTesting();
            b.SetupForCollisionTesting();

            MTV pass1 = SATResolve(a, b);
            MTV pass2 = SATResolve(b, a);

            if (pass1.Overlap == -1 || pass2.Overlap == -1)
                return;

            Console.WriteLine(pass1.EdgeIndex);

            if (pass1.Overlap < pass2.Overlap)
            {
                Vector2 axis = new Vector2(-(pass1.Edge.Y2 - pass1.Edge.Y1), pass1.Edge.X2 - pass1.Edge.X1);
                float edgeLength = axis.Length();
                float angle = (float)Math.Acos(Vector2.Dot(axis, Vector2.UnitX) / (edgeLength));

                float xFactor = (float)Math.Round(edgeLength * Math.Cos(angle));
                float yFactor = (float)Math.Round(edgeLength * Math.Sin(angle));

                float xOffset = xFactor == 0 ? 0 : pass1.Overlap / xFactor;
                float yOffset = yFactor == 0 ? 0 : pass1.Overlap / yFactor;

                if (velocity.X < 0)
                    xOffset *= -1;
                if (velocity.Y > 0)
                    yOffset *= -1;

                a.SetLocation(a.X + xOffset, a.Y + yOffset);
            }
            else
            {
                Vector2 axis = new Vector2(-(pass2.Edge.Y2 - pass2.Edge.Y1), pass2.Edge.X2 - pass2.Edge.X1);
                float edgeLength = axis.Length();
                float angle = (float)Math.Acos(Vector2.Dot(axis, Vector2.UnitX) / (edgeLength * Vector2.UnitX.Length()));

                float xFactor = (float)Math.Round(edgeLength * Math.Cos(angle));
                float yFactor = (float)Math.Round(edgeLength * Math.Sin(angle));

                float xOffset = xFactor == 0 ? 0 : pass2.Overlap / xFactor;
                float yOffset = yFactor == 0 ? 0 : pass2.Overlap / yFactor;

                if (velocity.X < 0)
                    xOffset *= -1;
                if (velocity.Y > 0)
                    yOffset *= -1;

                a.SetLocation(a.X + xOffset, a.Y + yOffset);
            }
        }

        private MTV SATResolve(Polygon a, Polygon b)
        {
            LineSegment edge = new LineSegment();
            float minOverlap = float.MaxValue;
            int edgeIndex = -1;
            float overlap;

            Vector2 normal;
            float projection;
            float minProjectionA;
            float maxProjectionA;
            float minProjectionB;
            float maxProjectionB;
            for (int i = 0; i < a.LineSegments.Length; i++)
            {
                normal = new Vector2(
                    -(a.LineSegments[i].Y2 - a.LineSegments[i].Y1),
                      a.LineSegments[i].X2 - a.LineSegments[i].X1
                    );

                minProjectionA = Vector2.Dot(a.TransformedVertices[0], normal);
                maxProjectionA = minProjectionA;
                for (int j = 0; j < a.TransformedVertices.Length; j++)
                {
                    projection = Vector2.Dot(a.TransformedVertices[j], normal);
                    minProjectionA = Math.Min(minProjectionA, projection);
                    maxProjectionA = Math.Max(maxProjectionA, projection);
                }

                minProjectionB = Vector2.Dot(b.TransformedVertices[0], normal);
                maxProjectionB = minProjectionB;
                for (int j = 0; j < b.TransformedVertices.Length; j++)
                {
                    projection = Vector2.Dot(b.TransformedVertices[j], normal);
                    minProjectionB = Math.Min(minProjectionB, projection);
                    maxProjectionB = Math.Max(maxProjectionB, projection);
                }

                overlap = Math.Min(maxProjectionA, maxProjectionB) - Math.Max(minProjectionA, minProjectionB);

                if (overlap < minOverlap)
                {
                    minOverlap = overlap;
                    edge = a.LineSegments[i];
                    edgeIndex = i;
                }

                if (!(maxProjectionB >= minProjectionA && maxProjectionA >= minProjectionB))
                    return new MTV(new LineSegment(), -1, -1);
            }

            return new MTV(edge, minOverlap, edgeIndex);
        }







        protected List<CollisionType> GetCollisionTypesAgainst(RightTriangle rightTriangle)
        {
            if (!AABB.Intersects(rightTriangle))
                return new List<CollisionType>();
           
            List<CollisionType> result = new List<CollisionType>();
            LineSegment lineSegment = new LineSegment(Center.X + Width / 2, Bounds.Top, Center.X + Width / 2, Bounds.Bottom + Velocity.Y);
            //LineSegment lineSegment = new LineSegment(Bounds.Right, Bounds.Top, Bounds.Right, Bounds.Bottom + Velocity.Y);

            if (lineSegment.Intersects(rightTriangle.LineSegments[2]))
            {
                result.Add(CollisionType.RightTriangleSlopeTop);
            }

            return result;
        }

        protected virtual void ResolveCollisionAgainst(AABB aabb)
        {
            collisionTypes = GetCollisionTypesAgainst(aabb);
            foreach (CollisionType collisionType in collisionTypes)
            {
                switch (collisionType)
                {
                    case CollisionType.QuadLeft:
                        SetLocation(aabb.Bounds.Left - Width, Y);
                        break;
                    case CollisionType.QuadRight:                        
                        SetLocation(aabb.Bounds.Right, Y);
                        break;
                    case CollisionType.QuadTop:
                        SetLocation(X, aabb.Bounds.Top - Height);
                        break;
                    case CollisionType.QuadBottom:
                        SetLocation(X, aabb.Bounds.Bottom);
                        break;
                }
            }
        }

        protected void ResolveCollisionAgainst(AABB aabb, float leeway)
        {
            collisionTypes = GetCollisionTypesAgainst(aabb);
            foreach (CollisionType collisionType in collisionTypes)
            {
                switch (collisionType)
                {
                    case CollisionType.QuadLeft:
                        if (Bounds.Bottom - aabb.Bounds.Top <= leeway)
                        {
                            SetLocation(X, aabb.Bounds.Top - Height);
                        }
                        else if (aabb.Bounds.Bottom - Bounds.Top <= leeway)
                        {
                            SetLocation(X, aabb.Bounds.Bottom);
                        }
                        else
                        {
                            SetLocation(aabb.Bounds.Left - Width, Y);
                        }
                        break;
                    case CollisionType.QuadRight:
                        if (Bounds.Bottom - aabb.Bounds.Top <= leeway)
                        {
                            SetLocation(X, aabb.Bounds.Top - Height);
                        }
                        else if (aabb.Bounds.Bottom - Bounds.Top <= leeway)
                        {
                            SetLocation(X, aabb.Bounds.Bottom);
                        }
                        else
                        {
                            SetLocation(aabb.Bounds.Right, Y);
                        }
                        break;
                    case CollisionType.QuadTop:
                        if (Bounds.Right - aabb.Bounds.Left <= leeway)
                        {
                            SetLocation(aabb.Bounds.Left - Width, Y);
                        }
                        else if (aabb.Bounds.Right - Bounds.Left <= leeway)
                        {
                            SetLocation(aabb.Bounds.Right, Y);
                        }
                        else
                        {
                            SetLocation(X, aabb.Bounds.Top - Height);
                        }
                        break;
                    case CollisionType.QuadBottom:
                        if (Bounds.Right - aabb.Bounds.Left <= leeway)
                        {
                            SetLocation(aabb.Bounds.Left - Width, Y);
                        }
                        else if (aabb.Bounds.Right - Bounds.Left <= leeway)
                        {
                            SetLocation(aabb.Bounds.Right, Y);
                        }
                        else
                        {
                            SetLocation(X, aabb.Bounds.Bottom);
                        }
                        break;
                }
            }
        }

        protected virtual void ResolveCollisionAgainst(RightTriangle rightTriangle)
        {
            collisionTypes = GetCollisionTypesAgainst(rightTriangle);
            foreach (CollisionType collisionType in collisionTypes)
            {
                switch (collisionType)
                {
                    case CollisionType.RightTriangleSlopeTop:
                        LineSegment lineSegment = new LineSegment(Center.X + Width / 2, Bounds.Top, Center.X + Width / 2, Bounds.Bottom - 0.5f);
                        //LineSegment lineSegment = new LineSegment(Bounds.Right, Bounds.Top, Bounds.Right, Bounds.Bottom + Velocity.Y);
                        IntersectionInformation intersectionInformation = lineSegment.GetIntersectionInformation(rightTriangle.LineSegments[2]);
                        SetLocation(X, Y - ((1 - intersectionInformation.T) * (lineSegment.Y2 - lineSegment.Y1)));
                        break;
                }
            }
        }
    }
}
