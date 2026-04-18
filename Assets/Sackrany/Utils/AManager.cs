using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Sackrany.Utils
{
    public abstract class AManager<T> : MonoBehaviour where T : AManager<T>
    {
        protected static T _instance;
        bool _initialized;
        
        public static bool IsExists => _instance != null;
        public static T Instance
        {
            get
            {
                if (ManagerState.IsQuitting) return null;
                if (_instance == null)
                {
                    _instance = Object.FindAnyObjectByType<T>(FindObjectsInactive.Exclude);
                    
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }

                    _instance.Initialize();
                }

                return _instance;
            }
        }
        
        #if UNITY_EDITOR
        void OnValidate()
        {
            gameObject.name = typeof(T).Name;
        }
        #endif
        
        void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            OnInitialize();
        }
        protected virtual void OnInitialize() { }
        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
            _instance.Initialize();
            OnManagerAwake();
        }
        protected virtual void OnManagerAwake() { }
        void OnDestroy()
        {
            OnManagerDestroy();
            if (_instance == this)
                _instance = null;
        }
        protected virtual void OnManagerDestroy() { }
        
        void OnApplicationQuit() => ManagerState.IsQuitting = true;

        public static async UniTaskVoid ExecuteSafe(Action<T> action, CancellationToken ct = default)
        {
            await UniTask.WaitUntil(() => IsExists, cancellationToken: ct);
            action(Instance);
        }
    }
    
    static class ManagerState
    {
        public static bool IsQuitting;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset() => IsQuitting = false;
    }
}