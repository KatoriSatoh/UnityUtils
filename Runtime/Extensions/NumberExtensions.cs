using UnityEngine;

namespace UnityUtils
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Returns the percentage of the value in relation to the total.
        /// </summary>
        /// <param name="value">The value to calculate the percentage of.</param>
        /// <param name="total">The total value to calculate the percentage of.</param>
        /// <returns>The percentage of the value in relation to the total.</returns>
        public static float PercentageOf(this int value, int total)
        {
            if (total == 0) return 1; // Handling division by zero
            return (float)value / total;
        }

        /// <summary>
        /// Returns the percentage of the value in relation to the total.
        /// </summary>
        /// <param name="value">The value to calculate the percentage of.</param>
        /// <param name="total">The total value to calculate the percentage of.</param>
        /// <returns>The percentage of the value in relation to the total.</returns>
        public static float PercentageOf(this float value, float total)
        {
            if (total == 0) return 1; // Handling division by zero
            return value / total;
        }

        /// <summary>
        /// Remaps the specified value from the range [from1, to1] to the range [from2, to2].
        /// </summary>
        /// <param name="value">The value to remap.</param>
        /// <param name="from1">The inclusive lower bound of the original range.</param>
        /// <param name="to1">The inclusive upper bound of the original range.</param>
        /// <param name="from2">The inclusive lower bound of the target range.</param>
        /// <param name="to2">The inclusive upper bound of the target range.</param>
        /// <returns>The value remapped to the target range.</returns>
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        /// <summary>
        /// Calculates the linear parameter t that produces the interpolant value within the range [from, to].
        /// Unlike the standard InverseLerp, this method does not clamp the result between 0 and 1.
        /// </summary>
        /// <param name="value">The value to be mapped.</param>
        /// <param name="from">The start value of the range.</param>
        /// <param name="to">The end value of the range.</param>
        /// <returns>The linear parameter t that maps to the value within the range [from, to].</returns>
        public static float InverseLerpUnclamped(this float value, float from, float to)
        {
            return (value - from) / (to - from);
        }

        /// <summary>
        /// Determines whether two float values are approximately equal within a specified tolerance.
        /// </summary>
        /// <param name="a">The first float value to compare.</param>
        /// <param name="b">The second float value to compare.</param>
        /// <param name="tolerance">The maximum difference between the two float values for them to be considered equal. Defaults to 0.0001f.</param>
        /// <returns>Returns true if the absolute difference between the two float values is less than the specified tolerance; otherwise, returns false.</returns>
        public static bool EqualsApproximately(this float a, float b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a - b) < tolerance;
        }

        /// <summary>
        /// Determines whether two double values are approximately equal within a specified tolerance.
        /// </summary>
        /// <param name="a">The first double value to compare.</param>
        /// <param name="b">The second double value to compare.</param>
        /// <param name="tolerance">The maximum difference between the two double values for them to be considered equal. Defaults to 0.0001.</param>
        /// <returns>Returns true if the absolute difference between the two double values is less than the specified tolerance; otherwise, returns false.</returns>
        public static bool EqualsApproximately(this double a, double b, double tolerance = 0.0001)
        {
            return System.Math.Abs(a - b) < tolerance;
        }
    }
}