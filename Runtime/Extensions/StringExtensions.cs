using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityUtils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the specified list contains a string that matches the specified string, ignoring case.
        /// </summary>
        /// <param name="list">The list to search.</param>
        /// <param name="other">The string to compare.</param>
        /// <returns><c>true</c> if the list contains a matching string; otherwise, <c>false</c>.</returns>
        public static bool ContainsIgnoreCase(this IList<string> list, string other)
        {
            foreach (string str in list)
            {
                if (str.ToLower() == other.ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the current string contains the specified string, ignoring case.
        /// </summary>
        /// <param name="str">The current string.</param>
        /// <param name="other">The string to search for.</param>
        /// <returns><c>true</c> if the current string contains the specified string, ignoring case; otherwise, <c>false</c>.</returns>
        public static bool ContainsIgnoreCase(this string str, string other)
        {
            return str.ToLower().Contains(other.ToLower());
        }

        /// <summary>
        /// Determines whether the current string is equal to another string, ignoring case.
        /// </summary>
        /// <param name="str">The current string.</param>
        /// <param name="other">The other string to compare.</param>
        /// <returns><c>true</c> if the strings are equal, ignoring case; otherwise, <c>false</c>.</returns>
        public static bool IsEqualIgnoreCase(this string str, string other)
        {
            return str.ToLower() == other.ToLower();
        }

        /// <summary>
        /// Converts an arbitrary string into a valid C# enum identifier:
        /// replaces invalid characters with '_', collapses duplicates, trims
        /// leading underscores/digits, and falls back to '_' if empty.
        /// </summary>
        public static string ToEnumEntry(this string name)
        {
            // Replace any character that isn't a letter, digit, or underscore with '_'
            string sanitized = Regex.Replace(name, @"[^\w]", "_");
            // Collapse consecutive underscores
            sanitized = Regex.Replace(sanitized, @"_+", "_");
            sanitized = sanitized.Trim('_');
            // Prefix with '_' if it starts with a digit (do this after trimming)
            if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))
                sanitized = "_" + sanitized;
            return string.IsNullOrEmpty(sanitized) ? "_" : sanitized;
        }
    }
}