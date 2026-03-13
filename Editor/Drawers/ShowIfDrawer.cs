#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        private bool IsVisible(SerializedProperty property)
        {
            var showIf = (ShowIfAttribute)attribute;
            return DrawerHelper.EvaluateCondition(property, showIf.Condition, showIf.CompareValue, showIf.HasCompareValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!IsVisible(property)) return 0f;
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!IsVisible(property)) return;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}

#endif
