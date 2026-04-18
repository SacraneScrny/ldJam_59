using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.Static;
using Sackrany.ActorAI.BlackboardSystem.Base;
using Sackrany.ActorAI.BlackboardSystem.Static;

using UnityEngine;

namespace Sackrany.ActorAI.BlackboardSystem
{
    public class SensorHandlerModule : Module, IUpdateModule
    {
        [Template] SensorHandler _template;
        
        [Dependency] BlackboardModule _blackboardModule;
        ISensor[] _sensors;

        protected override void OnStart()
        {
            var templates = _template.GetBehaviours();
            if (templates == null || templates.Length == 0) return;

            _sensors = new ISensor[templates.Length];

            for (int i = 0; i < templates.Length; i++)
            {
                var behaviour = templates[i].GetInstance();
                if (behaviour is SensorBase b)
                {
                    b.Initialize(Unit);
                    b.FillBlackboard(_blackboardModule.Board);
                    b.OnStart();
                }
                InjectSensor(behaviour, templates[i]);

                _sensors[i] = behaviour;
            }
        }
        void InjectSensor(ISensor sensor, ISensorTemplate template)
        {
            var meta = SensorReflectionCache.GetMetadata(sensor.GetType());

            if (meta.Template.Field != null)
            {
                var templateType = template.GetType();
                if (meta.Template.FieldType == templateType)
                    meta.Template.Field.SetValue(sensor, template);
            }

            DependencyInjector.InjectDependencies(sensor, Unit.GetController());
        }

        public void OnUpdate(float deltaTime)
        {
            if (_sensors == null) return;

            for (int i = 0; i < _sensors.Length; i++)
            {
                _sensors[i].OnTick(deltaTime);
            }
        }
        
        protected override void OnReset()
        {
            for (int i = 0; i < _sensors.Length; i++)
                _sensors[i].Dispose();
            _sensors = null;
        }
        protected override void OnDispose()
        {
            for (int i = 0; i < _sensors.Length; i++)
                _sensors[i].Dispose();
            _sensors = null;
        }
    }
    [Serializable]
    public struct SensorHandler : ModuleTemplate<SensorHandlerModule>
    {
        [SerializeField][SerializeReference][SubclassSelector]
        public ISensorTemplate[] Sensors;

        [SerializeField]
        public SensorPreset Preset;

        public ISensorTemplate[] GetBehaviours()
            => Preset != null ? Preset.Sensors : Sensors;
    }
}