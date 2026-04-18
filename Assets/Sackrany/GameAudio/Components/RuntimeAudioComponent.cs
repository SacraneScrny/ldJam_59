using System;

using JetBrains.Annotations;

using UnityEngine;

namespace Sackrany.GameAudio.Components
{
    [AddComponentMenu("")]
    [RequireComponent(typeof(AudioSource))]
    public class RuntimeAudioComponent : MonoBehaviour
    {
        uint _id;
        bool _hasId;
        public uint GetId()
        {
            if (!_hasId)
            {
                _id = AudioManager.GetId();
                _hasId = true;
            }

            return _id;
        }
        
        AudioSource _audioSource;
        float _length;
        float _currentLength;
        float? _currentDelay;
        bool _isPlaying;
        PlayType _playType;
        Action _onFinished;
        
        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = GameAudioRuntimeData.GetMixerGroup();
            _audioSource.playOnAwake = false;
        }
        
        public RuntimeAudioComponent Clip(AudioClip clip)
        {
            PrepareCore(clip);
            return this;
        }
        public RuntimeAudioComponent Setup(AudioManager.PlayData playData)
        {
            SetupCore(playData);
            return this;
        }
        public RuntimeAudioComponent Priority(int value)
        {
            _audioSource.priority = value;
            return this;
        }
        public RuntimeAudioComponent Volume(float value)
        {
            _audioSource.volume = value;
            return this;
        }
        public RuntimeAudioComponent Pitch(float value)
        {
            _audioSource.pitch = value;
            return this;
        }
        public RuntimeAudioComponent Distance(float minValue, float maxValue)
        {
            _audioSource.minDistance = minValue;
            _audioSource.maxDistance = maxValue;
            return this;
        }
        public RuntimeAudioComponent Spread(float value)
        {
            _audioSource.spread = value;
            return this;
        }
        public RuntimeAudioComponent Group(AudioManager.AudioGroup group)
        {
            _audioSource.outputAudioMixerGroup = GameAudioRuntimeData.GetMixerGroup(group);
            return this;
        }
        
        public RuntimeAudioComponent AtPosition(Vector3 position)
        {
            transform.position = position;
            return this;
        }
        public RuntimeAudioComponent CallbackOnStarted(Action<RuntimeAudioComponent> action)
        {
            OnStarted += action;
            return this;
        }
        public RuntimeAudioComponent CallbackOnFinished(Action<RuntimeAudioComponent> action)
        {
            OnFinished += action;
            return this;
        }

        void PrepareCore(AudioClip clip)
        {
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = clip;
        }
        void SetupCore(AudioManager.PlayData playData)
        {
            _audioSource.priority = playData.Priority;
            _audioSource.pitch = playData.Pitch;
            _audioSource.volume = playData.Volume;
            _audioSource.minDistance = playData.MinDistance;
            _audioSource.maxDistance = playData.MaxDistance;
            _audioSource.spread = playData.Spread;
            _audioSource.outputAudioMixerGroup = GameAudioRuntimeData.GetMixerGroup(playData.Group);
            _currentDelay = playData.Delay <= Time.deltaTime ? null : playData.Delay;
        }
        public void Play(
            [CanBeNull] AudioClip clip, 
            Action onFinished, 
            PlayType playType = default
            )
        {
            clip ??= _audioSource.clip;
            _length = clip.length;
            if (_length <= Time.deltaTime)
            {
                onFinished?.Invoke();
                return;
            }
            
            _playType = playType;
            _currentLength = 0;
            _onFinished = onFinished;
            _isPlaying = true;
            
            _audioSource.clip = clip;
            if (_currentDelay == null)
            {
                _audioSource.Play();
                OnStarted?.Invoke(this);
            }
            _audioSource.loop = playType == PlayType.Repeat;
        }
        
        void Update()
        {
            if (!_isPlaying) return;
            if (_currentDelay != null)
            {
                _currentDelay -= Time.deltaTime;
                if (_currentDelay <= 0)
                {
                    _currentDelay = null;
                    OnStarted?.Invoke(this);
                    _audioSource.Play();
                }
                return;
            }
            if (_currentLength >= _length)
            {
                if (_playType == PlayType.Repeat)
                {
                    _currentLength = 0;
                    return;
                }
                OnFinished?.Invoke(this);
                _onFinished?.Invoke();
                return;
            }
            _currentLength += Time.deltaTime;
        }
        
        public void OnPooled()
        {
            _currentDelay = null;
            _isPlaying = false;
            _currentLength = 0;
            _length = 0;
            _playType = default;
            _onFinished = null;
            gameObject.SetActive(true);
        }
        public void OnReleased()
        {
            _audioSource.Stop();
            OnStarted = null;
            OnFinished = null;
            gameObject.SetActive(false);
        }

        internal event Action<RuntimeAudioComponent> OnStarted;
        internal event Action<RuntimeAudioComponent> OnFinished;
    }
}