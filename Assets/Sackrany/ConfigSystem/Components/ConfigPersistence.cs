using UnityEngine;

namespace Sackrany.ConfigSystem.Components
{
    public class ConfigPersistence : MonoBehaviour
    {
        void OnApplicationQuit() => DynamicConfigLoader.SaveAll();

        void OnApplicationPause(bool pause)
        {
            if (pause) DynamicConfigLoader.SaveAll();
        }
    }
}