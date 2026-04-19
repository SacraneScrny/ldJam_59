using System;

using Game.Logic.UI.World;

using R3;

using Sackrany.Actor.Modules;
using Sackrany.Actor.Modules.ModuleComposition;
using Sackrany.Utils;

using UnityEngine;

namespace Game.Logic.Missle
{
    public class MissleUIModule : Module, ILateUpdateModule
    {
        [Template] MissleUI _template;
        [Dependency] MissleCollideModule _collideModule;
        [Dependency] MissleFlyModule _flyModule;
        
        Vector3 _startPosition;
        
        protected override void OnStart()
        {
            _template.Bar.gameObject.SetActive(true);
            _flyModule.OnFinished += () => _template.Bar.gameObject.SetActive(false);
            
            DeferredExecution.AfterFrames(1, () => _startPosition = Unit.transform.position);
            
            _template.Bar.Clear();
            _template.Bar.Inverse();
            _template.Bar.Spawn(3);
            _collideModule.CurrentDamage
                .Skip(1)
                .Subscribe((_) => _template.Bar.Hit());
            
            _template.Bar.ConnectLine(Unit.transform);
        }
        public void OnLateUpdate(float deltaTime)
        {
            _template.Bar.transform.position = Vector3.Lerp(_startPosition, _template.Follow.transform.position, _template.FollowLerp);
        }
    }

    [Serializable]
    public struct MissleUI : ModuleTemplate<MissleUIModule>
    {
        public SegmentedBar Bar;
        public Transform Follow;
        public float FollowLerp;
    }
}