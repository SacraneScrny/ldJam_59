using Sackrany.Actor.Modules;
using Sackrany.Actor.UnitMono;

namespace Sackrany.Actor.Base
{
    public abstract class UnitBase
    {
        public Unit Unit { get; private set; }
        protected ModulesController Controller;
        
        public bool HasUnit => Unit != null;
        public bool HasController => Controller != null;
        
        public void FillUnit(Unit unit) => Unit = unit;
        public void FillController(ModulesController controller) => Controller = controller;
    }
}