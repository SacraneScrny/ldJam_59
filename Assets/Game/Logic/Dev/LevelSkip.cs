using System;

using Game.Logic.Level;

using Sackrany.GameInput;

using UnityEngine;

namespace Game.Logic.Dev
{
    public class LevelSkip : MonoBehaviour
    {
        void Update()
        {
            #if UNITY_EDITOR
            if (InputManager.PlayerCache.CrouchJustPressed)
                GameLevelManager.MarkWon();
            #endif
        }
    }
}