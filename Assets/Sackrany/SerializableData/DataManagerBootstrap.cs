using Cysharp.Threading.Tasks;

using Sackrany.SerializableData.Converters;
using Sackrany.SerializableData.Storage;

using UnityEngine;

namespace Sackrany.SerializableData
{
    internal static class DataManagerBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            JsonSettings.Reset();
            JsonSettings.RegisterConverter(new Vector3Converter());
            JsonSettings.RegisterConverter(new Vector2Converter());
            JsonSettings.RegisterConverter(new Vector2IntConverter());
            JsonSettings.RegisterConverter(new QuaternionConverter());
            JsonSettings.ApplyDefaults();

            SaveDataStorage.Initialize(
                Application.persistentDataPath + "/Saves/",
                Application.platform == RuntimePlatform.WebGLPlayer);

            DataManager.Initialize();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Sackrany/Save Data/Save Now &s")]
        static void EditorSave() => DataManager.SaveAllData(true).Forget();

        [UnityEditor.MenuItem("Sackrany/Save Data/Reset Current Slot")]
        static void EditorReset() => DataManager.ResetSaveData();
#endif
    }
}
