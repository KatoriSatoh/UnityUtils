#if UNITY_EDITOR

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace UnityUtils
{
    /// <summary>
    /// Shared reflection helpers used by ShowIfDrawer, HideIfDrawer, and OnValueChangedDrawer.
    /// </summary>
    internal static class DrawerHelper
    {
        private const BindingFlags Flags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Evaluates a bool condition (field/property/method name) on the parent object.
        /// </summary>
        internal static bool EvaluateCondition(SerializedProperty property, string condition, object compareValue, bool hasCompareValue)
        {
            object target = GetParentObject(property);
            if (target == null) return false;

            Type type = target.GetType();

            FieldInfo field = type.GetField(condition, Flags);
            if (field != null)
            {
                object value = field.GetValue(target);
                return hasCompareValue ? Equals(value, compareValue) : value is true;
            }

            PropertyInfo prop = type.GetProperty(condition, Flags);
            if (prop != null)
            {
                object value = prop.GetValue(target);
                return hasCompareValue ? Equals(value, compareValue) : value is true;
            }

            MethodInfo method = type.GetMethod(condition, Flags);
            if (method != null)
            {
                object value = method.Invoke(target, null);
                return hasCompareValue ? Equals(value, compareValue) : value is true;
            }

            return false;
        }

        /// <summary>
        /// Invokes a void method by name on the serialized object targets.
        /// </summary>
        internal static void InvokeMethod(SerializedProperty property, string methodName)
        {
            foreach (var targetObject in property.serializedObject.targetObjects)
            {
                Type type = targetObject.GetType();
                MethodInfo method = type.GetMethod(methodName, Flags);
                method?.Invoke(targetObject, null);
            }
        }

        /// <summary>
        /// Traverses the serialized property path to return the object that directly
        /// contains the field — handles nested classes and arrays.
        /// </summary>
        internal static object GetParentObject(SerializedProperty property)
        {
            string path = property.propertyPath;
            object obj = property.serializedObject.targetObject;

            int lastDot = path.LastIndexOf('.');
            if (lastDot < 0) return obj;

            string parentPath = path.Substring(0, lastDot);

            foreach (string segment in parentPath.Split('.'))
            {
                if (obj == null) return null;
                if (segment == "Array") continue;

                if (segment.StartsWith("data["))
                {
                    if (obj is IList list)
                    {
                        int index = int.Parse(segment.Substring(5, segment.Length - 6));
                        obj = index < list.Count ? list[index] : null;
                    }
                    continue;
                }

                FieldInfo field = obj.GetType().GetField(segment, Flags);
                obj = field?.GetValue(obj);
            }

            return obj;
        }
    }
}

#endif
