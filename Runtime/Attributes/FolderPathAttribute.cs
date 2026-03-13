using System;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Draws a folder picker button for a string field in the Inspector.
    /// The stored path is relative to the project root (e.g. "Assets/Audio").
    /// Replaces Odin Inspector's FolderPath attribute.
    ///
    /// Usage:
    ///   [FolderPath] public string folderPath;
    ///   [FolderPath(RequireExistingPath = true)] public string folderPath;
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FolderPathAttribute : PropertyAttribute
    {
        /// <summary>When true, the chosen folder must already exist in the project.</summary>
        public bool RequireExistingPath { get; set; } = false;
    }
}
