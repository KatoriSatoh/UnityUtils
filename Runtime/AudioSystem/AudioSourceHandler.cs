using System.Collections;
using UnityEngine;

namespace UnityUtils
{
    public class AudioSourceHandler : MonoBehaviour
    {
        public AudioData Data { get; private set; }
        public AudioSource Source => _source;
        public bool IsActive => _source.isPlaying || IsPaused;
        public bool IsPaused { get; private set; } = false;

        private AudioSource _source;
        private Coroutine _playCoroutine;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _source = gameObject.GetOrAdd<AudioSource>();
        }

        private void OnDisable()
        {
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
                _playCoroutine = null;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
        }

        public void Init(AudioData data)
        {
            Data = data;

            _source.clip = data.clip;
            _source.outputAudioMixerGroup = data.group ? data.group : null;
            _source.volume = data.volume;
            _source.loop = data.loop;
            _source.pitch = data.randomPitch ? Random.Range(data.pitchRange.x, data.pitchRange.y) : 1f;

            gameObject.name = data.clip.name;
        }

        public void Play()
        {
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
            }

            _source.Play();
            _playCoroutine = StartCoroutine(WaitForEnd());
            IsPaused = false;
        }

        public void Play(float volume, bool isLoop, float pitch)
        {
            _source.volume = volume;
            _source.loop = isLoop;
            _source.pitch = pitch;

            Play();
        }

        public void Stop()
        {
            _source.Stop();

            IsPaused = false;
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
                _playCoroutine = null;
            }
        }

        public void Pause()
        {
            _source.Pause();

            IsPaused = true;
            if (_playCoroutine != null)
            {
                StopCoroutine(_playCoroutine);
                _playCoroutine = null;
            }
        }

        public void FadeVolume(float fromVolume = 1, float targetVolume = 0, float duration = .5f)
        {
            _source.volume = fromVolume;
            FadeVolume(targetVolume, duration);
        }

        public void FadeVolume(float targetVolume = 0, float duration = .5f)
        {
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeVolumeCoroutine(targetVolume, duration));
        }

        private IEnumerator FadeVolumeCoroutine(float targetVolume, float duration)
        {
            float startVolume = _source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                _source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                yield return null;
            }

            _source.volume = targetVolume;
            _fadeCoroutine = null;
        }

        private IEnumerator WaitForEnd()
        {
            yield return new WaitWhile(() => _source.isPlaying);
            Stop();
        }
    }
}