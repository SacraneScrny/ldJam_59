using Sackrany.ConfigSystem;

namespace Sackrany.GameSettings.Configs
{
    public class AudioConfig : IDynamicConfig
    {
        public float MasterVolume { get; set; } = 1f;
        public float MusicVolume  { get; set; } = 0.8f;
        public float SfxVolume    { get; set; } = 1f;
        public float UiVolume     { get; set; } = 1f;
        public bool  Muted        { get; set; } = false;
    }
}
