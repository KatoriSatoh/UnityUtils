using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils
{
    public interface IPath
    {
        public List<PathSegment> Segments { get; }
        public float TotalLength { get; }

        public Vector3 GetPositionAtPercentage(float t);
        public Vector3 GetPositionAtDistance(float distance);
    }
}