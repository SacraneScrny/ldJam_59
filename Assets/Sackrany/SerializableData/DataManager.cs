using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;

using Sackrany.SerializableData.Components;
using Sackrany.SerializableData.Serialization;
using Sackrany.SerializableData.Storage;

using UnityEngine.SceneManagement;

namespace Sackrany.SerializableData
{
    public static class DataManager
    {
        static SerializationContainer _container;
        static DateTime _sessionStart;
        static TimeSpan _accumulatedTime;
        static bool _sessionActive;

        const string ContainerKey = nameof(SerializationContainer);

        public static SlotManager Slots { get; private set; }

        public static event Action<List<object>> OnCollectSaveData;
        public static event Action OnBeforeCapture;
        public static event Action OnAfterCapture;

        internal static void Initialize()
        {
            _container = null;
            _accumulatedTime = TimeSpan.Zero;
            _sessionActive = false;
            OnCollectSaveData = null;
            OnBeforeCapture = null;
            OnAfterCapture = null;
            Slots = new SlotManager(OnSlotSwitched);
            EnsureContainerLoaded();
        }

        public static void StartSession()
        {
            if (_sessionActive) return;
            _sessionStart = DateTime.Now;
            _sessionActive = true;
        }

        public static void PauseSession()
        {
            if (!_sessionActive) return;
            _accumulatedTime += DateTime.Now - _sessionStart;
            _sessionActive = false;
        }

        public static void RegisterConverter(JsonConverter converter)
            => JsonSettings.RegisterConverter(converter);

        public static T Get<T>() where T : new()
            => SaveDataStorage.Get<T>(typeof(T).Name);

        public static T Get<T>(string key) where T : new()
            => SaveDataStorage.Get<T>(key);

        public static void Set<T>(T value)
            => SaveDataStorage.Set(typeof(T).Name, value);

        public static void Set(string key, object value)
            => SaveDataStorage.Set(key, value);

        public static async UniTask SaveAllData(bool withThumbnail = false)
        {
            if (withThumbnail)
            {
                OnBeforeCapture?.Invoke();
                var bytes = await ThumbnailCapture.Capture();
                OnAfterCapture?.Invoke();
                Slots.SetThumbnail(bytes);
            }

            _container.SerializeAll();
            SaveDataStorage.Set(ContainerKey, _container);

            var customData = new List<object>();
            OnCollectSaveData?.Invoke(customData);
            foreach (var data in customData)
                SaveDataStorage.Set(data.GetType().Name, data);

            Slots.StampCurrentSlot(GetPlayTime());
            SaveDataStorage.Flush();
        }

        public static void SerializeScene(Scene scene)
        {
            if (!scene.isLoaded) return;
            foreach (var root in scene.GetRootGameObjects())
                foreach (var behaviour in root.GetComponentsInChildren<SerializableBehaviour>())
                    SerializeObject(behaviour);
        }

        public static void SerializeObject(SerializableBehaviour behaviour)
        {
            behaviour.Serialize();
            _container.UpdateOne(behaviour);
        }

        public static void ResetSaveData() => SaveDataStorage.Reset();

        internal static void RegisterBehaviour(SerializableBehaviour behaviour)
        {
            EnsureContainerLoaded();
            _container.TemporaryContainer.TryAdd(behaviour.Guid, behaviour);
        }
        internal static void DeserializeOne(SerializableBehaviour behaviour)
        {
            EnsureContainerLoaded();
            _container.DeserializeOne(behaviour);
        }
        
        static TimeSpan GetPlayTime()
        {
            var current = _sessionActive ? DateTime.Now - _sessionStart : TimeSpan.Zero;
            return _accumulatedTime + current;
        }

        static void OnSlotSwitched()
        {
            _accumulatedTime = TimeSpan.Zero;
            _sessionActive = false;
            _container = null;
        }

        static void EnsureContainerLoaded()
        {
            if (_container != null) return;
            _container = SaveDataStorage.Get<SerializationContainer>(ContainerKey);
        }
    }
}
