using Sackrany.GameAudio.Components;

namespace Sackrany.GameAudio.Preset
{
    public static class AudioPresetExtensions
    {
        public static RuntimeAudioComponent Prepare(this AudioPreset preset)
        {
            return preset.Clip.Prepare().Setup(preset.Data);
        }
    }
}