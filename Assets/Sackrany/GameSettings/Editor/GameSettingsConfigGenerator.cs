#if UNITY_EDITOR
using System.IO;
using Newtonsoft.Json;
using Sackrany.ConfigSystem;
using Sackrany.GameSettings.Configs;

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Sackrany.GameSettings.Editor
{
    public static class GameSettingsConfigGenerator
    {
        const string ResourcesPath = "Assets/Resources/Configs";

        [MenuItem("Sackrany/GameSettings/Generate Default Configs")]
        static void GenerateAll()
        {
            Directory.CreateDirectory(ResourcesPath);

            GenerateFromInstance(BuildGraphicsConfig());
            GenerateFromInstance(new AudioConfig());
            GenerateFromInstance(new ControlsConfig());
            GenerateFromInstance(new LocalizationConfig());

            AssetDatabase.Refresh();
            Debug.Log("[GameSettingsConfigGenerator] Done");
        }

        static GraphicsConfig BuildGraphicsConfig()
        {
            var urp = (UniversalRenderPipelineAsset)QualitySettings.renderPipeline
                   ?? (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;

            var res = Screen.currentResolution;

            var cfg = new GraphicsConfig
            {
                // Разрешение и дисплей
                ResolutionX    = res.width,
                ResolutionY    = res.height,
                FullscreenMode = Screen.fullScreenMode,
                RefreshRate    = (int)res.refreshRateRatio.numerator,
                TargetFps      = Application.targetFrameRate == -1 ? 60 : Application.targetFrameRate,
                Vsync          = QualitySettings.vSyncCount > 0,

                // Качество
                QualityLevel   = QualitySettings.GetQualityLevel(),
                RenderScale    = urp != null ? urp.renderScale : 1f,
                AntiAliasing   = urp != null ? (int)urp.msaaSampleCount : 2,

                // Тени
                ShadowDistance   = urp != null ? urp.shadowDistance : 100f,
                ShadowCascades   = urp != null ? urp.shadowCascadeCount : 2,
                ShadowResolution = urp != null ? urp.mainLightShadowmapResolution : 1024,

                // Текстуры
                TextureQuality       = QualitySettings.globalTextureMipmapLimit,
                AnisotropicFiltering = QualitySettings.anisotropicFiltering != AnisotropicFiltering.Disable,
                AnisotropicLevel     = 4,

                // Освещение
                RealtimeReflections = QualitySettings.realtimeReflectionProbes,
                LodBias             = QualitySettings.lodBias,
                MaximumLodLevel     = QualitySettings.maximumLODLevel,

                // Прочее
                ParticleRaycastBudget = QualitySettings.particleRaycastBudget,
                SoftParticles         = QualitySettings.softParticles,
                SoftVegetation        = QualitySettings.softVegetation,
            };

            return cfg;
        }

        static void GenerateFromInstance<T>(T instance) where T : class, IDynamicConfig
        {
            var path = Path.Combine(ResourcesPath, $"{typeof(T).Name}.json");

            if (File.Exists(path))
                if (!EditorUtility.DisplayDialog("Overwrite?", $"{typeof(T).Name}.json exists. Overwrite?", "Overwrite", "Skip"))
                    return;

            File.WriteAllText(path, JsonConvert.SerializeObject(instance, Formatting.Indented));
            Debug.Log($"[GameSettingsConfigGenerator] Generated: {typeof(T).Name}.json");
        }
    }
}
#endif