using SackranyUI.Core.Base;
using SackranyUI.Core.Entities;

using UnityEngine;
using UnityEngine.UI;

namespace SackranyUI.Core.Views
{
    [AddComponentMenu("Sackrany/UI/General/Graphic")]
    public class GraphicView : View
    {
        public string ColorKey = "color";
        public string AlphaKey = "alpha";
        public string GraphicActiveKey = "graphic_active";

        [OutputBind("color")] [OutputBind("alpha")] public Graphic Graphic;
        [OutputBind("graphic_active")] GameObject _graphicGo;

        protected override void OnBeforeBinding()
        {
            _graphicGo = Graphic.gameObject;
            Remap("color", ColorKey);
            Remap("alpha", AlphaKey);
            Remap("graphic_active", GraphicActiveKey);
        }

        #if UNITY_EDITOR
        void OnValidate()
        {
            if (Graphic == null)
                Graphic = GetComponentInChildren<Graphic>();
        }
        #endif
    }
}