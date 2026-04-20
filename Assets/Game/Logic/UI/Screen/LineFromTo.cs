using System;

using UnityEngine;

namespace Game.Logic.UI.Screen
{
    public class LineFromTo : MonoBehaviour
    {
        public RectTransform From;
        public RectTransform To;
        void Update()
        {
            From.sizeDelta = new Vector2((To.localPosition - From.localPosition).magnitude, From.sizeDelta.y);
        }
    }
}