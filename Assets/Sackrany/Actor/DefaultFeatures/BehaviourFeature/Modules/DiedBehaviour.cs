using System;
using System.Collections;

using Sackrany.Actor.EventBus;
using Sackrany.Actor.Modules;
using Sackrany.Utils.Pool.Extensions;

namespace Sackrany.Actor.DefaultFeatures.BehaviourFeature.Modules
{
    public class DiedBehaviourModule : Module
    {
        [Template] DiedBehaviour _template;
        
        protected override void OnStart()
        {
            Unit.Event.Subscribe<Events.OnDied>(() =>
            {
                Unit.StartCoroutine(destroyEnum());
            });
        }

        IEnumerator destroyEnum()
        {
            yield return null;
            Unit.RELEASE();
        }
    }

    [Serializable]
    public struct DiedBehaviour : ModuleTemplate<DiedBehaviourModule>
    {
        
    }
}