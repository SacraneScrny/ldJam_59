using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using R3;

using Sackrany.Actor.Managers;

using SackranyUI.Core.Base;
using SackranyUI.Core.Entities;

using UnityEngine;

using Unit = Sackrany.Actor.UnitMono.Unit;

namespace Game.Logic.UI.Screen
{
    public class TutorialViewModel : ViewModel<Tutorial>
    {
        [Bind("alpha")] ReactiveProperty<float> _alpha = new(0);
        
        [Bind("lookAt")] ReactiveProperty<Quaternion> _lookAt = new(Quaternion.identity);
        [Bind("playerPos")] ReactiveProperty<Vector3> _playerPos = new(Vector3.zero);
        [Bind("enemyPos")] ReactiveProperty<Vector3> _enemyPos = new(Vector3.zero);
        
        CancellationTokenSource _cts = new();

        Unit _enemy;
        Unit _player;
        
        protected override void OnInitialized()
        {
            Open();
            T(_cts.Token).Forget();
            R(_cts.Token).Forget();
        }

        async UniTask T(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);

            float t = 0;
            while (t < 1 && !token.IsCancellationRequested)
            {
                t += Time.deltaTime * 0.1f;
                _alpha.Value = t;
                await UniTask.Yield(token);
            }
            _alpha.Value = 1;
            
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
            
            t = 1;
            while (t > 0 && !token.IsCancellationRequested)
            {
                t -= Time.deltaTime * 0.1f;
                _alpha.Value = t;
                await UniTask.Yield(token);
            }
            _alpha.Value = 0;
        }
        async UniTask R(CancellationToken token)
        {
            UnitCmd.Execute((u) => u.Tag.HasTag<Sackrany.Actor.Traits.Tags.Enemy>(), (u) => _enemy = u);
            UnitCmd.Execute((u) => u.Tag.HasTag<Sackrany.Actor.Traits.Tags.Player>(), (u) => _player = u);
            
            await UniTask.WaitWhile(() => _enemy == null || _player == null, cancellationToken: token);
            var cam = UnityEngine.Camera.main;

            while (token.IsCancellationRequested == false)
            {
                var pPos = cam.WorldToScreenPoint(_player.transform.position);
                var ePos = cam.WorldToScreenPoint(_enemy.transform.position);

                _playerPos.Value = pPos;
                _enemyPos.Value = ePos;
                _lookAt.Value = Quaternion.LookRotation(_player.transform.position - pPos, Vector3.up);
                await UniTask.Yield(token);
            }
        }
    }
    
    [Serializable]
    public class Tutorial : ViewModelTemplate<TutorialViewModel> { }
}