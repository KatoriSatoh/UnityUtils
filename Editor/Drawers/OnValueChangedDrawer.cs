#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Draws the field normally and invokes the target method whenever the value changes.
    /// Also respects ShowIf / HideIf attributes stacked on the same field, since Unity
    /// only runs one PropertyDrawer per field (the last matching attribute wins).
    /// </summary>
    [CustomPropertyDrawer(typeof(OnValueChangedAttribute))]
    public class OnValueChangedDrawer : PropertyDrawer
    {
        private bool IsVisible(SerializedProperty property)
        {
            // Check stacked ShowIf
            var showIf = (ShowIfAttribute)System.Attribute.GetCustomAttribute(
                fieldInfo, typeof(ShowIfAttribute));
            if (showIf != null)
                return DrawerHelper.EvaluateCondition(property, showIf.Condition, showIf.CompareValue, showIf.HasCompareValue);

            // Check stacked HideIf
            var hideIf = (HideIfAttribute)System.Attribute.GetCustomAttribute(
                fieldInfo, typeof(HideIfAttribute));
            if (hideIf != null)
                return !DrawerHelper.EvaluateCondition(property, hideIf.Condition, hideIf.CompareValue, hideIf.HasCompareValue);

            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsVisible(property)) return 0f;
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsVisible(property)) return;

            var attr = (OnValueChangedAttribute)attribute;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label, true);
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                DrawerHelper.InvokeMethod(property, attr.MethodName);
            }
        }
    }
}

#endif
