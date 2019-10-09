using Microsoft.Xna.Framework;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    public enum CollisionType
    {
        QuadTop,
        QuadBottom,
        QuadLeft,
        QuadRight,

        RightTriangleSlopeTop,
        RightTriangleSlopeBottom,
    }

    static class CollisionHelper
    {
        public static HashSet<CollisionType> GetCollisionTypes(this Quad q, Quad quad, Vector2 velocity)
        {
            if (!q.Intersects(quad))
                return new HashSet<CollisionType>();

            quad.SetupForCollisionTesting();

            HashSet<CollisionType> result = new HashSet<CollisionType>();
            LineSegment a;
            LineSegment b;
            LineSegment c;
            LineSegment d;
            float offset = 2;
            float buffer;

            buffer = velocity.Y + 1;
            // Object is moving left; resolve collision with the right of the quad.
            if (velocity.X < 0)
            {
                a = new LineSegment(q.Bounds.Left + offset, q.Bounds.Top + buffer, q.Bounds.Left - velocity.X, q.Bounds.Top + buffer);
                b = new LineSegment(q.Bounds.Left + offset, q.Bounds.Bottom - buffer, q.Bounds.Left - velocity.X, q.Bounds.Bottom - buffer);
                c = new LineSegment(quad.Bounds.Right - offset, quad.Bounds.Top + buffer, quad.Bounds.Right + velocity.X, quad.Bounds.Top + buffer);
                d = new LineSegment(quad.Bounds.Right - offset, quad.Bounds.Bottom - buffer, quad.Bounds.Right + velocity.X, quad.Bounds.Bottom - buffer);

                if (a.Intersects(quad.LineSegments[1]) || b.Intersects(quad.LineSegments[1]) || c.Intersects(q.LineSegments[3]) || d.Intersects(q.LineSegments[3]))
                {
                    result.Add(CollisionType.QuadRight);
                }
            }
            // Object is moving right; resolve collision with the left of the quad.
            else if (velocity.X > 0)
            {
                a = new LineSegment(q.Bounds.Right - offset, q.Bounds.Top + buffer, q.Bounds.Right + velocity.X, q.Bounds.Top + buffer);
                b = new LineSegment(q.Bounds.Right - offset, q.Bounds.Bottom - buffer, q.Bounds.Right + velocity.X, q.Bounds.Bottom - buffer);
                c = new LineSegment(quad.Bounds.Left + offset, quad.Bounds.Top + buffer, quad.Bounds.Left - velocity.X, quad.Bounds.Top + buffer);
                d = new LineSegment(quad.Bounds.Left + offset, quad.Bounds.Bottom - buffer, quad.Bounds.Left - velocity.X, quad.Bounds.Bottom - buffer);

                if (a.Intersects(quad.LineSegments[3]) || b.Intersects(quad.LineSegments[3]) || c.Intersects(q.LineSegments[1]) || d.Intersects(q.LineSegments[1]))
                {
                    result.Add(CollisionType.QuadLeft);
                }
            }

            buffer = velocity.X + 1;
            // Object is moving up; resolve collision with the bottom of the quad.
            if (velocity.Y < 0)
            {
                a = new LineSegment(q.Bounds.Left + buffer, q.Bounds.Top + offset, q.Bounds.Left + buffer, q.Bounds.Top - velocity.Y);
                b = new LineSegment(q.Bounds.Right - buffer, q.Bounds.Top + offset, q.Bounds.Right - buffer, q.Bounds.Top - velocity.Y);
                c = new LineSegment(quad.Bounds.Left + buffer, quad.Bounds.Bottom - offset, quad.Bounds.Left + buffer, quad.Bounds.Bottom + velocity.Y);
                d = new LineSegment(quad.Bounds.Right - buffer, quad.Bounds.Bottom - offset, quad.Bounds.Right - buffer, quad.Bounds.Bottom + velocity.Y);

                if (a.Intersects(quad.LineSegments[0]) || b.Intersects(quad.LineSegments[0]) || c.Intersects(q.LineSegments[2]) || d.Intersects(q.LineSegments[2]))
                {
                    result.Add(CollisionType.QuadBottom);
                }
            }
            // Object is moving down; resolve collision with the top of the quad.
            else if (velocity.Y > 0)
            {
                a = new LineSegment(q.Bounds.Left + buffer, q.Bounds.Bottom - offset, q.Bounds.Left + buffer, q.Bounds.Bottom + velocity.Y);
                b = new LineSegment(q.Bounds.Right - buffer, q.Bounds.Bottom - offset, q.Bounds.Right - buffer, q.Bounds.Bottom + velocity.Y);
                c = new LineSegment(quad.Bounds.Left + buffer, quad.Bounds.Top + offset, quad.Bounds.Left + buffer, quad.Bounds.Top - velocity.Y);
                d = new LineSegment(quad.Bounds.Right - buffer, quad.Bounds.Top + offset, quad.Bounds.Right - buffer, quad.Bounds.Top - velocity.Y);

                if (a.Intersects(quad.LineSegments[2]) || b.Intersects(quad.LineSegments[2]) || c.Intersects(q.LineSegments[0]) || d.Intersects(q.LineSegments[0]))
                {
                    result.Add(CollisionType.QuadTop);
                }
            }

            return result;
        }

        public static HashSet<CollisionType> GetCollisionTypes(this Quad q, RightTriangle rightTriangle, Vector2 velocity)
        {
            if (!q.Intersects(rightTriangle))
                return new HashSet<CollisionType>();

            HashSet<CollisionType> result = new HashSet<CollisionType>();
            IntersectionInformation intersectionInformation;
            LineSegment lineSegment = new LineSegment(q.Center.X, q.Center.Y, q.Center.X, q.Center.Y);
            float leeway = 2;

            switch (rightTriangle.RightAnglePosition)
            {
                case RightAnglePositionType.TopLeft:
                    lineSegment = new LineSegment(q.Bounds.Left, q.Bounds.Top + leeway, q.Bounds.Left, q.Bounds.Top - leeway);
                    break;
                case RightAnglePositionType.TopRight:
                    lineSegment = new LineSegment(q.Bounds.Right, q.Bounds.Top + leeway, q.Bounds.Right, q.Bounds.Top - leeway);
                    break;
                case RightAnglePositionType.BottomRight:
                    lineSegment = new LineSegment(q.Bounds.Right, q.Bounds.Bottom - leeway, q.Bounds.Right, q.Bounds.Bottom + leeway);
                    break;
                case RightAnglePositionType.BottomLeft:
                    lineSegment = new LineSegment(q.Bounds.Left, q.Bounds.Bottom - leeway, q.Bounds.Left, q.Bounds.Bottom + leeway);
                    break;
            }

            foreach (LineSegment segment in rightTriangle.LineSegments)
            {
                intersectionInformation = lineSegment.GetIntersectionInformation(segment);
                if (intersectionInformation.Intersected)
                {
                    if (rightTriangle.RightAnglePosition == RightAnglePositionType.BottomLeft || rightTriangle.RightAnglePosition == RightAnglePositionType.BottomRight)
                        result.Add(CollisionType.RightTriangleSlopeTop);
                    if (rightTriangle.RightAnglePosition == RightAnglePositionType.TopLeft || rightTriangle.RightAnglePosition == RightAnglePositionType.TopRight)
                        result.Add(CollisionType.RightTriangleSlopeBottom);
                }
            }

            return result;
        }

        public static void ResolveCollision(this Quad q, RightTriangle rightTriangle)
        {
            if (!q.Intersects(rightTriangle))
                return;

            IntersectionInformation intersectionInformation;
            LineSegment lineSegment = new LineSegment(q.Center.X, q.Center.Y, q.Center.X, q.Center.Y);

            float offsetY = 0;
            float leeway = 2;
            float extra = 0;// 0.1f;

            switch (rightTriangle.RightAnglePosition)
            {
                case RightAnglePositionType.TopLeft:
                    lineSegment = new LineSegment(q.Bounds.Left, q.Bounds.Top + leeway, q.Bounds.Left, q.Bounds.Top - extra);
                    break;
                case RightAnglePositionType.TopRight:
                    lineSegment = new LineSegment(q.Bounds.Right, q.Bounds.Top + leeway, q.Bounds.Right, q.Bounds.Top - extra);
                    break;
                case RightAnglePositionType.BottomRight:
                    lineSegment = new LineSegment(q.Bounds.Right, q.Bounds.Bottom - leeway, q.Bounds.Right, q.Bounds.Bottom + extra);
                    break;
                case RightAnglePositionType.BottomLeft:
                    lineSegment = new LineSegment(q.Bounds.Left, q.Bounds.Bottom - leeway, q.Bounds.Left, q.Bounds.Bottom + extra);
                    break;
            }

            //foreach (LineSegment segment in rightTriangle.LineSegments)
            //{
            //    intersectionInformation = lineSegment.GetIntersectionInformation(segment);
            //    if (intersectionInformation.Intersected)
            //    {
            //        offsetX += (1 - intersectionInformation.T) * (lineSegment.X2 - lineSegment.X1);
            //        offsetY += (1 - intersectionInformation.T) * (lineSegment.Y2 - lineSegment.Y1);
            //    }
            //}

            intersectionInformation = lineSegment.GetIntersectionInformation(rightTriangle.LineSegments[2]);
            if (intersectionInformation.Intersected)
            {
                offsetY += (1 - intersectionInformation.T) * (lineSegment.Y2 - lineSegment.Y1);
            }

            q.SetLocation(q.X, q.Y - offsetY);
        }

        public static void ResolveCollision(this Quad q, Quad quad, HashSet<CollisionType> collisionTypes)
        {
            if (collisionTypes.Count == 0)
                return;

            foreach (CollisionType collisionType in collisionTypes)
            {
                switch (collisionType)
                {
                    case CollisionType.QuadLeft:
                        q.SetLocation(quad.Bounds.Left - q.Width, q.Y);
                        break;
                    case CollisionType.QuadRight:
                        q.SetLocation(quad.Bounds.Right, q.Y);
                        break;
                    case CollisionType.QuadTop:
                        q.SetLocation(q.X, quad.Bounds.Top - q.Height);
                        break;
                    case CollisionType.QuadBottom:
                        q.SetLocation(q.X, quad.Bounds.Bottom);
                        break;
                }
            }
        }

        public static void ResolveCollision(this Quad q, Quad quad, HashSet<CollisionType> collisionTypes, float leeway)
        {
            if (collisionTypes.Count == 0)
                return;

            foreach (CollisionType collisionType in collisionTypes)
            {
                switch (collisionType)
                {
                    case CollisionType.QuadLeft:
                        if (q.Bounds.Bottom - quad.Bounds.Top <= leeway)
                        {
                            q.SetLocation(q.X, quad.Bounds.Top - q.Height);
                        }
                        else if (quad.Bounds.Bottom - q.Bounds.Top <= leeway)
                        {
                            q.SetLocation(q.X, quad.Bounds.Bottom);
                        }
                        else
                        {
                            q.SetLocation(quad.Bounds.Left - q.Width, q.Y);
                        }
                        break;
                    case CollisionType.QuadRight:
                        if (q.Bounds.Bottom - quad.Bounds.Top <= leeway)
                        {
                            q.SetLocation(q.X, quad.Bounds.Top - q.Height);
                        }
                        else if (quad.Bounds.Bottom - q.Bounds.Top <= leeway)
                        {
                            q.SetLocation(q.X, quad.Bounds.Bottom);
                        }
                        else
                        {
                            q.SetLocation(quad.Bounds.Right, q.Y);
                        }
                        break;
                    case CollisionType.QuadTop:
                        if (q.Bounds.Right - quad.Bounds.Left <= leeway)
                        {
                            q.SetLocation(quad.Bounds.Left - q.Width, q.Y);
                        }
                        else if (quad.Bounds.Right - q.Bounds.Left <= leeway)
                        {
                            q.SetLocation(quad.Bounds.Right, q.Y);
                        }
                        else
                        {
                            q.SetLocation(q.X, quad.Bounds.Top - q.Height);
                        }
                        break;
                    case CollisionType.QuadBottom:
                        if (q.Bounds.Right - quad.Bounds.Left <= leeway)
                        {
                            q.SetLocation(quad.Bounds.Left - q.Width, q.Y);
                        }
                        else if (quad.Bounds.Right - q.Bounds.Left <= leeway)
                        {
                            q.SetLocation(quad.Bounds.Right, q.Y);
                        }
                        else
                        {
                            q.SetLocation(q.X, quad.Bounds.Bottom);
                        }
                        break;
                }
            }
        }
    }
}

