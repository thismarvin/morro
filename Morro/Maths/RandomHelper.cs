using Morro.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    static class RandomHelper
    {
        public static int Range(int lowerBound, int upperBound)
        {
            return RandomManager.RNG.Next(lowerBound, upperBound + 1);
        }

        public static double Range(double lowerBound, double upperBound)
        {
            return lowerBound + RandomManager.RNG.NextDouble() * (upperBound - lowerBound);
        }

        public static bool Roll(float probability)
        {
            return RandomManager.RNG.NextDouble() <= probability;
        }

        public static double Gaussian(double mean, double standardDeviation)
        {
            double u1 = 1.0 - RandomManager.RNG.NextDouble();
            double u2 = 1.0 - RandomManager.RNG.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + standardDeviation * randStdNormal;
        }

        // Maybe add a PerlinNoise method.
    }
}
