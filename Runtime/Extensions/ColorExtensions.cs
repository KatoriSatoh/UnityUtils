using System;
using UnityEngine;

namespace UnityUtils
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Sets the alpha component of the color.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <param name="alpha">The new alpha value.</param>
        /// <returns>A new color with the specified alpha value.</returns>
        public static Color SetAlpha(this Color color, float alpha)
            => new(color.r, color.g, color.b, alpha);

        /// <summary>
        /// Adds the RGBA components of two colors and clamps the result between 0 and 1.
        /// </summary>
        /// <param name="thisColor">The first color.</param>
        /// <param name="otherColor">The second color.</param>
        /// <returns>A new color that is the sum of the two colors, clamped between 0 and 1.</returns>
        public static Color Add(this Color thisColor, Color otherColor)
            => (thisColor + otherColor).Clamp01();

        /// <summary>
        /// Subtracts the RGBA components of one color from another and clamps the result between 0 and 1.
        /// </summary>
        /// <param name="thisColor">The first color.</param>
        /// <param name="otherColor">The second color.</param>
        /// <returns>A new color that is the difference of the two colors, clamped between 0 and 1.</returns>
        public static Color Subtract(this Color thisColor, Color otherColor)
            => (thisColor - otherColor).Clamp01();

        /// <summary>
        /// Clamps the RGBA components of the color between 0 and 1.
        /// </summary>
        /// <param name="color">The original color.</param>
        /// <returns>A new color with each component clamped between 0 and 1.</returns>
        static Color Clamp01(this Color color)
        {
            return new Color
            {
                r = Mathf.Clamp01(color.r),
                g = Mathf.Clamp01(color.g),
                b = Mathf.Clamp01(color.b),
                a = Mathf.Clamp01(color.a)
            };
        }

        /// <summary>
        /// Converts a Color to a hexadecimal string.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <returns>A hexadecimal string representation of the color.</returns>
        public static string ToHex(this Color color) => $"#{ColorUtility.ToHtmlStringRGBA(color)}";

        /// <summary>
        /// Converts a hexadecimal string to a Color.
        /// </summary>
        /// <param name="hex">The hexadecimal string to convert.</param>
        /// <returns>The Color represented by the hexadecimal string.</returns>
        public static Color FromHex(this string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }

            throw new ArgumentException("Invalid hex string", nameof(hex));
        }

        /// <summary>
        /// Blends two colors with a specified ratio.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <param name="ratio">The blend ratio (0 to 1).</param>
        /// <returns>The blended color.</returns>
        public static Color Blend(this Color color1, Color color2, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            return new Color(
                color1.r * (1 - ratio) + color2.r * ratio,
                color1.g * (1 - ratio) + color2.g * ratio,
                color1.b * (1 - ratio) + color2.b * ratio,
                color1.a * (1 - ratio) + color2.a * ratio
            );
        }

        /// <summary>
        /// Inverts the color.
        /// </summary>
        /// <param name="color">The color to invert.</param>
        /// <returns>The inverted color.</returns>
        public static Color Invert(this Color color) => new(1 - color.r, 1 - color.g, 1 - color.b, color.a);

        /// <summary>
        /// Converts a Color from RGB to RYB color space.
        /// </summary>
        /// <param name="color">The color in RGB color space.</param>
        /// <returns>The color in RYB color space.</returns>
        public static ColorRYB ToRYB(this Color color)
        {
            var Iw = Mathf.Min(new float[] { color.r, color.g, color.b });
            var Ib = Mathf.Min(new float[] { 1 - color.r, 1 - color.g, 1 - color.b });

            var rRGB = color.r - Iw;
            var gRGB = color.g - Iw;
            var bRGB = color.b - Iw;

            var minRG = Mathf.Min(rRGB, gRGB);
            var rRYB = rRGB - minRG;
            var yRYB = (gRGB + minRG) / 2;
            var bRYB = (bRGB + gRGB - minRG) / 2;

            var n = Mathf.Max(new float[] { rRYB, yRYB, bRYB, 1 }) / Mathf.Max(new float[] { rRGB, gRGB, bRGB, 1 });
            return new ColorRYB(rRYB / n + Ib, yRYB / n + Ib, bRYB / n + Ib);
        }

        /// <summary>
        /// Converts a Color from RYB to RGB color space.
        /// </summary>
        /// <param name="color">The color in RYB color space.</param>
        /// <returns>The color in RGB color space.</returns>
        public static Color ToRGB(this ColorRYB color)
        {
            var Iw = Mathf.Min(new float[] { color.r, color.y, color.b });
            var Ib = Mathf.Min(new float[] { 1 - color.r, 1 - color.y, 1 - color.b });

            var rRYB = color.r - Iw;
            var yRYB = color.y - Iw;
            var bRYB = color.b - Iw;

            var minYB = Mathf.Min(yRYB, bRYB);
            var rRGB = rRYB + yRYB - minYB;
            var gRGB = yRYB + minYB;
            var bRGB = 2 * (bRYB - minYB);

            var n = Mathf.Max(new float[] { rRGB, gRGB, bRGB, 1 }) / Mathf.Max(new float[] { rRYB, yRYB, bRYB, 1 });
            return new Color(rRGB / n + Ib, gRGB / n + Ib, bRGB / n + Ib);
        }

        /// <summary>
        /// Mixes two colors in RYB color space.
        /// </summary>
        /// <param name="color1">The first color in RYB color space.</param>
        /// <param name="color2">The second color in RYB color space.</param>
        /// <returns>The mixed color in RYB color space.</returns>
        public static ColorRYB Mix(this ColorRYB color1, ColorRYB color2)
        {
            return new ColorRYB(
                Mathf.Min(255, color1.r + color2.r),
                Mathf.Min(255, color1.y + color2.y),
                Mathf.Min(255, color1.b + color2.b)
            );
        }

        public struct ColorRYB
        {
            public float r;
            public float y;
            public float b;

            public ColorRYB(float r, float y, float b)
            {
                this.r = r;
                this.y = y;
                this.b = b;
            }
        }
    }
}