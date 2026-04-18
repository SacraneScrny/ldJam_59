using Sackrany.GameInput.Caches;
using Sackrany.GameSettings;

namespace Sackrany.GameInput
{
    public static partial class InputManager
    {
        static SackranyInputScheme _inputScheme;

        public static SackranyInputScheme.PlayerActions Player
            { get; private set; }

        public static SackranyInputScheme.UIActions UI
            { get; private set; }

        static PlayerActionsCache _playerCache;
        static UIActionsCache _uICache;

        public static PlayerActionsCache PlayerCache
            => _playerCache;

        public static UIActionsCache UICache
            => _uICache;

        public static void EnablePlayer()
        {
            Player.Enable();
            UI.Disable();
        }

        public static void EnableUI()
        {
            Player.Disable();
            UI.Enable();
        }

        static partial void InitGenerated()
        {
            _inputScheme = new();
            _inputScheme.Enable();

            Player = _inputScheme.Player;
            UI = _inputScheme.UI;

            _playerCache = new PlayerActionsCache(_inputScheme, _cancellation.Token);
            Register(_playerCache);
            _uICache = new UIActionsCache(_inputScheme, _cancellation.Token);
            Register(_uICache);
        }

        static partial void DisposeGenerated()
        {
            Player.Disable();
            UI.Disable();

            _inputScheme?.Disable();
            _inputScheme?.Dispose();
        }

        static partial void ApplySettingsGenerated()
        {
            Internal_ApplySettings(_inputScheme.asset);
        }
    }
}
