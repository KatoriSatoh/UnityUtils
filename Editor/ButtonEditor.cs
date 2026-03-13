#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Catch-all custom editor that draws [Button]-decorated methods below the default inspector.
    /// Supports methods with parameters (string, int, float, bool, enum) by rendering input fields.
    /// Applied to all UnityEngine.Object types automatically — no per-class editor needed.
    /// </summary>
    [CustomEditor(typeof(UnityEngine.Object), true)]
    [CanEditMultipleObjects]
    public class ButtonEditor : Editor
    {
        // Persistent parameter value cache keyed by (Type, methodName)
        private static readonly Dictionary<(Type, string), object[]> ParamCache = new();

        private List<MethodInfo> _buttonMethods;

        private void OnEnable()
        {
            _buttonMethods = new List<MethodInfo>();
            var type = target.GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var method in type.GetMethods(flags))
            {
                if (method.GetCustomAttribute<ButtonAttribute>() != null)
                    _buttonMethods.Add(method);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_buttonMethods == null || _buttonMethods.Count == 0) return;

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

            foreach (var method in _buttonMethods)
                DrawButton(method);
        }

        private void DrawButton(MethodInfo method)
        {
            var attr = method.GetCustomAttribute<ButtonAttribute>();
            string label = string.IsNullOrEmpty(attr.Label)
                ? ObjectNames.NicifyVariableName(method.Name)
                : attr.Label;

            var parameters = method.GetParameters();

            if (parameters.Length > 0)
            {
                var key = (target.GetType(), method.Name);
                if (!ParamCache.TryGetValue(key, out object[] paramValues))
                {
                    paramValues = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                        paramValues[i] = parameters[i].HasDefaultValue
                            ? parameters[i].DefaultValue
                            : DefaultOf(parameters[i].ParameterType);
                    ParamCache[key] = paramValues;
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

                for (int i = 0; i < parameters.Length; i++)
                    paramValues[i] = DrawParamField(parameters[i], paramValues[i]);

                if (GUILayout.Button("Execute"))
                    InvokeOnTargets(method, paramValues);

                EditorGUILayout.EndVertical();
            }
            else
            {
                if (GUILayout.Button(label))
                    InvokeOnTargets(method, null);
            }
        }

        private void InvokeOnTargets(MethodInfo method, object[] args)
        {
            foreach (var t in targets)
            {
                Undo.RecordObject(t, method.Name);
                method.Invoke(t, args);
                EditorUtility.SetDirty(t);
            }
        }

        private static object DrawParamField(ParameterInfo param, object value)
        {
            string label = ObjectNames.NicifyVariableName(param.Name);
            Type type = param.ParameterType;

            if (type == typeof(string))
                return EditorGUILayout.TextField(label, value as string ?? string.Empty);
            if (type == typeof(int))
                return EditorGUILayout.IntField(label, value is int i ? i : 0);
            if (type == typeof(float))
                return EditorGUILayout.FloatField(label, value is float f ? f : 0f);
            if (type == typeof(bool))
                return EditorGUILayout.Toggle(label, value is bool b && b);
            if (type.IsEnum)
                return EditorGUILayout.EnumPopup(label, value is Enum e ? e : (Enum)Enum.GetValues(type).GetValue(0));

            EditorGUILayout.LabelField(label, $"[Unsupported: {type.Name}]");
            return value;
        }

        private static object DefaultOf(Type type)
        {
            if (type == typeof(string)) return string.Empty;
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}

#endif
