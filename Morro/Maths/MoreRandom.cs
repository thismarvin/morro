using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Morro.Maths
{
    /// <summary>
    /// Manages an instance of a <see cref="Random"/> class object, and adds additional random number generation related functionality.
    /// </summary>
    static class MoreRandom
    {
        /// <summary>
        /// The game's main pseudo-random number generator that is seeded upon startup.
        /// </summary>
        public static Random RNG { get; set; }

        static MoreRandom()
        {
            RNG = new Random(DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Returns a random integer between the given inclusive bounds.
        /// </summary>
        /// <param name="lowerBound">The miniumum value of the random value.</param>
        /// <param name="upperBound">The maxiumum value of the random value.</param>
        /// <returns>A random integer between the given inclusive bounds.</returns>
        public static int Range(int lowerBound, int upperBound)
        {
            return RNG.Next(lowerBound, upperBound + 1);
        }

        /// <summary>
        /// Returns a random double between the given inclusive bounds.
        /// </summary>
        /// <param name="lowerBound">The miniumum value of the random value.</param>
        /// <param name="upperBound">The maxiumum value of the random value.</param>
        /// <returns>A random double between the given inclusive bounds.</returns>
        public static double Range(double lowerBound, double upperBound)
        {
            return lowerBound + RNG.NextDouble() * (upperBound - lowerBound);
        }

        /// <summary>
        /// Returns a boolean value that is consistent with a given probability.
        /// </summary>
        /// <param name="probability">Represents how likely the roll is to be successful. (The value should be greater than or equal to zero but less than or equal to one).</param>
        /// <returns>A boolean value that is consistent with a given probability.</returns>
        public static bool Roll(double probability)
        {
            if (probability <= 0)
                return false;

            if (probability >= 1)
                return true;

            return RNG.NextDouble() <= probability;
        }

        /// <summary>
        /// Returns a random double that depends on a gaussian (or normal) distrubition.
        /// </summary>
        /// <param name="mean">The central value of your desired randomness.</param>
        /// <param name="standardDeviation">The amount of variance from the mean of your desired randomness.</param>
        /// <returns>A random double that depends on a gaussian (or normal) distrubition.</returns>
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

        /// <summary>
        /// Returns a <see cref="Vector2"/> that points in a random direction and is of a given length.
        /// </summary>
        /// <param name="magnitude">The length of the random vector.</param>
        /// <returns>A <see cref="Vector2"/> that points in a random direction and is of a given length.</returns>
        public static Vector2 RandomVector2(float magnitude)
        {
            float angle = (float)Range(0.0, MathHelper.TwoPi);
            Vector2 result = VectorHelper.FromAngle(angle);
            result.SetMagnitude(magnitude);

            return result;
        }
    }
}
