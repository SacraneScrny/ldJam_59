using SackranyUI.Core.Base;
using SackranyUI.Core.Entities;

using UnityEngine;
using UnityEngine.UI;

namespace SackranyUI.Core.Views
{
    [AddComponentMenu("Sackrany/UI/General/Transform")]
    public class TransformView : View
    {
        public string PositionKey = "position";
        public string RotationKey = "rotation";
        public string ActiveKey = "active";

        [OutputBind("position")] [OutputBind("rotation")] public RectTransform Transform;
        [OutputBind("active")] GameObject _transformGo;

        protected override void OnBeforeBinding()
        {
            _transformGo = Transform.gameObject;
            Remap("position", PositionKey);
            Remap("rotation", RotationKey);
            Remap("active", ActiveKey);
        }

        #if UNITY_EDITOR
        void OnValidate()
        {
            if (Transform == null)
                Transform = gameObject.GetComponent<RectTransform>();
        }
        #endif
    }
}