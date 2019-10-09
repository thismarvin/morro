using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Core
{
    static class RandomHelper
    {
        private static Random RNG;

        public static void Initialize()
        {
            RNG = new Random(DateTime.Now.Millisecond);
        }

        public static int Range(int lowerBound, int upperBound)
        {
            return RNG.Next(lowerBound, upperBound + 1);
        }

        public static double Range(double lowerBound, double upperBound)
        {
            return lowerBound + RNG.NextDouble() * (upperBound - lowerBound);
        }

        public static double Gaussian(double mean, double standardDeviation)
        {
            double u1 = 1.0 - RNG.NextDouble();
            double u2 = 1.0 - RNG.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + standardDeviation * randStdNormal;
        }

        // Maybe add a PerlinNoise method.
    }
}
