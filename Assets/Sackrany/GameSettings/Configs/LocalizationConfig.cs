using Sackrany.ConfigSystem;

namespace Sackrany.GameSettings.Configs
{
    public class LocalizationConfig : IDynamicConfig
    {
        public string Language  { get; set; } = "en";
        public bool   Subtitles { get; set; } = true;
    }
}
