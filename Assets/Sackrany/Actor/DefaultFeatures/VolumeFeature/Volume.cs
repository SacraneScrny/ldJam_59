using System;

using ModifiableVariable;

using Sackrany.Actor.DefaultFeatures.VolumeFeature.Entities;
using Sackrany.Actor.Modules;

using UnityEngine.Rendering.Universal;

namespace Sackrany.Actor.DefaultFeatures.VolumeFeature
{
    public class VolumeModule : Module
    {
        [Dependency] UnityEngine.Rendering.Volume Volume;

        #region VARS
        private Vignette _vignette;
        public Modifiable<VignetteVar> VignetteVariable;

        private Tonemapping _tonemapping;
        public Modifiable<TonemappingVar> TonemappingVariable;

        private Bloom _bloom;
        public Modifiable<BloomVar> BloomVariable;

        private MotionBlur _motionBlur;
        public Modifiable<MotionBlurVar> MotionBlurVariable;

        private ChannelMixer _channelMixer;
        public Modifiable<ChannelMixerVar> ChannelMixerVariable;

        private ChromaticAberration _chromaticAberration;
        public Modifiable<ChromaticAberrationVar> ChromaticAberrationVariable;

        private ColorAdjustments _colorAdjustments;
        public Modifiable<ColorAdjustmentsVar> ColorAdjustmentsVariable;

        private ColorCurves _colorCurves;
        public Modifiable<ColorCurvesVar> ColorCurvesVariable;

        private ColorLookup _colorLookup;
        public Modifiable<ColorLookupVar> ColorLookupVariable;

        private DepthOfField _depthOfField;
        public Modifiable<DepthOfFieldVar> DepthOfFieldVariable;

        private FilmGrain _filmGrain;
        public Modifiable<FilmGrainVar> FilmGrainVariable;

        private LensDistortion _lensDistortion;
        public Modifiable<LensDistortionVar> LensDistortionVariable;

        private LiftGammaGain _liftGammaGain;
        public Modifiable<LiftGammaGainVar> LiftGammaGainVariable;

        private PaniniProjection _paniniProjection;
        public Modifiable<PaniniProjectionVar> PaniniProjectionVariable;

        private ScreenSpaceLensFlare _screenSpaceLensFlare;
        public Modifiable<ScreenSpaceLensFlareVar> ScreenSpaceLensFlareVariable;

        private ShadowsMidtonesHighlights _shadowsMidtonesHighlights;
        public Modifiable<ShadowsMidtonesHighlightsVar> ShadowsMidtonesHighlightsVariable;

        private SplitToning _splitToning;
        public Modifiable<SplitToningVar> SplitToningVariable;

        private WhiteBalance _whiteBalance;
        public Modifiable<WhiteBalanceVar> WhiteBalanceVariable;
        #endregion

        protected override void OnStart()
        {
            if (Volume == null)
                Volume = Unit.GetComponent<UnityEngine.Rendering.Volume>();
            GatherProfiles();
        }

        void GatherProfiles()
        {
            var profile = Volume.profile;

            if (profile.TryGet(out _tonemapping))
                TonemappingVariable = new TonemappingVar(_tonemapping);

            if (profile.TryGet(out _vignette))
                VignetteVariable = new VignetteVar(_vignette);

            if (profile.TryGet(out _bloom))
                BloomVariable = new BloomVar(_bloom);

            if (profile.TryGet(out _motionBlur))
                MotionBlurVariable = new MotionBlurVar(_motionBlur);

            if (profile.TryGet(out _channelMixer))
                ChannelMixerVariable = new ChannelMixerVar(_channelMixer);

            if (profile.TryGet(out _chromaticAberration))
                ChromaticAberrationVariable = new ChromaticAberrationVar(_chromaticAberration);

            if (profile.TryGet(out _colorAdjustments))
                ColorAdjustmentsVariable = new ColorAdjustmentsVar(_colorAdjustments);

            if (profile.TryGet(out _colorCurves))
                ColorCurvesVariable = new ColorCurvesVar(_colorCurves);

            if (profile.TryGet(out _colorLookup))
                ColorLookupVariable = new ColorLookupVar(_colorLookup);

            if (profile.TryGet(out _depthOfField))
                DepthOfFieldVariable = new DepthOfFieldVar(_depthOfField);

            if (profile.TryGet(out _filmGrain))
                FilmGrainVariable = new FilmGrainVar(_filmGrain);

            if (profile.TryGet(out _lensDistortion))
                LensDistortionVariable = new LensDistortionVar(_lensDistortion);

            if (profile.TryGet(out _liftGammaGain))
                LiftGammaGainVariable = new LiftGammaGainVar(_liftGammaGain);

            if (profile.TryGet(out _paniniProjection))
                PaniniProjectionVariable = new PaniniProjectionVar(_paniniProjection);

            if (profile.TryGet(out _screenSpaceLensFlare))
                ScreenSpaceLensFlareVariable = new ScreenSpaceLensFlareVar(_screenSpaceLensFlare);

            if (profile.TryGet(out _shadowsMidtonesHighlights))
                ShadowsMidtonesHighlightsVariable = new ShadowsMidtonesHighlightsVar(_shadowsMidtonesHighlights);

            if (profile.TryGet(out _splitToning))
                SplitToningVariable = new SplitToningVar(_splitToning);

            if (profile.TryGet(out _whiteBalance))
                WhiteBalanceVariable = new WhiteBalanceVar(_whiteBalance);
        }
        
        public void SyncVolume()
        {    
            VignetteVariable?.GetValue().Apply(_vignette);
            TonemappingVariable?.GetValue().Apply(_tonemapping);
            BloomVariable?.GetValue().Apply(_bloom);
            MotionBlurVariable?.GetValue().Apply(_motionBlur);
            ChannelMixerVariable?.GetValue().Apply(_channelMixer);
            ChromaticAberrationVariable?.GetValue().Apply(_chromaticAberration);
            ColorAdjustmentsVariable?.GetValue().Apply(_colorAdjustments);
            ColorCurvesVariable?.GetValue().Apply(_colorCurves);
            ColorLookupVariable?.GetValue().Apply(_colorLookup);
            DepthOfFieldVariable?.GetValue().Apply(_depthOfField);
            FilmGrainVariable?.GetValue().Apply(_filmGrain);
            LensDistortionVariable?.GetValue().Apply(_lensDistortion);
            LiftGammaGainVariable?.GetValue().Apply(_liftGammaGain);
            PaniniProjectionVariable?.GetValue().Apply(_paniniProjection);
            ScreenSpaceLensFlareVariable?.GetValue().Apply(_screenSpaceLensFlare);
            ShadowsMidtonesHighlightsVariable?.GetValue().Apply(_shadowsMidtonesHighlights);
            SplitToningVariable?.GetValue().Apply(_splitToning);
            WhiteBalanceVariable?.GetValue().Apply(_whiteBalance);
        }
    }

    [Serializable]
    public struct Volume : ModuleTemplate<VolumeModule>
    {
        
    }
}