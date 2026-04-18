using Sackrany.ConfigSystem;
using Sackrany.GameSettings.Configs;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Sackrany.GameSettings
{
    public static class GameSettingsHelper
    {
        public static void ApplyGraphics()
        {
            var cfg    = ConfigGet<GraphicsConfig>.Value;
            var urp    = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline
                         ?? (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;

            // Разрешение и дисплей
            Screen.SetResolution(cfg.ResolutionX, cfg.ResolutionY, cfg.FullscreenMode,
                new RefreshRate { numerator = (uint)cfg.RefreshRate, denominator = 1 });
            Application.targetFrameRate  = cfg.TargetFps;
            QualitySettings.vSyncCount   = cfg.Vsync ? 1 : 0;

            // Качество
            QualitySettings.SetQualityLevel(cfg.QualityLevel, true);

            // Тени — через URP Asset
            if (urp != null)
            {
                urp.shadowDistance          = cfg.ShadowDistance;
                urp.shadowCascadeCount      = cfg.ShadowCascades;
                urp.mainLightShadowmapResolution = (int)cfg.ShadowResolution;
            }

            // Текстуры
            QualitySettings.globalTextureMipmapLimit = cfg.TextureQuality;
            QualitySettings.anisotropicFiltering     = cfg.AnisotropicFiltering
                ? AnisotropicFiltering.ForceEnable
                : AnisotropicFiltering.Disable;
            Texture.SetGlobalAnisotropicFilteringLimits(cfg.AnisotropicLevel, cfg.AnisotropicLevel);

            // Освещение
            QualitySettings.realtimeReflectionProbes = cfg.RealtimeReflections;
            QualitySettings.lodBias                  = cfg.LodBias;
            QualitySettings.maximumLODLevel          = cfg.MaximumLodLevel;

            // Прочее
            QualitySettings.particleRaycastBudget = cfg.ParticleRaycastBudget;
            QualitySettings.softParticles         = cfg.SoftParticles;
            QualitySettings.softVegetation        = cfg.SoftVegetation;
            
            DynamicConfigLoader.Save<GraphicsConfig>();
        }

        public static void ApplyControls(InputActionAsset asset)
        {
            var cfg = ConfigGet<ControlsConfig>.Value;
            if (!string.IsNullOrEmpty(cfg.BindingOverridesJson))
                asset.LoadBindingOverridesFromJson(cfg.BindingOverridesJson);
        }

        public static void SaveControls(InputActionAsset asset)
        {
            ConfigSet<ControlsConfig>.Do(c =>
                c.BindingOverridesJson = asset.SaveBindingOverridesAsJson());
            DynamicConfigLoader.Save<ControlsConfig>();
        }

        public static void ResetControls(InputActionAsset asset)
        {
            asset.RemoveAllBindingOverrides();
            ConfigSet<ControlsConfig>.Do(c => c.BindingOverridesJson = "");
            DynamicConfigLoader.Reset<ControlsConfig>();
        }
    }
}