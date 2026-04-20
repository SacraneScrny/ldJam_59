using System;

using R3;

using Sackrany.ConfigSystem;
using Sackrany.GameAudio;
using Sackrany.GameAudio.Configurations;
using Sackrany.GameInput;

using SackranyUI.Core.Base;
using SackranyUI.Core.Entities;

namespace Game.Logic.UI.Screen
{
    public class GameMainUIViewModel : ViewModel<GameMainUI>
    {
        [InitBind("mute")] bool _initMute;
        [InitBind("masterVolume")] float _initMasterVolume;
        
        [Bind("isSettingsOpen")] ReactiveProperty<bool> _isSettingsOpen = new(false);
        [Bind("isTutorialOpen")] ReactiveProperty<bool> _isTutorialOpen = new(false);
        [Bind("level")] ReactiveProperty<string> _level = new("LEVEL 1");
        
        protected override void OnInitialized()
        {
            Track(Difficulty.Instance.CurrentDifficulty.Subscribe((d) => _level.Value = $"LEVEL {d}"));
            var gameAudioConfig = ConfigGet<GameAudioConfig>.Value;
            _initMute = gameAudioConfig.Muted;
            _initMasterVolume = gameAudioConfig.MasterVolume;
            Open();
            
            InputManager.Player.Inventory.performed += _ => _isSettingsOpen.Value = !_isSettingsOpen.Value;
            InputManager.Player.Crouch.performed += _ => _isTutorialOpen.Value = !_isTutorialOpen.Value;
        }
        
        [Bind("masterVolume")]
        void MasterVolumeBind(float value)
        {
            ConfigSet<GameAudioConfig>.Do((c) => c.MasterVolume = value);
            AudioManager.ApplySettings(ConfigGet<GameAudioConfig>.Value);
        }
        [Bind("mute")]
        void MuteBind(bool value)
        {
            ConfigSet<GameAudioConfig>.Do((c) => c.Muted = value);
            AudioManager.ApplySettings(ConfigGet<GameAudioConfig>.Value);
        }    
    }
    
    [Serializable]
    public class GameMainUI : ViewModelTemplate<GameMainUIViewModel> { }
}