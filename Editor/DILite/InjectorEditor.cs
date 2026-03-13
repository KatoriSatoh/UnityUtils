#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    [CustomEditor(typeof(Injector))]
    public class InjectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Injector injector = (Injector)target;

            if (GUILayout.Button("Validate Dependencies"))
            {
                injector.ValidateDependencies();
            }
        }
    }
}

#endif