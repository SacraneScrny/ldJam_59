using System;
using System.Collections.Generic;

using Sackrany.GameAudio.Components;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Sackrany.GameAudio
{
    public class AudioPool : IDisposable
    {
        int _createdCount;
        readonly Stack<RuntimeAudioComponent> _pool;
        Transform _parent;
        
        public AudioPool(int prewarm = 0)
        {
            _pool = new Stack<RuntimeAudioComponent>();

            for (int i = 0; i < prewarm; i++)
                Create();
        }

        public RuntimeAudioComponent Pool(Transform parent = null)
        {
            if (_pool.Count == 0) Create();
            RuntimeAudioComponent obj = _pool.Pop();
            obj.transform.SetParent(parent);
            obj.OnPooled();
            return obj;
        }
        public void Release(RuntimeAudioComponent obj)
        {
            obj.OnReleased();
            ParentValidation();
            obj.transform.SetParent(_parent);
            _pool.Push(obj);
        }
        void Create()
        {
            var go = new GameObject($"RuntimeAudio {_createdCount}", 
                typeof(AudioSource),
                typeof(RuntimeAudioComponent))
                .GetComponent<RuntimeAudioComponent>();
            go.gameObject.SetActive(false);
            go.transform.SetParent(_parent);
            _pool.Push(go);
            _createdCount++;
        }

        void ParentValidation()
        {
            if (_parent != null) return;
            _parent = new GameObject($"{this.GetType().Name}").transform;
            _parent.transform.position = Vector3.zero;
            _parent.transform.rotation = Quaternion.identity;
            _parent.transform.localScale = Vector3.one;
        }
        
        public void Dispose()
        {
            if (_parent != null && _parent.gameObject) Object.Destroy(_parent.gameObject);
            foreach (var obj in _pool)
                if (obj != null && obj.gameObject) Object.Destroy(obj.gameObject);
            _pool.Clear();
        }
    }
}