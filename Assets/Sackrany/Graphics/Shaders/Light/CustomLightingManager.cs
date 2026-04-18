using UnityEngine;

namespace Sackrany.Graphics.Shaders.Light
{
    [ExecuteAlways]
    public class CustomLightingManager : MonoBehaviour
    {
        public int PositionQuantumStep = 4;
        
        const int MAX_LIGHTS = 64;
        static readonly int _LightCountID = Shader.PropertyToID("_CustomLightCount");
        static readonly int _LightDataID = Shader.PropertyToID("_CustomLightData");
        static readonly int _LightColorID = Shader.PropertyToID("_CustomLightColor");
        
        static readonly int _LightPositionQuantumStepID = Shader.PropertyToID("_CustomLightPositionQuantumStep");
        
        readonly Vector4[] _lightDataArray = new Vector4[MAX_LIGHTS];
        readonly Vector4[] _lightColorArray = new Vector4[MAX_LIGHTS];
        
        void OnValidate() => gameObject.name = $"{GetType().Name}";
        
        void Update()
        {
            var lights = CustomLightSource.AllLights;
            int count = Mathf.Min(lights.Count, MAX_LIGHTS);

            for (int i = 0; i < count; i++)
            {
                var l = lights[i];
                
                _lightDataArray[i] = new Vector4(
                    l.transform.position.x, 
                    l.transform.position.y, 
                    l.Level, 
                    l.Radius
                );

                _lightColorArray[i] = new Vector4(
                    l.Color.r, 
                    l.Color.g, 
                    l.Color.b, 
                    l.Intensity
                );
            }

            Shader.SetGlobalFloat(_LightCountID, count);
            Shader.SetGlobalVectorArray(_LightDataID, _lightDataArray);
            Shader.SetGlobalVectorArray(_LightColorID, _lightColorArray);
            Shader.SetGlobalInt(_LightPositionQuantumStepID, PositionQuantumStep);
        }
    }
}