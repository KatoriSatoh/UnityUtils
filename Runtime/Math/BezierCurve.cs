using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityUtils
{
    /// <summary>
    /// Represents a Bezier curve defined by a start point, end point, control point, and weight.
    /// </summary>
    public class BezierCurve
    {
        /// <summary>
        /// Gets the start point of the curve.
        /// </summary>
        public Vector3 Start => _startPoint;

        /// <summary>
        /// Gets the end point of the curve.
        /// </summary>
        public Vector3 End => _endPoint;

        /// <summary>
        /// Gets the control point of the curve.
        /// </summary>
        public Vector3 Control => _controlPoint;

        /// <summary>
        /// Gets the weight of the curve.
        /// </summary>
        public float Weight => _weight;

        /// <summary>
        /// Gets the length of the curve.
        /// </summary>
        public float Length { get; private set; }

        /// <summary>
        /// Gets the segments of the curve.
        /// </summary>
        public List<CurveSegment> Segments => _segments;

        private List<CurveSegment> _segments = new();
        private Vector3 _startPoint;
        private Vector3 _endPoint;
        private Vector3 _controlPoint;
        private float _weight;

        public BezierCurve()
        {
            _startPoint = Vector3.zero;
            _endPoint = Vector3.zero;
            _controlPoint = Vector3.zero;
            _weight = 1f;
        }

        public BezierCurve(Vector3 start, Vector3 end, Vector3 control, float weight = 1f)
        {
            _startPoint = start;
            _endPoint = end;
            _controlPoint = control;
            _weight = weight;
        }

        public void Create(Vector3 start, Vector3 end, Vector3 control, float weight = 1f)
        {
            _startPoint = start;
            _endPoint = end;
            _controlPoint = control;
            _weight = weight;
        }

        /// <summary>
        /// Asynchronously calculates the segments of the Bezier curve.
        /// </summary>
        /// <param name="count">The number of segments to divide the curve into. Default is 100.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task CalculatePathSegments(int count = 100)
        {
            while (_segments.Count != count)
            {
                if (_segments.Count < count)
                {
                    _segments.Add(new CurveSegment());
                }
                else if (_segments.Count > count)
                {
                    _segments.RemoveAt(_segments.Count - 1);
                }
            }

            Length = 0;
            await Task.Run(() =>
            {
                Vector3 current = _startPoint;
                for (int i = 1; i <= _segments.Count; i++)
                {
                    float t = i / (float)count;
                    Vector3 next = GetPointAtPercentage(t);

                    var segment = _segments[i - 1];
                    segment.Set(current, next, Length);
                    Length = segment.length + segment.prevTotalLength;

                    current = next;
                }
            });
        }

        /// <summary>
        /// Returns the point on the Bezier curve at a specified distance from the start of the curve.
        /// </summary>
        /// <param name="distance">The distance from the start of the curve.</param>
        /// <returns>The point on the curve at the specified distance.</returns>
        public Vector3 GetPointAtDistance(float distance)
        {
            if (_segments.Count == 0) return Vector3.zero;

            CurveSegment segment = _segments.Find(s => s.length + s.prevTotalLength >= distance);
            if (segment == null) return _segments[^1].next;

            float t = Mathf.InverseLerp(0, segment.length, distance - segment.prevTotalLength);
            return Vector3.Lerp(segment.current, segment.next, t);
        }

        /// <summary>
        /// Returns the point on the Bezier curve at a specified percentage along the curve.
        /// </summary>
        /// <param name="t">The percentage along the curve, where 0 is the start and 1 is the end.</param>
        /// <returns>The point on the curve at the specified percentage.</returns>
        public Vector3 GetPointAtPercentage(float t)
        {
            return CalculateBezierPoint(t, _startPoint, _controlPoint, _endPoint, _weight);
        }

        private Vector3 CalculateBezierPoint(float t, Vector3 start, Vector3 control, Vector3 end, float weight)
        {
            float u = 1 - t;
            return u * u * start + 2 * u * weight * t * control + t * t * end;
        }

        private Vector3 CalculateBezierTangent(float t, Vector3 start, Vector3 control, Vector3 end, float weight)
        {
            float u = 1 - t;
            Vector3 weightedControl = weight * control;
            Vector3 tangent =
                2 * u * (weightedControl - start) +
                2 * t * (end - weightedControl);
            return tangent.normalized;
        }
    }

    /// <summary>
    /// Represents a segment of a Bezier curve.
    /// </summary>
    public class CurveSegment
    {
        /// <summary>
        /// Gets or sets the current point of the segment.
        /// </summary>
        public Vector3 current;

        /// <summary>
        /// Gets or sets the next point of the segment.
        /// </summary>
        public Vector3 next;

        /// <summary>
        /// Gets or sets the length of the segment.
        /// </summary>
        public float length;

        /// <summary>
        /// Gets or sets the total length of the previous segments.
        /// </summary>
        public float prevTotalLength;

        public CurveSegment()
        {
            current = Vector3.zero;
            next = Vector3.zero;
            length = 0;
            prevTotalLength = 0;
        }

        public CurveSegment(Vector3 current, Vector3 next, float length, float prevTotalLength)
        {
            this.current = current;
            this.next = next;
            this.length = length;
            this.prevTotalLength = prevTotalLength;
        }

        public void Set(Vector3 current, Vector3 next, float prevTotalLength)
        {
            this.current = current;
            this.next = next;
            length = (next - current).magnitude;
            this.prevTotalLength = prevTotalLength;
        }
    }
}
