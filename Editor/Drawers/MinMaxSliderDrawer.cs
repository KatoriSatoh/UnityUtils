#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        private const float FieldWidth = 50f;
        private const float Padding = 4f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUI.HelpBox(position, $"{property.name} must be a Vector2.", MessageType.Error);
                return;
            }

            var attr = (MinMaxSliderAttribute)attribute;
            Vector2 range = property.vector2Value;
            float minVal = range.x;
            float maxVal = range.y;

            position = EditorGUI.PrefixLabel(position, label);

            if (attr.ShowFields)
            {
                // [minField] [====slider====] [maxField]
                Rect minRect    = new(position.x, position.y, FieldWidth, position.height);
                Rect sliderRect = new(position.x + FieldWidth + Padding, position.y,
                                     position.width - (FieldWidth + Padding) * 2, position.height);
                Rect maxRect    = new(position.xMax - FieldWidth, position.y, FieldWidth, position.height);

                EditorGUI.BeginChangeCheck();
                minVal = EditorGUI.FloatField(minRect, minVal);
                EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, attr.Min, attr.Max);
                maxVal = EditorGUI.FloatField(maxRect, maxVal);
                if (EditorGUI.EndChangeCheck())
                {
                    minVal = Mathf.Clamp(minVal, attr.Min, maxVal);
                    maxVal = Mathf.Clamp(maxVal, minVal, attr.Max);
                    property.vector2Value = new Vector2(minVal, maxVal);
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.MinMaxSlider(position, ref minVal, ref maxVal, attr.Min, attr.Max);
                if (EditorGUI.EndChangeCheck())
                    property.vector2Value = new Vector2(minVal, maxVal);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}

#endif
