using System;

namespace UnityUtils
{
    /// <summary>
    /// Draws a button in the Inspector that invokes the decorated method when clicked.
    /// Replaces Odin Inspector's Button attribute.
    /// Methods with parameters are supported — a field is drawn for each parameter.
    ///
    /// Usage:
    ///   [Button] private void MyMethod() { }
    ///   [Button("Click Me")] private void MyMethod() { }
    ///   [Button] private void MyMethod(string label = "default", int count = 1) { }
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        public string Label { get; }

        public ButtonAttribute(string label = null)
        {
            Label = label;
        }
    }
}
