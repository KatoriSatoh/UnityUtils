#if UNITY_EDITOR

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class SceneDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneNames = GetAllSceneInBuildSettings();
            var index = Mathf.Max(0, ArrayUtility.IndexOf(sceneNames, property.stringValue));

            index = EditorGUI.Popup(position, label.text, index, sceneNames);
            property.stringValue = sceneNames[index];
        }

        public string[] GetAllSceneInBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes;
            return scenes.Where(scene => scene.enabled).Select(scene => Path.GetFileNameWithoutExtension(scene.path)).ToArray();
        }
    }
}

#endif