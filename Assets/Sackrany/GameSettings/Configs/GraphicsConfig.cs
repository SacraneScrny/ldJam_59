using Sackrany.ConfigSystem;

using UnityEngine;

using ShadowQuality = UnityEngine.Rendering.Universal.ShadowQuality;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

namespace Sackrany.GameSettings.Configs
{
    public class GraphicsConfig : IDynamicConfig
    {
        // Разрешение и дисплей
        public int          ResolutionX         { get; set; } = 1920;
        public int          ResolutionY         { get; set; } = 1080;
        public FullScreenMode FullscreenMode    { get; set; } = FullScreenMode.FullScreenWindow;
        public int          RefreshRate         { get; set; } = 60;
        public int          TargetFps           { get; set; } = 60;
        public bool         Vsync               { get; set; } = true;

        // Качество
        public int          QualityLevel        { get; set; } = 2;
        public float        RenderScale         { get; set; } = 1f;
        public int          AntiAliasing        { get; set; } = 2;  // 0, 2, 4, 8
        public bool         Hdr                 { get; set; } = true;

        // Тени
        public int  ShadowResolution { get; set; } = 1024; // 256, 512, 1024, 2048, 4096
        public float            ShadowDistance      { get; set; } = 100f;
        public int              ShadowCascades      { get; set; } = 2;  // 1, 2, 4

        // Текстуры
        public int          TextureQuality      { get; set; } = 0;  // 0 = full, 1 = half, 2 = quarter
        public int          AnisotropicLevel    { get; set; } = 4;
        public bool         AnisotropicFiltering { get; set; } = true;

        // Постобработка
        public bool         Bloom               { get; set; } = true;
        public bool         AmbientOcclusion    { get; set; } = true;
        public bool         MotionBlur          { get; set; } = false;
        public bool         DepthOfField        { get; set; } = false;
        public bool         ChromaticAberration { get; set; } = false;
        public bool         Vignette            { get; set; } = true;
        public bool         ColorGrading        { get; set; } = true;
        public bool         FilmGrain           { get; set; } = false;
        public bool         Tonemapping         { get; set; } = true;

        // Освещение
        public bool         RealtimeReflections { get; set; } = true;
        public bool         RealtimeGI          { get; set; } = false;
        public float        LodBias             { get; set; } = 1f;
        public int          MaximumLodLevel     { get; set; } = 0;

        // Прочее
        public int          ParticleRaycastBudget { get; set; } = 256;
        public bool         SoftParticles       { get; set; } = true;
        public bool         SoftVegetation      { get; set; } = true;
        public float        Brightness          { get; set; } = 1f;
        public float        Gamma               { get; set; } = 1f;
    }
}
