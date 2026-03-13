#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    [CustomPropertyDrawer(typeof(InjectAttribute))]
    public class InjectPropertyDrawer : PropertyDrawer
    {
        Texture2D icon;

        Texture2D LoadIcon()
        {
            if (icon != null) return icon;

            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(GetType().Assembly);
            string root = packageInfo != null ? packageInfo.assetPath : "Assets/Utils";
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{root}/Editor/DILite/icon_inject.png");
            return icon;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            icon = LoadIcon();
            var iconRect = new Rect(position.x, position.y, 20, 20);
            position.xMin += 24;

            if (icon != null)
            {
                var savedColor = GUI.color;
                GUI.color = property.objectReferenceValue == null ? savedColor : Color.green;
                GUI.DrawTexture(iconRect, icon);
                GUI.color = savedColor;
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }
}

#endif