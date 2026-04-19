using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sackrany.Actor.DefaultFeatures.VolumeFeature;
using Sackrany.Actor.DefaultFeatures.VolumeFeature.Entities;
using Sackrany.Actor.Modules;

using UnityEngine;

namespace Game.Logic.Volume
{
    public class VolumeBridgeModule : Module
    {
        [Dependency] VolumeModule _volumeModule;

        private CancellationTokenSource _cts;

        protected override void OnStart()
        {
            _cts = new CancellationTokenSource();
        }
        protected override void OnReset()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Bloom(float intensity = 0, float threshold = 0, float duration = 0.3f)
        {
            if (_volumeModule.BloomVariable == null) return;
            BloomAsync(intensity, threshold, duration).Forget();
        }

        public void Vignette(float intensity = 0, float duration = 0.3f)
        {
            if (_volumeModule.VignetteVariable == null) return;
            VignetteAsync(intensity, duration).Forget();
        }

        public void ChromaticAberration(float intensity = 0, float duration = 0.3f)
        {
            if (_volumeModule.ChromaticAberrationVariable == null) return;
            ChromaticAberrationAsync(intensity, duration).Forget();
        }

        public void LensDistortion(float intensity = 0, float scale = 0, float duration = 0.3f)
        {
            if (_volumeModule.LensDistortionVariable == null) return;
            LensDistortionAsync(intensity, scale, duration).Forget();
        }

        public void ColorAdjustments(float postExposure = 0, float contrast = 0, float duration = 0.3f)
        {
            if (_volumeModule.ColorAdjustmentsVariable == null) return;
            ColorAdjustmentsAsync(postExposure, contrast, duration).Forget();
        }

        private async UniTaskVoid BloomAsync(float intensity, float threshold, float duration)
        {
            BloomVar current = default;
            using var _ = _volumeModule.BloomVariable.Add(() => current);
            await DecayAsync(duration, t =>
            {
                current = new BloomVar { intensity = intensity * t, threshold = threshold * t };
            });
        }

        private async UniTaskVoid VignetteAsync(float intensity, float duration)
        {
            VignetteVar current = default;
            using var _ = _volumeModule.VignetteVariable.Add(() => current);
            await DecayAsync(duration, t =>
            {
                current = new VignetteVar { intensity = intensity * t };
            });
        }

        private async UniTaskVoid ChromaticAberrationAsync(float intensity, float duration)
        {
            ChromaticAberrationVar current = default;
            using var _ = _volumeModule.ChromaticAberrationVariable.Add(() => current);
            await DecayAsync(duration, t =>
            {
                current = new ChromaticAberrationVar { intensity = intensity * t };
            });
        }

        private async UniTaskVoid LensDistortionAsync(float intensity, float scale, float duration)
        {
            LensDistortionVar current = default;
            using var _ = _volumeModule.LensDistortionVariable.Add(() => current);
            await DecayAsync(duration, t =>
            {
                current = new LensDistortionVar { intensity = intensity * t, scale = scale * t };
            });
        }

        private async UniTaskVoid ColorAdjustmentsAsync(float postExposure, float contrast, float duration)
        {
            ColorAdjustmentsVar current = default;
            using var _ = _volumeModule.ColorAdjustmentsVariable.Add(() => current);
            await DecayAsync(duration, t =>
            {
                current = new ColorAdjustmentsVar { postExposure = postExposure * t, contrast = contrast * t };
            });
        }

        private async UniTask DecayAsync(float duration, Action<float> onTick)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                onTick(1f - elapsed / duration);
                await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);
                elapsed += Time.deltaTime;
            }
            onTick(0f);
        }
    }

    [Serializable]
    public struct VolumeBridge : ModuleTemplate<VolumeBridgeModule>
    {
    }
}