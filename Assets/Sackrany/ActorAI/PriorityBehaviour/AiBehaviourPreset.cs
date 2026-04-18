using Sackrany.ActorAI.PriorityBehaviour.Base;

using UnityEngine;

namespace Sackrany.ActorAI.PriorityBehaviour
{
    [CreateAssetMenu(fileName = "AiBehavioursPreset", menuName = "AI/Behaviours Preset")]
    public class AiBehavioursPreset : ScriptableObject
    {
        [SerializeReference][SubclassSelector]
        public IAiBehaviourTemplate[] Behaviours;
    }
}