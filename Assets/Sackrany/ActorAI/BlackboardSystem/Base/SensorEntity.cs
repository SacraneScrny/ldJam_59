using System;

using Sackrany.Actor.UnitMono;
using Sackrany.ActorAI.BlackboardSystem.Static;

namespace Sackrany.ActorAI.BlackboardSystem.Base
{
    public interface ISensor : IDisposable
    {
        void OnStart();
        void OnTick(float deltaTime);
    }
    public interface ISensorTemplate
    {
        int GetId();
        ISensor GetInstance();
    }

    public interface ISensorTemplate<T> : ISensorTemplate where T : ISensor, new()
    {
        int ISensorTemplate.GetId() => SensorRegistry.GetId<T>();
        ISensor ISensorTemplate.GetInstance() => new T();
    }
    
    public abstract class SensorBase : ISensor
    {
        protected Unit Unit { get; private set; }
        protected IWriteBlackboard Blackboard { get; private set; }

        internal void Initialize(Unit unit) => Unit = unit;
        internal void FillBlackboard(IWriteBlackboard blackboard) => Blackboard = blackboard;

        public virtual void OnStart() { }
        public abstract void OnTick(float deltaTime);
        public virtual void Dispose() { }
    }
}