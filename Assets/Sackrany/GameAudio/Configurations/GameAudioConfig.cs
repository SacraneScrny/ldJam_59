using System;

using Sackrany.ConfigSystem;

namespace Sackrany.GameAudio.Configurations
{
    [Serializable]
    public class GameAudioConfig : IDynamicConfig
    {
        public float MasterVolume = 1f;
        public float GameVolume = 1f;
        public float MusicVolume = 1f;
        public float UIVolume = 1f;
        public bool Muted = false;
    }
}