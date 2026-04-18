using System;

using Sackrany.GameAudio.Components;

using UnityEngine;

namespace Sackrany.GameAudio
{
    public static partial class AudioManager
    {        
        public static RuntimeAudioComponent Prepare(this AudioClip clip)
        {
            var audio = _pool.Pool();
            audio.Clip(clip);
            return audio.Setup(Default);
        }
        public static void Forget(
            this AudioClip clip,
            PlayType playType = default,
            Vector3 position = default,
            float volume = 1.0f)
        {
            clip
                .Prepare()
                .AtPosition(position)
                .Volume(volume)
                .Play(playType);
        }
        public static void Play(this RuntimeAudioComponent audio, PlayType playType = default)
        {
            audio.Play(null, () => _pool.Release(audio), playType);
        }
    }
}