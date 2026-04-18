using Sackrany.ActorAI.BlackboardSystem.Base;

using UnityEngine;

namespace Sackrany.ActorAI.BlackboardSystem
{
    [CreateAssetMenu(fileName = "SensorPreset", menuName = "AI/Sensor Preset")]
    public class SensorPreset : ScriptableObject
    {
        [SerializeReference][SubclassSelector]
        public ISensorTemplate[] Sensors;
    }
}