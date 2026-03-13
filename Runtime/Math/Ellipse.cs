using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityUtils
{
    public class Ellipse : MonoBehaviour
    {
        [SerializeField] private float width, height;
        [SerializeField] private int segmentNum = 100;

        public float Length => _length;

        private List<CurveSegment> _segments = new();
        private float _length;

        [Button("Calculate Path Segments")]
        public async Task CalculatePathSegments()
        {
            while (_segments.Count != segmentNum)
            {
                if (_segments.Count < segmentNum)
                {
                    _segments.Add(new CurveSegment());
                }
                else if (_segments.Count > segmentNum)
                {
                    _segments.RemoveAt(_segments.Count - 1);
                }
            }

            _length = 0;
            await Task.Run(() =>
            {
                Vector2 current = GetPointAtPercentage(0);
                for (int i = 1; i <= segmentNum; i++)
                {
                    float t = i / (float)segmentNum;
                    Vector3 next = GetPointAtPercentage(t);

                    var segment = _segments[i - 1];
                    segment.Set(current, next, _length);
                    _length = segment.length + segment.prevTotalLength;

                    current = next;
                }
            });
        }

        public Vector2 GetPointAtPercentage(float t)
        {
            float angle = Mathf.Deg2Rad * 360 * t;
            float x = Mathf.Cos(angle) * width;
            float y = Mathf.Sin(angle) * height;

            return new Vector2(x, y);
        }

        public Vector2 GetPointAtDistance(float distance)
        {
            if (_segments.Count == 0) return Vector2.zero;

            CurveSegment segment = _segments.Find(s => s.length + s.prevTotalLength >= distance);
            if (segment == null) return _segments[^1].next;

            float t = Mathf.InverseLerp(0, segment.length, distance - segment.prevTotalLength);
            return Vector2.Lerp(segment.current, segment.next, t);
        }

        private void OnDrawGizmos()
        {
            if (_segments.Count == 0) return;

            Gizmos.color = Color.red;
            for (int i = 0; i < _segments.Count; i++)
            {
                Gizmos.DrawLine(_segments[i].current, _segments[i].next);
                Gizmos.DrawSphere(_segments[i].current, 0.1f);
                Gizmos.DrawSphere(_segments[i].next, 0.1f);
            }
        }
    }
}