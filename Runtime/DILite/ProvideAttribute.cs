using System;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Attribute to mark a method as a provider.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class ProvideAttribute : PropertyAttribute { }
}
