using System;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Attribute to mark a field or property for injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class InjectAttribute : PropertyAttribute { }
}
