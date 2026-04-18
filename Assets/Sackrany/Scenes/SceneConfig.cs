using System;

using Sackrany.ConfigSystem;

namespace Sackrany.Scenes
{
    [Serializable]
    public class SceneConfig : IConfig
    {
        public string DefaultScene { get; set; } = "SampleScene";
    }
}