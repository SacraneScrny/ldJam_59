using System;

using Sackrany.Actor.UnitMono;
using Sackrany.ActorAI.BlackboardSystem;
using Sackrany.ActorAI.BlackboardSystem.Base;
using Sackrany.ActorAI.PriorityBehaviour.Static;

namespace Sackrany.ActorAI.PriorityBehaviour.Base
{
    public interface IAiBehaviour : IDisposable
    {
        bool CanExecute();
        void OnActivate();
        void OnTick(float deltaTime);
        void OnDeactivate();
    }
    public interface IAiBehaviourTemplate
    {
        int          GetId();
        IAiBehaviour GetInstance();
    }

    public interface IAiBehaviourTemplate<T> : IAiBehaviourTemplate where T : IAiBehaviour, new()
    {
        int          IAiBehaviourTemplate.GetId()          => BehaviourRegistry.GetId<T>();
        IAiBehaviour IAiBehaviourTemplate.GetInstance()    => new T();
    }
    
    public abstract class AiBehaviourBase : IAiBehaviour
    {
        protected Unit Unit { get; private set; }
        protected IReadBlackboard Blackboard { get; private set; }

        internal void Initialize(Unit unit) => Unit = unit;
        internal void FillBlackboard(IReadBlackboard blackboard) => Blackboard = blackboard;

        public virtual  bool CanExecute()              => true;
        public virtual  void OnActivate()              { }
        public abstract void OnTick(float deltaTime);
        public virtual  void OnDeactivate()            { }
        public virtual void Dispose() { }
    }
}