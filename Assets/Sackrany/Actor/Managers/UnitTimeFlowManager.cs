using ModifiableVariable;

using Sackrany.Utils;

namespace Sackrany.Actor.Managers
{
    public class UnitTimeFlowManager : AManager<UnitTimeFlowManager>
    {
        public Modifiable<float> UnitsTimeFlow = new (1f);
        
        float _lastTimeFlow = 0f;
        public static float TimeFlow => Instance._lastTimeFlow;
        protected override void OnInitialize()
        {
            _lastTimeFlow = UnitsTimeFlow;
        }
        void Update()
        {
            _lastTimeFlow = UnitsTimeFlow;
        }
    }
}