using Microsoft.Xna.Framework;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    /// <summary>
    /// An identifier for tailored collision data.
    /// </summary>
    public enum CollisionType
    {
        Top,
        Bottom,
        Left,
        Right,
        Slope,
    }

    static class CollisionHelper
    {
        /// <summary>
        /// Prevents two AABBs from overlapping.
        /// <para>This method was tailored specifically for AABBs,
        /// therefore the collision resolution is better than just calling <see cref="NaivelyResolveCollisionBetween(Polygon, Polygon, Vector2)"/>.</para>
        /// </summary>
        /// <param name="a">the AABB that should be affected by the collision resolution</param>
        /// <param name="b">the AABB that the collision should be tested against</param>
        /// <param name="aVelocity">the velocity of AABB a</param>
        /// <returns>Returns a list of all the CollisionTypes that needed to be resolved.</returns>
        public static List<CollisionType> ResolveCollisionBetween(AABB a, AABB b, Vector2 aVelocity)
        {
            float leeway = 0.1f;
            List<CollisionType> collisionTypes = GetCollisionTypesBetween(a, b, aVelocity);
            foreach (CollisionType collisionType in collisionTypes)
            {
                switch (collisionType)
                {
                    case CollisionType.Left:
                        a.SetLocation(b.Bounds.Left - a.Width - leeway, a.Y);
                        break;
                    case CollisionType.Right:
                        a.SetLocation(b.Bounds.Right + leeway, a.Y);
                        break;
                    case CollisionType.Top:
                        a.SetLocation(a.X, b.Bounds.Top - a.Height - leeway);
                        break;
                    case CollisionType.Bottom:
                        a.SetLocation(a.X, b.Bounds.Bottom + leeway);
                        break;
                }
            }
            return collisionTypes;
        }

        // Not sure if this really works!
        public static List<CollisionType> ResolveCollisionBetween(AABB a, AABB b, Vector2 aVelocity, float leeway)
        {
            List<CollisionType> collisionTypes = GetCollisionTypesBetween(a, b, aVelocity);
            foreach (CollisionType collisionType in collisionTypes)
            {
                switch (collisionType)
                {
                    case CollisionType.Left:
                        if (a.Bounds.Bottom - b.Bounds.Top <= leeway)
                        {
                            a.SetLocation(a.X, b.Bounds.Top - a.Height);
                        }
                        else if (b.Bounds.Bottom - a.Bounds.Top <= leeway)
                        {
                            a.SetLocation(a.X, b.Bounds.Bottom);
                        }
                        else
                        {
                            a.SetLocation(b.Bounds.Left - a.Width, a.Y);
                        }
                        break;
                    case CollisionType.Right:
                        if (a.Bounds.Bottom - b.Bounds.Top <= leeway)
                        {
                            a.SetLocation(a.X, b.Bounds.Top - a.Height);
                        }
                        else if (b.Bounds.Bottom - a.Bounds.Top <= leeway)
                        {
                            a.SetLocation(a.X, b.Bounds.Bottom);
                        }
                        else
                        {
                            a.SetLocation(b.Bounds.Right, a.Y);
                        }
                        break;
                    case CollisionType.Top:
                        if (a.Bounds.Right - b.Bounds.Left <= leeway)
                        {
                            a.SetLocation(b.Bounds.Left - a.Width, a.Y);
                        }
                        else if (b.Bounds.Right - a.Bounds.Left <= leeway)
                        {
                            a.SetLocation(b.Bounds.Right, a.Y);
                        }
                        else
                        {
                            a.SetLocation(a.X, b.Bounds.Top - a.Height);
                        }
                        break;
                    case CollisionType.Bottom:
                        if (a.Bounds.Right - b.Bounds.Left <= leeway)
                        {
                            a.SetLocation(b.Bounds.Left - a.Width, a.Y);
                        }
                        else if (b.Bounds.Right - a.Bounds.Left <= leeway)
                        {
                            a.SetLocation(b.Bounds.Right, a.Y);
                        }
                        else
                        {
                            a.SetLocation(a.X, b.Bounds.Bottom);
                        }
                        break;
                }
            }
            return collisionTypes;
        }

        /// <summary>
        /// DOESNT REALLY WORK
        /// </summary>
        public static List<CollisionType> ResolveCollisionBetween(AABB a, RightTriangle b, Vector2 aVelocity)
        {
            List<CollisionType> collisionTypes = GetCollisionTypesBetween(a, b, aVelocity);
            foreach (CollisionType collisionType in collisionTypes)
            {
                switch (collisionType)
                {
                    case CollisionType.Slope:
                        LineSegment lineSegment = new LineSegment(a.Center.X, a.Bounds.Top, a.Center.X, a.Bounds.Bottom);
                        IntersectionInformation intersectionInformation = lineSegment.GetIntersectionInformation(b.LineSegments[2]);
                        a.SetLocation(a.X, a.Y - ((1 - intersectionInformation.T) * (lineSegment.Y2 - lineSegment.Y1)));
                        break;
                }
            }
            return collisionTypes;
        }

        /// <summary>
        /// Prevent two polygons from colliding with eachother.
        /// Non-overlap is guaranteed, but the displacement of the resolution is not necessarily perfect.
        /// </summary>
        /// <param name="a">the polygon that should be affected by the collision resolution</param>
        /// <param name="b">the polygon that the collision should be tested against</param>
        /// <param name="aVelocity">the velocity of polygon a</param>
        /// <returns>Returns a list of all the CollisionTypes that needed to be resolved.</returns>
        public static List<CollisionType> NaivelyResolveCollisionBetween(Polygon a, Polygon b, Vector2 aVelocity)
        {
            List<CollisionType> collisionTypes = new List<CollisionType>();

            a.SetupForCollisionTesting();
            b.SetupForCollisionTesting();

            MTV pass1 = SATResolve(a, b);
            MTV pass2 = SATResolve(b, a);

            if (pass1.Overlap == -1 || pass2.Overlap == -1)
                return collisionTypes;

            if (pass1.Overlap < pass2.Overlap)
            {
                Vector2 axis = new Vector2(-(pass1.Edge.Y2 - pass1.Edge.Y1), pass1.Edge.X2 - pass1.Edge.X1);
                float edgeLength = axis.Length();
                float angle = (float)Math.Acos(Vector2.Dot(axis, Vector2.UnitX) / (edgeLength));
                int angleAsDegrees = (int)(angle * 180 / Math.PI);

                if (angleAsDegrees == 90)
                {
                    collisionTypes.Add(aVelocity.Y > 0 ? CollisionType.Top : CollisionType.Bottom);
                }
                else if (angleAsDegrees == 180)
                {
                    collisionTypes.Add(aVelocity.X > 0 ? CollisionType.Left : CollisionType.Right);
                }
                else
                {
                    collisionTypes.Add(CollisionType.Slope);
                }

                float xFactor = (float)Math.Round(edgeLength * Math.Cos(angle));
                float yFactor = (float)Math.Round(edgeLength * Math.Sin(angle));

                float xOffset = xFactor == 0 ? 0 : pass1.Overlap / xFactor;
                float yOffset = yFactor == 0 ? 0 : pass1.Overlap / yFactor;

                if (aVelocity.X < 0)
                    xOffset *= -1;
                if (aVelocity.Y > 0)
                    yOffset *= -1;

                a.SetLocation(a.X + xOffset, a.Y + yOffset);
            }
            else
            {
                Vector2 axis = new Vector2(-(pass2.Edge.Y2 - pass2.Edge.Y1), pass2.Edge.X2 - pass2.Edge.X1);
                float edgeLength = axis.Length();
                float angle = (float)Math.Acos(Vector2.Dot(axis, Vector2.UnitX) / (edgeLength * Vector2.UnitX.Length()));
                int angleAsDegrees = (int)(angle * 180 / Math.PI);

                if (angleAsDegrees == 90)
                {
                    collisionTypes.Add(aVelocity.Y > 0 ? CollisionType.Top : CollisionType.Bottom);
                }                    
                else if (angleAsDegrees == 180)
                {
                    collisionTypes.Add(aVelocity.X > 0 ? CollisionType.Left : CollisionType.Right);
                }
                else
                {
                    collisionTypes.Add(CollisionType.Slope);
                }
                    
                float xFactor = (float)Math.Round(edgeLength * Math.Cos(angle));
                float yFactor = (float)Math.Round(edgeLength * Math.Sin(angle));

                float xOffset = xFactor == 0 ? 0 : pass2.Overlap / xFactor;
                float yOffset = yFactor == 0 ? 0 : pass2.Overlap / yFactor;

                if (aVelocity.X < 0)
                    xOffset *= -1;
                if (aVelocity.Y > 0)
                    yOffset *= -1;

                a.SetLocation(a.X + xOffset, a.Y + yOffset);
            }

            return collisionTypes;
        }

        /// <summary>
        /// Get the types of collisions between two AABBs.
        /// <para>This is useful for special cases where more than just collision resolution is needed (eg. manipulate velocity).</para>
        /// </summary>
        /// <param name="a">the AABB that should be affected by the collision resolution</param>
        /// <param name="b">the AABB that the collision should be tested against</param>
        /// <param name="aVelocity">the velocity of AABB a</param>
        /// <returns>Returns a list of all the CollisionTypes that need to be resolved.</returns>
        private static List<CollisionType> GetCollisionTypesBetween(AABB a, AABB b, Vector2 aVelocity)
        {
            if (!a.Bounds.Intersects(b.Bounds))
                return new List<CollisionType>();

            List<CollisionType> result = new List<CollisionType>();

            a.SetupForCollisionTesting();
            b.SetupForCollisionTesting();

            MTV pass1 = SATResolve(a, b);
            MTV pass2 = SATResolve(b, a);

            if (pass1.Overlap == -1 || pass2.Overlap == -1)
                return result;

            if (pass1.Overlap < pass2.Overlap)
            {
                if (pass1.EdgeIndex % 2 == 0)
                {
                    if (aVelocity.X < 0 && a.Bounds.Left > b.Bounds.Left)
                        result.Add(CollisionType.Right);
                    if (aVelocity.X > 0 && a.Bounds.Right < b.Bounds.Right)
                        result.Add(CollisionType.Left);
                }
                else
                {
                    if (aVelocity.Y < 0 && a.Bounds.Top > b.Bounds.Top)
                        result.Add(CollisionType.Bottom);
                    if (aVelocity.Y > 0 && a.Bounds.Bottom < b.Bounds.Bottom)
                        result.Add(CollisionType.Top);
                }
            }
            else
            {
                if (pass2.EdgeIndex % 2 == 0)
                {
                    if (aVelocity.X < 0 && a.Bounds.Left > b.Bounds.Left)
                        result.Add(CollisionType.Right);
                    if (aVelocity.X > 0 && a.Bounds.Right < b.Bounds.Right)
                        result.Add(CollisionType.Left);
                }
                else
                {
                    if (aVelocity.Y < 0 && a.Bounds.Top > b.Bounds.Top)
                        result.Add(CollisionType.Bottom);
                    if (aVelocity.Y > 0 && a.Bounds.Bottom < b.Bounds.Bottom)
                        result.Add(CollisionType.Top);
                }
            }

            return result;
        }

        /// <summary>
        /// DOESNT REALLY WORK
        /// </summary>
        private static List<CollisionType> GetCollisionTypesBetween(AABB a, RightTriangle b, Vector2 aVelocity)
        {
            if (!a.Intersects(b))
                return new List<CollisionType>();

            List<CollisionType> result = new List<CollisionType>();
            LineSegment lineSegment = new LineSegment(a.Center.X , a.Bounds.Top, a.Center.X , a.Bounds.Bottom + aVelocity.Y * 0.5f);

            if (lineSegment.Intersects(b.LineSegments[2]))
            {
                result.Add(CollisionType.Slope);
            }

            return result;
        }

        private static MTV SATResolve(Polygon a, Polygon b)
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
    }
}

