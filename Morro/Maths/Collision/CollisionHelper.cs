using Microsoft.Xna.Framework;
using Morro.Core;
using Morro.ECS;
using Morro.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths.Collision
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
        public static Vector2 GetResolution(IShape a, IShape b)
        {
            if (!a.Bounds.Intersects(b.Bounds))
                return Vector2.Zero;

            CollisionInformation MTV = CalculateMTV(a, b);

            if (MTV == null)
                return Vector2.Zero;

            return ProcessCollisionInformation();

            Vector2 ProcessCollisionInformation()
            {
                Vector2 axis = new Vector2(-(MTV.Edge.Y2 - MTV.Edge.Y1), MTV.Edge.X2 - MTV.Edge.X1);

                float edgeLength = axis.Length();
                float angle = (float)Math.Acos(Vector2.Dot(axis, Vector2.UnitX) / edgeLength);

                double xFactor = Math.Round(edgeLength * Math.Cos(angle));
                double yFactor = Math.Round(edgeLength * Math.Sin(angle));

                int xResolutionDirection = a.Bounds.Left > b.Bounds.Left ? 1 : -1;
                int yResolutionDirection = a.Bounds.Bottom > b.Bounds.Bottom ? 1 : -1;

                float xResolution = xFactor == 0 ? 0 : (float)(MTV.Overlap / xFactor * xResolutionDirection);
                float yResolution = yFactor == 0 ? 0 : (float)(MTV.Overlap / yFactor * yResolutionDirection);

                return new Vector2(xResolution, yResolution);
            }
        }

        public static Vector2 GetResolutionAABB(IShape a, IShape b)
        {
            if (!a.Bounds.Intersects(b.Bounds))
                return Vector2.Zero;

            CollisionInformation MTV = CalculateMTV(a, b);

            if (MTV == null)
                return Vector2.Zero;

            return ProcessCollisionInformation();

            Vector2 ProcessCollisionInformation()
            {
                Vector2 resolution = Vector2.Zero;

                if (MTV.EdgeIndex % 2 == 0)
                {
                    if (a.Bounds.Left > b.Bounds.Left)
                    {
                        resolution.X = b.Bounds.Right - a.Bounds.X;
                    }
                    if (a.Bounds.Right < b.Bounds.Right)
                    {
                        resolution.X = b.Bounds.Left - a.Bounds.Width - a.Bounds.X;
                    }
                }
                else
                {
                    if (a.Bounds.Top > b.Bounds.Top)
                    {
                        resolution.Y = b.Bounds.Bottom - a.Bounds.Y;
                    }
                    if (a.Bounds.Bottom < b.Bounds.Bottom)
                    {
                        resolution.Y = b.Bounds.Top - a.Bounds.Height - a.Bounds.Y;
                    }
                }

                return resolution;
            }
        }

        private static CollisionInformation CalculateMTV(IShape a, IShape b)
        {
            CollisionInformation pass1 = SAT.CalculateCollisionInformation(a, b);
            CollisionInformation pass2 = SAT.CalculateCollisionInformation(b, a);

            if (pass1 == null || pass2 == null)
                return null;

            return pass1.Overlap < pass2.Overlap ? pass1 : pass2;
        }
    }
}

