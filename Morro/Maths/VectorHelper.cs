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
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float AngleBetween(Vector2 a, Vector2 b)
        {
            return (float)Math.Acos(Vector2.Dot(a, b) / (a.Length() * b.Length()));
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