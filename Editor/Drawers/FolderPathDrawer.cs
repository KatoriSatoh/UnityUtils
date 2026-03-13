#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    [CustomPropertyDrawer(typeof(FolderPathAttribute))]
    public class FolderPathDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 26f;
        private const float Padding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, $"{property.name} must be a string.", MessageType.Error);
                return;
            }

            var attr = (FolderPathAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            Rect fieldRect  = new(position.x, position.y, position.width - ButtonWidth - Padding, position.height);
            Rect buttonRect = new(position.xMax - ButtonWidth, position.y, ButtonWidth, position.height);

            // Highlight in red when RequireExistingPath and path is invalid
            bool invalid = attr.RequireExistingPath &&
                           !string.IsNullOrEmpty(property.stringValue) &&
                           !AssetDatabase.IsValidFolder(property.stringValue);

            if (invalid)
            {
                var savedColor = GUI.color;
                GUI.color = new Color(1f, 0.4f, 0.4f);
                property.stringValue = EditorGUI.TextField(fieldRect, label, property.stringValue);
                GUI.color = savedColor;
            }
            else
            {
                property.stringValue = EditorGUI.TextField(fieldRect, label, property.stringValue);
            }

            if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("FolderEmpty On Icon")))
            {
                string current = property.stringValue;
                string absolute = string.IsNullOrEmpty(current)
                    ? Application.dataPath
                    : Path.GetFullPath(Path.Combine(Application.dataPath, "..", current));

                // Defer the panel outside the current GUI event to avoid
                // "EndLayoutGroup: BeginLayoutGroup must be called first"
                EditorApplication.delayCall += () =>
                {
                    string chosen = EditorUtility.OpenFolderPanel("Select Folder", absolute, "");

                    if (!string.IsNullOrEmpty(chosen))
                    {
                        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                        if (chosen.StartsWith(projectRoot))
                            chosen = chosen.Substring(projectRoot.Length).TrimStart('/', '\\');

                        property.stringValue = chosen;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                };
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}

#endif
