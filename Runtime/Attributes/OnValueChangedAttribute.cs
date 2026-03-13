using System;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Invokes a method on the serialized object whenever the field value changes in the Inspector.
    /// Replaces Odin Inspector's OnValueChanged attribute.
    /// Also respects stacked ShowIf / HideIf attributes on the same field.
    ///
    /// Usage:
    ///   [OnValueChanged(nameof(OnMyFieldChanged))] public float myField;
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class OnValueChangedAttribute : PropertyAttribute
    {
        public string MethodName { get; }

        public OnValueChangedAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
