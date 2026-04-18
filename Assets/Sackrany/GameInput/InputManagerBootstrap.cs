using UnityEngine;

namespace Sackrany.GameInput
{
    internal static class InputManagerBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init() => InputManager.Init();
    }
}