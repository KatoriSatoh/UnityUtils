using System;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Shows the field in the Inspector only when a condition is met.
    /// Replaces Odin Inspector's ShowIf attribute.
    ///
    /// Usage:
    ///   [ShowIf(nameof(myBoolField))]              — show when myBoolField is true
    ///   [ShowIf(nameof(myEnumField), MyEnum.Value)] — show when myEnumField == MyEnum.Value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string Condition { get; }
        public object CompareValue { get; }
        public bool HasCompareValue { get; }

        public ShowIfAttribute(string condition)
        {
            Condition = condition;
            HasCompareValue = false;
        }

        public ShowIfAttribute(string condition, object compareValue)
        {
            Condition = condition;
            CompareValue = compareValue;
            HasCompareValue = true;
        }
    }
}
