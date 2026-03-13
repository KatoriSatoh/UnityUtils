using UnityEngine;
using UnityEngine.Profiling;

namespace UnityUtils
{
    public class SystemInfoRenderer : MonoBehaviour
    {
        public static GUIStyle TextStyle => _displayStyle;
        public static bool IsDebugging => _isShowingInfo;

        private const uint kMB = 1024 * 1024;
        private const string kDisplayFormat = "FPS: {0}\nRAM: {1}/{2}MB";
        private const float kRefreshRate = .5f;

        private int _frameCounter = 0;
        private float _timePassed = 0;
        private static bool _isShowingInfo = false;
        private bool _isShowingVersion = false;

        private string _displayText;
        private static GUIStyle _displayStyle;
        private Rect _resizeRect = new(0, 0, 1, 1);
        private Rect _displayRect;
        private Rect _versionRect;

        private void Awake()
        {
            _displayRect = new Rect(Screen.width / 2f, 5, 0, 0);
            _versionRect = new Rect(Screen.width / 2f, Screen.height - 30, 0, 0);
            _displayStyle = new GUIStyle
            {
                fontSize = 22,
                normal = { textColor = Color.white },
                alignment = TextAnchor.UpperCenter
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                _isShowingInfo = !_isShowingInfo;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                _isShowingVersion = !_isShowingVersion;
            }

            _timePassed += Time.unscaledDeltaTime;
            if (_timePassed < kRefreshRate)
            {
                _frameCounter++;
                return;
            }

            _frameCounter++;
            var fps = Mathf.RoundToInt(_frameCounter / _timePassed);
            var ramUsage = Profiler.GetTotalAllocatedMemoryLong() / kMB;
            _displayText = string.Format(kDisplayFormat, fps, ramUsage, SystemInfo.systemMemorySize);

            _frameCounter = 0;
            _timePassed = 0;
        }

        public void Resize(Rect rect)
        {
            _resizeRect = rect;

            _displayRect.x = Screen.width * (rect.x + rect.width / 2);
            _displayRect.y = Screen.height * (1 - rect.y - rect.height) + 5;

            _versionRect.x = Screen.width * (rect.x + rect.width / 2);
            _versionRect.y = Screen.height * (1 - rect.y) - 30;

            _displayStyle.fontSize = (int)(Screen.height * rect.height * .03f);
        }

        private void OnGUI()
        {
            if (!_isShowingInfo && !_isShowingVersion) return;

            if (Application.isEditor)
            {
                Resize(_resizeRect);
            }

            if (_isShowingInfo) GUIHelper.LabelWithBorder(_displayRect, _displayText, _displayStyle, Color.black);
            if (_isShowingVersion) GUIHelper.LabelWithBorder(_versionRect, Application.version, _displayStyle, Color.black);
        }
    }
}