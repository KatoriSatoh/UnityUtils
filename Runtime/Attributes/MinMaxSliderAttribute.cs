using System;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Draws a min-max range slider for a Vector2 field in the Inspector.
    /// Replaces Odin Inspector's MinMaxSlider attribute.
    ///
    /// Usage:
    ///   [MinMaxSlider(-3f, 3f)]             — slider only
    ///   [MinMaxSlider(-3f, 3f, true)]       — slider + float input fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }
        public bool ShowFields { get; }

        public MinMaxSliderAttribute(float min, float max, bool showFields = false)
        {
            Min = min;
            Max = max;
            ShowFields = showFields;
        }
    }
}
