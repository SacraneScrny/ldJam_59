using System;

using Sackrany.Actor.Modules;
using Sackrany.ActorAI.BlackboardSystem.Base;

namespace Sackrany.ActorAI.BlackboardSystem
{
    public class BlackboardModule : Module
    {
        public Blackboard Board { get; } = new();

        protected override void OnReset()   => Board.ClearAll();
        protected override void OnDispose() => Board.ClearAll();
    }

    [Serializable]
    public struct BlackboardHandler : ModuleTemplate<BlackboardModule> { }
}