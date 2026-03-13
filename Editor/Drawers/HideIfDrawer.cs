#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        private bool IsHidden(SerializedProperty property)
        {
            var attr = (HideIfAttribute)attribute;
            return DrawerHelper.EvaluateCondition(property, attr.Condition, attr.CompareValue, attr.HasCompareValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsHidden(property)) return 0f;
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsHidden(property)) return;
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}

#endif
