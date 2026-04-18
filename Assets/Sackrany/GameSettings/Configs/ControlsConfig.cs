using Sackrany.ConfigSystem;

namespace Sackrany.GameSettings.Configs
{
    public class ControlsConfig : IDynamicConfig
    {
        public float  MouseSensitivity     { get; set; } = 1f;
        public bool   InvertY              { get; set; } = false;
        public string BindingOverridesJson { get; set; } = "";
    }
}
