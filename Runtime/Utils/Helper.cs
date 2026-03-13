using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtils
{
    public static class Helper
    {
        public static readonly float semiTone = Mathf.Pow(2, 1f / 12);
        public static readonly int[] pentatonicScale = { 0, 2, 4, 7, 9 };

        public static readonly System.Random Rand = new();
        private static readonly Dictionary<float, WaitForSeconds> _waitForSecondsCache = new(100, new FloatComparer());

        /// <summary>
        /// Returns a WaitForSeconds object for the specified duration. </summary>
        /// <param name="seconds">The duration in seconds to wait.</param>
        /// <returns>A WaitForSeconds object.</returns>
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (seconds < 1f / Application.targetFrameRate) return null;

            if (_waitForSecondsCache.TryGetValue(seconds, out var forSeconds)) return forSeconds;

            var waitForSeconds = new WaitForSeconds(seconds);
            _waitForSecondsCache[seconds] = waitForSeconds;
            return waitForSeconds;
        }

        /// <summary>
        /// Waits for the specified number of seconds and then executes the provided action.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour instance.</param>
        /// <param name="seconds">The number of seconds to wait.</param>
        /// <param name="action">The action to execute.</param>
        public static void WaitThenExecute(this MonoBehaviour monoBehaviour, float seconds, Action action)
        {
            monoBehaviour.StartCoroutine(WaitThenExecuteCoroutine(seconds, action));
        }

        /// <summary>
        /// Waits for a specified number of seconds and then executes an action.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait.</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>An IEnumerator used for coroutine execution.</returns>
        private static IEnumerator WaitThenExecuteCoroutine(float seconds, Action action)
        {
            yield return GetWaitForSeconds(seconds);
            action?.Invoke();
        }

        class FloatComparer : IEqualityComparer<float>
        {
            public bool Equals(float x, float y) => Mathf.Abs(x - y) <= Mathf.Epsilon;
            public int GetHashCode(float obj) => obj.GetHashCode();
        }
    }
}