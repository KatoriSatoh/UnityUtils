using System;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Displays a field in the Inspector as read-only (greyed out, non-editable).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReadOnlyAttribute : PropertyAttribute { }
}
