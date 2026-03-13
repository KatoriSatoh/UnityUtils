using System;
using UnityEngine;

namespace UnityUtils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimation : MonoBehaviour
    {
        [SerializeField, OnValueChanged(nameof(UpdateDefaultFrame))] private Sprite[] sprites;
        [SerializeField, OnValueChanged(nameof(UpdateDefaultFrame))] private bool randomStartFrame = false;
        [SerializeField] private bool setByFrameRate = false;
        [SerializeField, HideIf(nameof(setByFrameRate)), OnValueChanged(nameof(UpdateFPS))] private float duration = 1f;
        [SerializeField, ShowIf(nameof(setByFrameRate)), OnValueChanged(nameof(UpdateFPS))] private float framesPerSecond = 30f;
        [SerializeField] private bool isLoop = true;
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool useTextureOffset = false;
        [SerializeField, ShowIf(nameof(useTextureOffset))] private Vector2 textureOffsetSpeed = Vector2.zero;

        public bool JustFinished => _justFinished;

        private SpriteRenderer _spriteRenderer;
        private Material _spriteMaterial;
        private int _currentFrame = 0;
        private float _fps = 0f;
        private float _timer = 0f;
        private float _duration = 0f;
        private float _animationSpeed = 1f;
        private Vector2 _currentOffset = Vector2.zero;
        private bool _isPlaying = false;
        private bool _justFinished = false;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteMaterial = _spriteRenderer.material;
            _isPlaying = playOnAwake;

            UpdateDefaultFrame();
            UpdateFPS();
        }

        private void Update()
        {
            if (!_isPlaying || _fps <= 0f) return;

            _timer += _animationSpeed * Time.deltaTime;
            if (isLoop && _timer > _duration)
            {
                _timer -= _duration;
            }

            var previousFrame = _currentFrame;
            _currentFrame = Math.Clamp(Mathf.FloorToInt(_timer * _fps), 0, sprites.Length - 1);
            _justFinished = previousFrame > _currentFrame;
            _spriteRenderer.sprite = sprites[_currentFrame];

            if (useTextureOffset)
            {
                _currentOffset += textureOffsetSpeed * Time.deltaTime;
                if (_currentOffset.x > 1f) _currentOffset.x -= 1f;
                if (_currentOffset.y > 1f) _currentOffset.y -= 1f;
                _spriteMaterial.SetTextureOffset("_MainTex", _currentOffset);
            }

            if (!isLoop && _currentFrame == sprites.Length - 1)
            {
                _isPlaying = false;
                _justFinished = true;
            }
        }

        public void SetSpeed(float speed)
        {
            _animationSpeed = speed;
        }

        public void Play()
        {
            _currentFrame = randomStartFrame ? Helper.Rand.Next(0, sprites.Length) : 0;
            _timer = 0f;
            _isPlaying = true;
            _justFinished = false;
        }

        private void UpdateDefaultFrame()
        {
            if (sprites.Length == 0) return;
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();

            _currentFrame = randomStartFrame ? Helper.Rand.Next(0, sprites.Length) : 0;
            _spriteRenderer.sprite = sprites[_currentFrame];
        }

        private void UpdateFPS()
        {
            if (setByFrameRate)
            {
                _fps = framesPerSecond;
                _duration = sprites.Length > 0 ? sprites.Length / _fps : 0f;
            }
            else
            {
                _fps = sprites.Length > 0 ? sprites.Length / duration : 0f;
                _duration = duration;
            }
        }
    }
}
