using System;

using Sackrany.ConfigSystem;

namespace Sackrany.GameAudio.Configurations
{
    [Serializable]
    public class GameAudioMetadataConfig : IConfig
    {
        public float MinDb = -80;
        public float MaxDb = 20;
        public float DbOffset = -20;
        public string MasterVolume = "MasterVolume";
        public string GameVolume = "GameVolume";
        public string MusicVolume = "MusicVolume";
        public string UIVolume = "UIVolume";
    }
}