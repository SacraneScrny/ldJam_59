using System;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Actor.Static;
using Sackrany.ActorAI.BlackboardSystem;
using Sackrany.ActorAI.PriorityBehaviour.Base;
using Sackrany.ActorAI.PriorityBehaviour.Static;

using UnityEngine;

namespace Sackrany.ActorAI.PriorityBehaviour
{
    public class AiBehaviourModule : Module, IUpdateModule
    {
        [Template] AiBehaviours _template;
        
        [Dependency(optional: true)] BlackboardModule _blackboardModule;
        IAiBehaviour[] _behaviours;
        IAiBehaviour   _active;
        bool _haveLocalBlackboard;

        protected override void OnStart()
        {
           _haveLocalBlackboard = _blackboardModule != null;
           if (!_haveLocalBlackboard) _blackboardModule = GlobalBlackboard.Module;
            
            var templates = _template.GetBehaviours();
            if (templates == null || templates.Length == 0) return;

            _behaviours = new IAiBehaviour[templates.Length];

            for (int i = 0; i < templates.Length; i++)
            {
                var behaviour = templates[i].GetInstance();
                if (behaviour is AiBehaviourBase b)
                {
                    b.Initialize(Unit);
                    b.FillBlackboard(_blackboardModule.Board);
                }
                InjectBehaviour(behaviour, templates[i]);

                _behaviours[i] = behaviour;
            }
        }
        void InjectBehaviour(IAiBehaviour behaviour, IAiBehaviourTemplate template)
        {
            var meta = BehaviourReflectionCache.GetMetadata(behaviour.GetType());

            if (meta.Template.Field != null)
            {
                var templateType = template.GetType();
                if (meta.Template.FieldType == templateType)
                    meta.Template.Field.SetValue(behaviour, template);
            }

            DependencyInjector.InjectDependencies(behaviour, Unit.GetController());
        }

        public void OnUpdate(float deltaTime)
        {
            if (_behaviours == null) return;

            IAiBehaviour candidate = null;
            for (int i = 0; i < _behaviours.Length; i++)
            {
                if (!_behaviours[i].CanExecute()) continue;
                candidate = _behaviours[i];
                break;
            }
            
            if (candidate != _active)
            {
                _active?.OnDeactivate();
                _active = candidate;
                _active?.OnActivate();
            }

            _active?.OnTick(deltaTime);
        }
        
        protected override void OnReset()
        {
            _active?.OnDeactivate();
            _active?.Dispose();
            _active = null;
        }
        protected override void OnDispose()
        {
            _active?.OnDeactivate();
            _active?.Dispose();
            _active     = null;
            _behaviours = null;
        }
    }
    [Serializable]
    public struct AiBehaviours : ModuleTemplate<AiBehaviourModule>
    {
        [SerializeField][SerializeReference][SubclassSelector]
        public IAiBehaviourTemplate[] Behaviours;

        [SerializeField]
        public AiBehavioursPreset Preset;

        public IAiBehaviourTemplate[] GetBehaviours()
            => Preset != null ? Preset.Behaviours : Behaviours;
    }
}