using Sackrany.Utils;
using UnityEngine.Audio;

namespace Sackrany.GameAudio.Components
{
    public class GameAudioRuntimeData : AManager<GameAudioRuntimeData>
    {
        public AudioMixer Mixer;
        public AudioMixerGroup Master;
        public AudioMixerGroup Game;
        public AudioMixerGroup Music;
        public AudioMixerGroup UI;

        public static AudioMixerGroup GetMixerGroup(AudioManager.AudioGroup group = default)
            => group switch
            {
                AudioManager.AudioGroup.Game => Instance.Game,
                AudioManager.AudioGroup.Music => Instance.Music,
                AudioManager.AudioGroup.UI => Instance.UI,
                _ => Instance.Master
            };
    }
}