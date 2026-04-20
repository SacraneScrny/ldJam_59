using System;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using ModifiableVariable.Stages.StageFactory;

using Sackrany.Actor.Managers;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Game.Logic.Level
{
    public static class GameLevelManager
    {
        static readonly List<GameObject> _destroyOnReset = new();
        static CancellationTokenSource _cts;
        static bool _isLose;
        static bool _alreadyMarked;

        public static bool IsWaitingForRestart => _alreadyMarked;
        public static float TimeFlow { get; private set; } = 1;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            _destroyOnReset.Clear();
            OnLevelStarted = null;
            OnLevelFinished = null;
            _cts = new CancellationTokenSource();

            _isLose = false;
            _alreadyMarked = false;
            
            TimeFlow = 1;
            
            UnitTimeFlowManager.ExecuteSafe((m) => m.UnitsTimeFlow.Add(() => TimeFlow, General.Multiply), _cts.Token);
        }
        
        public static void DestroyOnReset(GameObject gameObject) => _destroyOnReset.Add(gameObject);

        public static void MarkLose()
        {
            if (_alreadyMarked) return;
            _alreadyMarked = true;
            
            _isLose = true;
            OnLevelFinished?.Invoke(true);

            TimeFlowChange(0.1f, _cts.Token).Forget();
        }
        public static void MarkWon()
        {
            if (_alreadyMarked) return;
            _alreadyMarked = true;
            
            OnLevelFinished?.Invoke(false);
            TimeFlowChange(0.1f, _cts.Token).Forget();
        }

        static async UniTask TimeFlowChange(float to, CancellationToken ct)
        {
            float t = 0;
            while (t < 1 && !ct.IsCancellationRequested)
            {
                TimeFlow = Mathf.Lerp(TimeFlow, to, Time.deltaTime);
                t += Time.deltaTime;
                await UniTask.Yield(ct);
            }
            TimeFlow = to;
        }

        public static void NextLevel()
        {
            if (!_alreadyMarked) return;
            
            if (_isLose)
            {
                Difficulty.Instance.CurrentDifficulty.Value = 1;
            }
            else
                Difficulty.Instance.CurrentDifficulty.Value++;

            _isLose = false;

            foreach (var destroyOnReset in _destroyOnReset)
                if (destroyOnReset != null)
                    Object.Destroy(destroyOnReset);
            
            OnLevelStarted?.Invoke();
            TimeFlowChange(1f, _cts.Token).Forget();
            _alreadyMarked = false;
        }
        
        public static event Action<bool> OnLevelFinished;
        public static event Action OnLevelStarted;
    }
}