using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils
{
    public class RunningTrack : IPath
    {
        public List<PathSegment> Segments => _segments;
        private readonly List<PathSegment> _segments = new();

        public float TotalLength => _totalLength;
        private readonly float _totalLength;

        private float _halfWidth;
        private float _radius;

        public RunningTrack(float width, float height, int curveSegments)
        {
            _totalLength = 0;

            _halfWidth = width / 2;
            _radius = height / 2;

            var prevPoint = new Vector2(_halfWidth + _radius, 0);
            float stepAngle = 180f / curveSegments;
            for (int i = 1; i <= curveSegments * 2; i++)
            {
                float angle = i * stepAngle;
                float dx = Mathf.Cos(angle * Mathf.Deg2Rad) * _radius;
                float dy = Mathf.Sin(angle * Mathf.Deg2Rad) * _radius;
                float x = dx >= 0 ? _halfWidth + dx : -_halfWidth + dx;
                var point = new Vector2(x, dy);
                _segments.Add(new PathSegment(prevPoint, point, _totalLength));

                _totalLength += Vector2.Distance(prevPoint, point);
                prevPoint = point;
            }
        }

        public Vector3 GetPositionAtDistance(float distance)
        {
            if (_segments.Count == 0) return Vector3.zero;

            for (int i = 0; i < _segments.Count; i++)
            {
                if (_segments[i].Length + _segments[i].PreviousTotalLength >= distance)
                {
                    float t = Mathf.InverseLerp(0, _segments[i].Length, distance - _segments[i].PreviousTotalLength);
                    return Vector3.Lerp(_segments[i].Current, _segments[i].Next, t);
                }
            }

            return _segments[^1].Next;
        }

        public float GetDistanceAtPosition(Vector2 pos)
        {
            if (_segments.Count == 0) return 0;

            float distance = 0;
            for (int i = 0; i < _segments.Count; i++)
            {
                float tX = pos.x.InverseLerpUnclamped(_segments[i].Current.x, _segments[i].Next.x);
                float tY = pos.y.InverseLerpUnclamped(_segments[i].Current.y, _segments[i].Next.y);
                if (tX >= 0 && tX <= 1 && tY >= 0 && tY <= 1)
                {
                    distance += Vector2.Distance(_segments[i].Current, pos);
                    return distance;
                }
                else
                {
                    distance += _segments[i].Length;
                }
            }

            return _totalLength;
        }

        public float GetAngleAtPoint(Vector2 point)
        {
            if (point.y.EqualsApproximately(-_radius, .01f))
            {
                return 0;
            }

            if (point.y.EqualsApproximately(_radius, .01f))
            {
                return 180;
            }

            float dx = point.x > 0 ? point.x - _halfWidth : point.x + _halfWidth;
            return Mathf.Atan2(point.y, dx) * Mathf.Rad2Deg;
        }

        public Vector3 GetPositionAtPercentage(float t)
        {
            throw new System.NotImplementedException();
        }
    }
}