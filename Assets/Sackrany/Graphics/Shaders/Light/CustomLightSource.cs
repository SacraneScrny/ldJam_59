using System.Collections.Generic;

using UnityEngine;

namespace Sackrany.Graphics.Shaders.Light
{
    [ExecuteAlways]
    public class CustomLightSource : MonoBehaviour
    {
        public float Level;
        [Header("Base Settings")]
        public float OriginalRadius = 5f;
        public Color OriginalColor = Color.white;
        public float OriginalIntensity = 1f;

        [Header("Flicker Settings (Torch Effect)")]
        public bool EnableFlicker = true;
        [Range(0.1f, 20f)] public float FlickerSpeed = 5f;
        [Range(0f, 1f)] public float IntensityJitter = 0.2f;
        [Range(0f, 1f)] public float RadiusJitter = 0.1f;

        [HideInInspector] public float Radius;
        [HideInInspector] public Color Color;
        [HideInInspector] public float Intensity;

        public static readonly List<CustomLightSource> AllLights = new List<CustomLightSource>();

        private float _noiseOffset;

        void OnEnable() 
        {
            AllLights.Add(this);
            _noiseOffset = Random.Range(0f, 1000f); 
        }

        void OnDisable() => AllLights.Remove(this);

        void Update()
        {
            if (!EnableFlicker)
            {
                Radius = OriginalRadius;
                Intensity = OriginalIntensity;
                Color = OriginalColor;
                return;
            }

            //transform.position = transform.position.With(z: transform.position.y * 0.01f * 4 - 1);

            float time = Time.time * FlickerSpeed + _noiseOffset;
            float noise = Mathf.PerlinNoise(time, time * 0.5f);
            float jitterModifier = (noise * 2f - 1f);

            Intensity = OriginalIntensity + (jitterModifier * IntensityJitter);
            Radius = OriginalRadius + (jitterModifier * RadiusJitter);
            Color = OriginalColor;
        }
    }
}