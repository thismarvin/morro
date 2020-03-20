using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    static class MoreRandom
    {
        private static readonly System.Random RNG;

        static MoreRandom()
        {
            RNG = new System.Random(DateTime.Now.Millisecond);
        }

        public static int Range(int lowerBound, int upperBound)
        {
            return RNG.Next(lowerBound, upperBound + 1);
        }

        public static double Range(double lowerBound, double upperBound)
        {
            return lowerBound + RNG.NextDouble() * (upperBound - lowerBound);
        }

        public static bool Roll(float probability)
        {
            return RNG.NextDouble() <= probability;
        }

        public static double Gaussian(double mean, double standardDeviation)
        {
            double u1 = 1.0 - RNG.NextDouble();
            double u2 = 1.0 - RNG.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + standardDeviation * randStdNormal;
        }

        public static Vector2 RandomVector2()
        {
            return RandomVector2(1);
        }

        public static Vector2 RandomVector2(float magnitude)
        {
            float angle = (float)Range(0.0, MathHelper.TwoPi);
            Vector2 result = VectorHelper.FromAngle(angle);
            result.SetMagnitude(magnitude);

            return result;
        }
    }
}
