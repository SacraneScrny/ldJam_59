using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Sackrany.Actor.DefaultFeatures.VolumeFeature.Entities
{
    public struct TonemappingVar : IVolumeVariable<Tonemapping>
    {
        public TonemappingMode tonemappingMode;

        public TonemappingVar(Tonemapping tonemapping)
        {
            tonemappingMode = tonemapping.mode.value;
        }

        public static TonemappingVar operator +(TonemappingVar a, TonemappingVar b)
        {
            a.tonemappingMode = b.tonemappingMode;
            return a;
        }

        public static TonemappingVar operator *(TonemappingVar a, TonemappingVar b)
        {
            return a;
        }

        public void Apply(Tonemapping component)
        {
        }
    }

    public struct VignetteVar : IVolumeVariable<Vignette>
    {
        public Color color;
        public Vector2 center;
        public float intensity;
        public float smoothness;
        public bool rounded;

        public VignetteVar(Vignette vignette)
        {
            color = vignette.color.value;
            center = vignette.center.value;
            intensity = vignette.intensity.value;
            smoothness = vignette.smoothness.value;
            rounded = vignette.rounded.value;
        }

        public static VignetteVar operator +(VignetteVar a, VignetteVar b)
        {
            a.color += b.color;
            a.center += b.center;
            a.intensity += b.intensity;
            a.smoothness += b.smoothness;
            a.rounded |= b.rounded;
            return a;
        }

        public static VignetteVar operator *(VignetteVar a, VignetteVar b)
        {
            a.color *= b.color;
            a.center *= b.center;
            a.intensity *= b.intensity;
            a.smoothness *= b.smoothness;
            return a;
        }

        public void Apply(Vignette component)
        {
            component.color.value = color;
            component.center.value = center;
            component.intensity.value = intensity;
            component.smoothness.value = smoothness;
            component.rounded.value = rounded;
        }
    }

    public struct BloomVar : IVolumeVariable<Bloom>
    {
        public float intensity;
        public float threshold;

        public BloomVar(Bloom bloom)
        {
            intensity = bloom.intensity.value;
            threshold = bloom.threshold.value;
        }

        public static BloomVar operator +(BloomVar a, BloomVar b)
        {
            a.intensity += b.intensity;
            a.threshold += b.threshold;
            return a;
        }

        public static BloomVar operator *(BloomVar a, BloomVar b)
        {
            a.intensity *= b.intensity;
            a.threshold *= b.threshold;
            return a;
        }

        public void Apply(Bloom c)
        {
            c.intensity.value = intensity;
            c.threshold.value = threshold;
        }
    }

    public struct MotionBlurVar : IVolumeVariable<MotionBlur>
    {
        public float intensity;

        public MotionBlurVar(MotionBlur mb)
        {
            intensity = mb.intensity.value;
        }

        public static MotionBlurVar operator +(MotionBlurVar a, MotionBlurVar b)
        {
            a.intensity += b.intensity;
            return a;
        }

        public static MotionBlurVar operator *(MotionBlurVar a, MotionBlurVar b)
        {
            a.intensity *= b.intensity;
            return a;
        }

        public void Apply(MotionBlur c) => c.intensity.value = intensity;
    }

    public struct ChannelMixerVar : IVolumeVariable<ChannelMixer>
    {
        public float red;
        public float green;
        public float blue;

        public ChannelMixerVar(ChannelMixer c)
        {
            red = c.redOutRedIn.value;
            green = c.greenOutGreenIn.value;
            blue = c.blueOutBlueIn.value;
        }

        public static ChannelMixerVar operator +(ChannelMixerVar a, ChannelMixerVar b)
        {
            a.red += b.red;
            a.green += b.green;
            a.blue += b.blue;
            return a;
        }

        public static ChannelMixerVar operator *(ChannelMixerVar a, ChannelMixerVar b)
        {
            a.red *= b.red;
            a.green *= b.green;
            a.blue *= b.blue;
            return a;
        }

        public void Apply(ChannelMixer c)
        {
            c.redOutRedIn.value = red;
            c.greenOutGreenIn.value = green;
            c.blueOutBlueIn.value = blue;
        }
    }

    public struct ChromaticAberrationVar : IVolumeVariable<ChromaticAberration>
    {
        public float intensity;

        public ChromaticAberrationVar(ChromaticAberration c)
        {
            intensity = c.intensity.value;
        }

        public static ChromaticAberrationVar operator +(ChromaticAberrationVar a, ChromaticAberrationVar b)
        {
            a.intensity += b.intensity;
            return a;
        }

        public static ChromaticAberrationVar operator *(ChromaticAberrationVar a, ChromaticAberrationVar b)
        {
            a.intensity *= b.intensity;
            return a;
        }

        public void Apply(ChromaticAberration c) => c.intensity.value = intensity;
    }

    public struct ColorAdjustmentsVar : IVolumeVariable<ColorAdjustments>
    {
        public float postExposure;
        public float contrast;
        public float saturation;
        public Color colorFilter;

        public ColorAdjustmentsVar(ColorAdjustments c)
        {
            postExposure = c.postExposure.value;
            contrast = c.contrast.value;
            saturation = c.saturation.value;
            colorFilter = c.colorFilter.value;
        }

        public static ColorAdjustmentsVar operator +(ColorAdjustmentsVar a, ColorAdjustmentsVar b)
        {
            a.postExposure += b.postExposure;
            a.contrast += b.contrast;
            a.colorFilter += b.colorFilter;
            a.saturation += b.saturation;
            return a;
        }

        public static ColorAdjustmentsVar operator *(ColorAdjustmentsVar a, ColorAdjustmentsVar b)
        {
            a.postExposure *= b.postExposure;
            a.contrast *= b.contrast;
            a.colorFilter *= b.colorFilter;
            a.saturation *= b.saturation;
            return a;
        }

        public void Apply(ColorAdjustments c)
        {
            c.postExposure.value = postExposure;
            c.contrast.value = contrast;
            c.colorFilter.value = colorFilter;
            c.saturation.value = saturation;
        }
    }

    public struct DepthOfFieldVar : IVolumeVariable<DepthOfField>
    {
        public float focusDistance;
        public float aperture;

        public DepthOfFieldVar(DepthOfField d)
        {
            focusDistance = d.focusDistance.value;
            aperture = d.aperture.value;
        }

        public static DepthOfFieldVar operator +(DepthOfFieldVar a, DepthOfFieldVar b)
        {
            a.focusDistance += b.focusDistance;
            a.aperture += b.aperture;
            return a;
        }

        public static DepthOfFieldVar operator *(DepthOfFieldVar a, DepthOfFieldVar b)
        {
            a.focusDistance *= b.focusDistance;
            a.aperture *= b.aperture;
            return a;
        }

        public void Apply(DepthOfField c)
        {
            c.focusDistance.value = focusDistance;
            c.aperture.value = aperture;
        }
    }

    public struct WhiteBalanceVar : IVolumeVariable<WhiteBalance>
    {
        public float temperature;
        public float tint;

        public WhiteBalanceVar(WhiteBalance w)
        {
            temperature = w.temperature.value;
            tint = w.tint.value;
        }

        public static WhiteBalanceVar operator +(WhiteBalanceVar a, WhiteBalanceVar b)
        {
            a.temperature += b.temperature;
            a.tint += b.tint;
            return a;
        }

        public static WhiteBalanceVar operator *(WhiteBalanceVar a, WhiteBalanceVar b)
        {
            a.temperature *= b.temperature;
            a.tint *= b.tint;
            return a;
        }

        public void Apply(WhiteBalance c)
        {
            c.temperature.value = temperature;
            c.tint.value = tint;
        }
    }

    public struct ColorCurvesVar : IVolumeVariable<ColorCurves>
    {
        public TextureCurve master;
        public TextureCurve red;
        public TextureCurve green;
        public TextureCurve blue;

        public ColorCurvesVar(ColorCurves c)
        {
            master = c.master.value;
            red = c.red.value;
            green = c.green.value;
            blue = c.blue.value;
        }

        public static ColorCurvesVar operator +(ColorCurvesVar a, ColorCurvesVar b) => a;
        public static ColorCurvesVar operator *(ColorCurvesVar a, ColorCurvesVar b) => a;

        public void Apply(ColorCurves c)
        {
            c.master.value = master;
            c.red.value = red;
            c.green.value = green;
            c.blue.value = blue;
        }
    }

    public struct ColorLookupVar : IVolumeVariable<ColorLookup>
    {
        public Texture texture;
        public float contribution;

        public ColorLookupVar(ColorLookup c)
        {
            texture = c.texture.value;
            contribution = c.contribution.value;
        }

        public static ColorLookupVar operator +(ColorLookupVar a, ColorLookupVar b)
        {
            a.contribution += b.contribution;
            return a;
        }

        public static ColorLookupVar operator *(ColorLookupVar a, ColorLookupVar b)
        {
            a.contribution *= b.contribution;
            return a;
        }

        public void Apply(ColorLookup c)
        {
            c.texture.value = texture;
            c.contribution.value = contribution;
        }
    }

    public struct FilmGrainVar : IVolumeVariable<FilmGrain>
    {
        public float intensity;
        public float response;

        public FilmGrainVar(FilmGrain f)
        {
            intensity = f.intensity.value;
            response = f.response.value;
        }

        public static FilmGrainVar operator +(FilmGrainVar a, FilmGrainVar b)
        {
            a.intensity += b.intensity;
            a.response += b.response;
            return a;
        }

        public static FilmGrainVar operator *(FilmGrainVar a, FilmGrainVar b)
        {
            a.intensity *= b.intensity;
            a.response *= b.response;
            return a;
        }

        public void Apply(FilmGrain c)
        {
            c.intensity.value = intensity;
            c.response.value = response;
        }
    }

    public struct LensDistortionVar : IVolumeVariable<LensDistortion>
    {
        public float intensity;
        public float scale;

        public LensDistortionVar(LensDistortion l)
        {
            intensity = l.intensity.value;
            scale = l.scale.value;
        }

        public static LensDistortionVar operator +(LensDistortionVar a, LensDistortionVar b)
        {
            a.intensity += b.intensity;
            a.scale += b.scale;
            return a;
        }

        public static LensDistortionVar operator *(LensDistortionVar a, LensDistortionVar b)
        {
            a.intensity *= b.intensity;
            a.scale *= b.scale;
            return a;
        }

        public void Apply(LensDistortion c)
        {
            c.intensity.value = intensity;
            c.scale.value = scale;
        }
    }

    public struct LiftGammaGainVar : IVolumeVariable<LiftGammaGain>
    {
        public Vector4 lift;
        public Vector4 gamma;
        public Vector4 gain;

        public LiftGammaGainVar(LiftGammaGain l)
        {
            lift = l.lift.value;
            gamma = l.gamma.value;
            gain = l.gain.value;
        }

        public static LiftGammaGainVar operator +(LiftGammaGainVar a, LiftGammaGainVar b)
        {
            a.lift += b.lift;
            a.gamma += b.gamma;
            a.gain += b.gain;
            return a;
        }

        public static LiftGammaGainVar operator *(LiftGammaGainVar a, LiftGammaGainVar b)
        {
            a.lift *= b.lift.x;
            a.gamma *= b.gamma.x;
            a.gain *= b.gain.x;
            return a;
        }

        public void Apply(LiftGammaGain c)
        {
            c.lift.value = lift;
            c.gamma.value = gamma;
            c.gain.value = gain;
        }
    }

    public struct PaniniProjectionVar : IVolumeVariable<PaniniProjection>
    {
        public float distance;
        public float cropToFit;

        public PaniniProjectionVar(PaniniProjection p)
        {
            distance = p.distance.value;
            cropToFit = p.cropToFit.value;
        }

        public static PaniniProjectionVar operator +(PaniniProjectionVar a, PaniniProjectionVar b)
        {
            a.distance += b.distance;
            a.cropToFit += b.cropToFit;
            return a;
        }

        public static PaniniProjectionVar operator *(PaniniProjectionVar a, PaniniProjectionVar b)
        {
            a.distance *= b.distance;
            a.cropToFit *= b.cropToFit;
            return a;
        }

        public void Apply(PaniniProjection c)
        {
            c.distance.value = distance;
            c.cropToFit.value = cropToFit;
        }
    }

    public struct ScreenSpaceLensFlareVar : IVolumeVariable<ScreenSpaceLensFlare>
    {
        public float intensity;

        public ScreenSpaceLensFlareVar(ScreenSpaceLensFlare s)
        {
            intensity = s.intensity.value;
        }

        public static ScreenSpaceLensFlareVar operator +(ScreenSpaceLensFlareVar a, ScreenSpaceLensFlareVar b)
        {
            a.intensity += b.intensity;
            return a;
        }

        public static ScreenSpaceLensFlareVar operator *(ScreenSpaceLensFlareVar a, ScreenSpaceLensFlareVar b)
        {
            a.intensity *= b.intensity;
            return a;
        }

        public void Apply(ScreenSpaceLensFlare c)
        {
            c.intensity.value = intensity;
        }
    }

    public struct ShadowsMidtonesHighlightsVar : IVolumeVariable<ShadowsMidtonesHighlights>
    {
        public Vector4 shadows;
        public Vector4 midtones;
        public Vector4 highlights;

        public ShadowsMidtonesHighlightsVar(ShadowsMidtonesHighlights s)
        {
            shadows = s.shadows.value;
            midtones = s.midtones.value;
            highlights = s.highlights.value;
        }

        public static ShadowsMidtonesHighlightsVar operator +(ShadowsMidtonesHighlightsVar a, ShadowsMidtonesHighlightsVar b)
        {
            a.shadows += b.shadows;
            a.midtones += b.midtones;
            a.highlights += b.highlights;
            return a;
        }

        public static ShadowsMidtonesHighlightsVar operator *(ShadowsMidtonesHighlightsVar a, ShadowsMidtonesHighlightsVar b)
        {
            a.shadows *= b.shadows.x;
            a.midtones *= b.midtones.x;
            a.highlights *= b.highlights.x;
            return a;
        }

        public void Apply(ShadowsMidtonesHighlights c)
        {
            c.shadows.value = shadows;
            c.midtones.value = midtones;
            c.highlights.value = highlights;
        }
    }

    public struct SplitToningVar : IVolumeVariable<SplitToning>
    {
        public Color shadows;
        public Color highlights;
        public float balance;

        public SplitToningVar(SplitToning s)
        {
            shadows = s.shadows.value;
            highlights = s.highlights.value;
            balance = s.balance.value;
        }

        public static SplitToningVar operator +(SplitToningVar a, SplitToningVar b)
        {
            a.shadows += b.shadows;
            a.highlights += b.highlights;
            a.balance += b.balance;
            return a;
        }

        public static SplitToningVar operator *(SplitToningVar a, SplitToningVar b)
        {
            a.shadows *= b.shadows;
            a.highlights *= b.highlights;
            a.balance *= b.balance;
            return a;
        }

        public void Apply(SplitToning c)
        {
            c.shadows.value = shadows;
            c.highlights.value = highlights;
            c.balance.value = balance;
        }
    }

    public interface IVolumeVariable<in T>
        where T : VolumeComponent
    {
        public void Apply(T component);
    }
}