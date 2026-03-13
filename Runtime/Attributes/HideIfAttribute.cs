using System;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Hides the field in the Inspector when a condition is met (inverse of ShowIf).
    /// Replaces Odin Inspector's HideIf attribute.
    ///
    /// Usage:
    ///   [HideIf(nameof(myBoolField))]               — hide when myBoolField is true
    ///   [HideIf(nameof(myEnumField), MyEnum.Value)] — hide when myEnumField == MyEnum.Value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class HideIfAttribute : PropertyAttribute
    {
        public string Condition { get; }
        public object CompareValue { get; }
        public bool HasCompareValue { get; }

        public HideIfAttribute(string condition)
        {
            Condition = condition;
            HasCompareValue = false;
        }

        public HideIfAttribute(string condition, object compareValue)
        {
            Condition = condition;
            CompareValue = compareValue;
            HasCompareValue = true;
        }
    }
}
