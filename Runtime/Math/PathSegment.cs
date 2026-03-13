using UnityEngine;

namespace UnityUtils
{
    public struct PathSegment
    {
        public Vector3 Current;
        public Vector3 Next;
        public float Length;
        public float PreviousTotalLength;

        public PathSegment(Vector3 start, Vector3 end, float prevLength)
        {
            Current = start;
            Next = end;
            Length = (end - start).magnitude;
            PreviousTotalLength = prevLength;
        }
    }
}