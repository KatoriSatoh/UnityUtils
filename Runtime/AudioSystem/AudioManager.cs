using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityUtils
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioDatabaseSO audioDatabase;
        [SerializeField] private int initPoolSize = 10;

        private readonly Dictionary<int, AudioData> _audioDataDict = new();
        private readonly List<AudioSourceHandler> _activeSources = new();
        private readonly Queue<AudioSourceHandler> _restingSources = new();

        protected override void Awake()
        {
            base.Awake();

            foreach (var audioData in audioDatabase.AudioDatas)
            {
                _audioDataDict.Add(audioData.id, audioData);
            }

            for (int i = 0; i < initPoolSize; i++)
            {
                AddAudioSourceHandler();
            }

            // AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
        }

        private void OnDestroy()
        {
            // AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigurationChanged;
        }

        private void Update()
        {
            for (int i = 0; i < _activeSources.Count; i++)
            {
                if (!_activeSources[i].IsActive)
                {
                    var source = _activeSources[i];
                    ReturnSourceHandlerToPool(source);

                    i--;
                }
            }
        }

        public AudioSourceHandler GetActiveAudioSourceHandler(string audioName)
        {
            var audioData = GetAudioDataByName(audioName);
            if (audioData == null) return null;

            return _activeSources.FirstOrDefault(s => s.Data.id == audioData.id);
        }

        public bool IsPlaying(string audioName)
        {
            var audioData = GetAudioDataByName(audioName);
            if (audioData == null) return false;

            return _activeSources.Any(source => source.Data.id == audioData.id);
        }

        public void Mute(string groupName = "Master")
        {
            audioDatabase.Mixer.SetFloat(groupName, -80f);
        }

        public void Unmute(string groupName = "Master")
        {
            audioDatabase.Mixer.SetFloat(groupName, 0f);
        }

        public void SetVolume(float volume, string groupName = "Master")
        {
            audioDatabase.Mixer.SetFloat(groupName, volume.Remap(0f, 1f, -80f, 0f));
        }

        public void SetVolume(float volume, float duration, string groupName = "Master")
        {
            // audioDatabase.Mixer.DOSetFloat(groupName, volume.Remap(0f, 1f, -80f, 0f), duration);
        }

        public void FadeVolume(string audioName, float to, float duration)
        {
            var audioSource = GetActiveAudioSourceHandler(audioName);
            if (audioSource == null) return;

            audioSource.FadeVolume(to, duration);
        }

        public void FadeVolume(string audioName, float from, float to, float duration)
        {
            var audioSource = GetActiveAudioSourceHandler(audioName);
            if (audioSource == null) return;

            audioSource.FadeVolume(from, to, duration);
        }

        public void Play(string audioName)
        {
            var audioData = GetAudioDataByName(audioName);
            if (audioData == null) return;

            Play(audioData);
        }

        public void Play(string audioName, float volume = 1f, bool isLoop = false, float pitch = 1f)
        {
            var audioData = GetAudioDataByName(audioName);
            if (audioData == null) return;

            var source = GetAudioSourceHandler();
            source.Init(audioData);
            source.Play(volume, isLoop, pitch);
        }

        public void Play(AudioClip clip, string groupName = "Master", float volume = 1f, bool isLoop = false, float pitch = 1f)
        {
            var audioData = GetAudioDataByName(clip.name);
            if (audioData == null)
            {
                var clipName = clip.name.ToEnumEntry();
                var clipId = clipName.GetHashCode();
                if (_audioDataDict.TryGetValue(clipId, out audioData))
                {
                    Logger.LogWarning<AudioManager>("AudioData already exists for clip: {0}", clip.name);
                    return;
                }

                audioData = new AudioData
                {
                    clip = clip,
                    id = clipId,
                    name = clipName,
                    group = audioDatabase.Mixer.FindMatchingGroups(groupName)[0],
                    loop = isLoop,
                };
                _audioDataDict.Add(clipId, audioData);
            }

            var source = GetAudioSourceHandler();
            source.Init(audioData);
            source.Play(volume, isLoop, pitch);
        }

        private void Play(AudioData audioData)
        {
            var source = GetAudioSourceHandler();
            source.Init(audioData);
            source.Play();
        }

        public void Stop(string audioName)
        {
            var audioSource = GetActiveAudioSourceHandler(audioName);
            if (audioSource == null) return;

            audioSource.Stop();
        }

        public void StopGroup(string groupName)
        {
            foreach (var source in _activeSources.ToList())
            {
                if (source.Data.group.name.Equals(groupName))
                {
                    source.Stop();
                }
            }
        }

        public void StopAll()
        {
            foreach (var source in _activeSources.ToList())
            {
                source.Stop();
            }
        }

        public void Pause(string audioName)
        {
            var audioSource = GetActiveAudioSourceHandler(audioName);
            if (audioSource == null) return;

            audioSource.Pause();
        }

        public void Resume(string audioName)
        {
            var audioSource = GetActiveAudioSourceHandler(audioName);
            if (audioSource == null) return;

            if (audioSource.IsPaused) audioSource.Play();
        }

        private AudioData GetAudioDataByName(string audioName)
        {
            if (!_audioDataDict.TryGetValue(audioName.GetHashCode(), out var audioData))
            {
                Logger.LogError<AudioManager>("AudioData not found: {0}", audioName);
                return null;
            }

            return audioData;
        }

        private void AddAudioSourceHandler()
        {
            var source = new GameObject().AddComponent<AudioSourceHandler>();
            _restingSources.Enqueue(source);

            source.transform.SetParent(transform);
            source.gameObject.SetActive(false);
        }

        private AudioSourceHandler GetAudioSourceHandler()
        {
            if (_restingSources.Count == 0)
            {
                AddAudioSourceHandler();
            }

            var restingSource = _restingSources.Dequeue();
            _activeSources.Add(restingSource);

            restingSource.gameObject.SetActive(true);
            return restingSource;
        }

        private void ReturnSourceHandlerToPool(AudioSourceHandler handler)
        {
            handler.gameObject.SetActive(false);
            _activeSources.Remove(handler);
            _restingSources.Enqueue(handler);
        }

        // private void PlayAt(AudioData audioData, float time)
        // {
        //     var source = GetAudioSourceHandler();
        //     source.Init(audioData);
        //     source.Play();
        //     source.Source.time = time;
        // }

        private void OnAudioConfigurationChanged(bool deviceWasChanged)
        {
            Logger.LogWarning<AudioManager>("Audio configuration changed. Replaying all audio sources.");

            var activeList = _activeSources.Clone();
            // var times = activeList.Select(source => source.Source.time).ToList();
            StopAll();

            foreach (var source in activeList)
            {
                Play(source.Data);
            }

            // for (int i = 0; i < activeList.Count; i++)
            // {
            //     PlayAt(activeList[i].Data, times[i]);
            // }
        }
    }
}