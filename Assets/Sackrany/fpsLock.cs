using System;

using UnityEngine;

namespace Sackrany
{
    public class fpsLock : MonoBehaviour
    {
        private void Start()
        {
            Application.targetFrameRate = 60;
        }
    }
}