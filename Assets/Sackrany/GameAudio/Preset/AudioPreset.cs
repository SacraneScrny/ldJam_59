using System;

using UnityEngine;

namespace Sackrany.GameAudio.Preset
{
    [Serializable]
    public struct AudioPreset
    {
        public AudioClip Clip;
        public AudioManager.PlayData Data;
    }
}