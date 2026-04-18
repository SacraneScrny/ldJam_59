using System.Linq;

using Sackrany.Actor.Builder;
using Sackrany.Actor.Modules;
using Sackrany.Actor.UnitMono;
using Sackrany.ActorAI.BlackboardSystem.Base;
using Sackrany.Utils;

using UnityEngine;

namespace Sackrany.ActorAI.BlackboardSystem
{
    public class GlobalBlackboard : AManager<GlobalBlackboard>
    {
        [SerializeReference, SubclassSelector] public ISensorTemplate[] Sensors;
        
        Unit _blackboardUnit;
        BlackboardModule _blackboardModule;

        public static Blackboard Board => Instance._blackboardModule.Board;
        public static BlackboardModule Module => Instance._blackboardModule;
        
        protected override void OnInitialize()
        {
            _blackboardUnit = gameObject
                .AsUnit()
                .AddUnit()
                .WithModules(new[]
                {
                    new BlackboardHandler() as ModuleTemplate,
                    new SensorHandler() { Sensors = this.Sensors } as ModuleTemplate,
                })
                .At(Vector3.zero)
                .Facing(Quaternion.identity)
                .Activate()
                .Build();
            _blackboardModule = _blackboardUnit.Get<BlackboardModule>();
        }
    }
}