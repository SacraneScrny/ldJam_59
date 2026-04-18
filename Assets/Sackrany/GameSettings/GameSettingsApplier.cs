using Sackrany.ConfigSystem;
using Sackrany.GameSettings.Configs;
using Sackrany.Utils;

using UnityEngine;
using UnityEngine.InputSystem;

namespace Sackrany.GameSettings
{
    public class GameSettingsApplier : AManager<GameSettingsApplier>
    {
        protected override void OnManagerAwake()
        {
            //GameSettingsHelper.ApplyGraphics();
        }
        protected override void OnManagerDestroy()
        {
            
        }
    }
}
