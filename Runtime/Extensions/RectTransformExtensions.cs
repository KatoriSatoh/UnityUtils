using UnityEngine;

namespace UnityUtils
{
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Fits the RectTransform to the specified maximum width and height.
        /// </summary>
        /// <param name="rectTransform">The RectTransform to fit.</param>
        /// <param name="maxWidth">The maximum width to fit the RectTransform to.</param>
        /// <param name="maxHeight">The maximum height to fit the RectTransform to.</param>
        /// <remarks>
        /// This method scales the RectTransform to fit within the specified width and height while maintaining the aspect ratio.
        /// </remarks>
        public static void Fit(this RectTransform rectTransform, float maxWidth, float maxHeight)
        {
            float scale = Mathf.Min(maxWidth / rectTransform.sizeDelta.x, maxHeight / rectTransform.sizeDelta.y);
            rectTransform.sizeDelta *= scale;
        }

        /// <summary>
        /// Fits the RectTransform to the specified parent RectTransform.
        /// </summary>
        /// <param name="rectTransform">The RectTransform to fit.</param>
        /// <param name="parent">The parent RectTransform to fit the RectTransform to.</param>
        /// <remarks>
        /// This method scales the RectTransform to fit within the specified parent RectTransform while maintaining the aspect ratio.
        /// </remarks>
        public static void Fit(this RectTransform rectTransform, RectTransform parent)
        {
            float scale = Mathf.Min(parent.rect.width / rectTransform.sizeDelta.x, parent.rect.height / rectTransform.sizeDelta.y);
            rectTransform.sizeDelta *= scale;
        }
    }
}
