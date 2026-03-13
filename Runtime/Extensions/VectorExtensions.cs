using UnityEngine;

namespace UnityUtils
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Sets any x y values of a Vector2
        /// </summary>
        /// <param name="vector">The original Vector2.</param>
        /// <param name="x">The new x value. If null, the original x value is used.</param>
        /// <param name="y">The new y value. If null, the original y value is used.</param>
        /// <returns>A new Vector2 with the specified x and y values.</returns>
        public static Vector2 With(this Vector2 vector, float? x = null, float? y = null)
        {
            return new Vector2(x ?? vector.x, y ?? vector.y);
        }

        /// <summary>
        /// Sets any x y z values of a Vector3
        /// </summary>
        /// <param name="vector">The original Vector3.</param>
        /// <param name="x">The new x value. If null, the original x value is used.</param>
        /// <param name="y">The new y value. If null, the original y value is used.</param>
        /// <param name="z">The new z value. If null, the original z value is used.</param>
        /// <returns>A new Vector3 with the specified x, y, and z values.</returns>
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }

        /// <summary>
        /// Adds any x y values to a Vector2
        /// </summary>
        public static Vector2 Add(this Vector2 vector, float? x = null, float? y = null)
        {
            return new Vector2(vector.x + (x ?? 0), vector.y + (y ?? 0));
        }

        /// <summary>
        /// Adds any x y z values to a Vector3
        /// </summary>
        public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
        }

        /// <summary>
        /// Multiplies any x y values of a Vector2
        /// </summary>
        public static Vector2 Mult(this Vector2 vector, float? x = null, float? y = null)
        {
            return new Vector2(vector.x * (x ?? 1), vector.y * (y ?? 1));
        }

        /// <summary>
        /// Multiplies any x y z values of a Vector3
        /// </summary>
        public static Vector3 Mult(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(vector.x * (x ?? 1), vector.y * (y ?? 1), vector.z * (z ?? 1));
        }
    }
}