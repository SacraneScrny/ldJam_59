using System;

using Sackrany.ConfigSystem;
using Sackrany.GameAudio.Components;
using Sackrany.GameAudio.Configurations;

using UnityEngine;

namespace Sackrany.GameAudio
{
    public static partial class AudioManager
    {
        static uint _nextId;
        public static uint GetId()
        {
            _nextId++;
            return _nextId;
        }
        
        static AudioPool _pool;
        static PlayData? _default;
        public static PlayData Default
        {
            get
            {
                _default ??= new PlayData()
                {
                    Delay = 0,
                    Priority = 0,
                    Volume = 1,
                    Pitch = 1,
                    MinDistance = 0,
                    MaxDistance = 500,
                    Spread = 0,
                    Group = AudioGroup.Game
                };
                return _default.Value;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            _default = null;
            _nextId = 0;
            _pool = new AudioPool();

            Application.quitting -= OnQuitting;
            Application.quitting += OnQuitting;
            
            GameAudioRuntimeData.ExecuteSafe((d) => InternalApplySettings(ConfigGet<GameAudioConfig>.Value, d)).Forget();
        }

        public static void Play(
            AudioClip clip, 
            Vector3 position = default,
            PlayData? playData = null,
            PlayType playType = default)
        {
            playData ??= Default;
            
            var audio = _pool.Pool();
            audio.transform.position = position;
            audio.Setup(playData.Value);
            audio.Play(clip, () => _pool.Release(audio), playType);
        }

        public static void ApplySettings(GameAudioConfig config) 
            => InternalApplySettings(config, GameAudioRuntimeData.Instance);
        static void InternalApplySettings(GameAudioConfig config, GameAudioRuntimeData data)
        {
            var meta = ConfigGet<GameAudioMetadataConfig>.Value;
            
            float Volume(float normalized)
            {
                if (config.Muted) return meta.MinDb;
                if (normalized < 0.001f) return meta.MinDb;
                float db = Mathf.Log10(normalized) * 20f + meta.DbOffset;
                db = Mathf.Clamp(db, meta.MinDb, meta.MaxDb);
                return db;
            }
            
            data.Mixer?.SetFloat(meta.MasterVolume, Volume(config.MasterVolume));
            data.Mixer?.SetFloat(meta.GameVolume, Volume(config.GameVolume * config.MasterVolume));
            data.Mixer?.SetFloat(meta.MusicVolume, Volume(config.MusicVolume * config.MasterVolume));
            data.Mixer?.SetFloat(meta.UIVolume, Volume(config.UIVolume * config.MasterVolume));
        }

        static void OnQuitting()
        {
            _pool.Dispose();
        }
        
        [Serializable]
        public struct PlayData
        {
            public float Delay;
            public int Priority;
            public float Volume;
            public float Pitch;
            public float MinDistance;
            public float MaxDistance;
            public float Spread;
            public AudioGroup Group;
        }
        public enum AudioGroup
        {
            None,
            Game,
            Music,
            UI
        }
    }
}