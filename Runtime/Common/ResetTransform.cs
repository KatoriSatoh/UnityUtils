using UnityEditor;
using UnityEngine;

namespace UnityUtils
{
    public class ResetTransform : MonoBehaviour
    {
        [SerializeField] private bool disableOnAwake = true;
        private void Awake()
        {
            // Set the canvas element's position to zero
            if (TryGetComponent<RectTransform>(out var rectTransform))
            {
                rectTransform.anchoredPosition = Vector2.zero;
            }
            else
            {
                transform.position = transform.position.With(x: 0, y: 0);
            }

            if (disableOnAwake)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDrawGizmos()
        {
            // Draw the camera aspect in Gizmos
            Camera camera = Camera.main;
            if (camera != null)
            {
                Gizmos.color = Color.green;
                float aspect = camera.aspect;
                float camHeight = camera.orthographicSize * 2;
                float camWidth = camHeight * aspect;
                Gizmos.DrawWireCube(transform.position, new Vector3(camWidth, camHeight, 0));

#if UNITY_EDITOR
                // Calculate top right position in world space
                Vector3 topRight = transform.position + new Vector3(-camWidth / 2 + .2f, camHeight / 2 - .5f, 0);
                Handles.Label(topRight, name);
#endif
            }
        }
    }
}
