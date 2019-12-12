﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    static class VectorHelper
    {
        public static Vector2 Perdendicular(Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static Vector2 FromAngle(float angle)
        {
            return PolarToCartesian(1, angle);
        }

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            return (float)Math.Acos(Vector2.Dot(a, b) / (a.Length() * b.Length()));
        }

        /// <summary>
        /// Converts spherical coordinates into cartesian coordinates.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static Vector2 PolarToCartesian(float radius, float theta)
        {
            return new Vector2
            (
                radius * (float)Math.Cos(theta),
                radius * (float)Math.Sin(theta)
            );
        }

        /// <summary>
        /// Converts spherical coordinates into cartesian coordinates.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="theta">the vertical angle from the z-axis.</param>
        /// <param name="azimuth">the horizontal angle from the x-axis.</param>
        /// <returns></returns>
        public static Vector3 PolarToCartesian(float radius, float theta, float azimuth)
        {
            return new Vector3
            (
                radius * (float)Math.Sin(theta) * (float)Math.Cos(azimuth),
                radius * (float)Math.Sin(theta) * (float)Math.Sin(azimuth),
                radius * (float)Math.Cos(theta)
            );
        }

        public static void SetMagnitude(this ref Vector2 self, float magnitude)
        {
            self.Normalize();
            self *= magnitude;
        }

        public static void Limit(this ref Vector2 self, float maxLength)
        {
            if (self.LengthSquared() > maxLength * maxLength)
            {
                self.SetMagnitude(maxLength);
            }
        }
    }
}
