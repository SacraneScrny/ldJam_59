using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Game.Logic.SignalWave;

using Sackrany.Actor.Managers;
using Sackrany.Actor.Traits.Tags;
using Sackrany.Actor.UnitMono;
using Sackrany.Extensions;
using Sackrany.Utils;

using UnityEngine;

namespace Game.Logic.Enemy
{
    public class EnemyComponent : MonoBehaviour
    {
        public Transform Head;
        
        bool _isInited;
        bool _isAttacking;
        Unit _player;
        float _attackInterval;

        CancellationToken _ct;
        
        void Start()
        {
            _ct = gameObject.GetCancellationTokenOnDestroy();
            UnitCmd.Execute(
                (u) => u.Tag.HasTag<Player>(), 
                (u) =>
                    {
                        _player = u;
                        _isInited = true;
                    }
                );
            Attack().Forget();
        }
        
        void Update()
        {
            if (!_isInited) return;
            
            var dir = _player.transform.position.With(z: 0) - transform.position.With(z: 0);
            var hit = Physics2D.Raycast(transform.position.With(z: 0), dir, Mathf.Infinity);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
            {
                _isAttacking = true;
            }
            else
                _isAttacking = false;
            LookAtPlayer();
        }

        void LookAtPlayer()
        {
            if (!_isAttacking)
            {
                return;
            }
            Vector2 dir = (_player.transform.position - transform.position);

            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            float angle = Mathf.LerpAngle(
                Head.transform.eulerAngles.z,
                targetAngle,
                Difficulty.Instance.EnemyHeadRotateSpeed * Time.deltaTime
            );

            Head.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        async UniTaskVoid Attack()
        {
            while (true)
            {
                if (_ct.IsCancellationRequested) break;
                
                if (!_isAttacking)
                {
                    await UniTask.Yield(_ct);
                    continue;
                }
                
                if (_attackInterval > 0)
                {
                    _attackInterval -= Time.deltaTime;
                    await UniTask.Yield(_ct);
                    continue;
                }
                
                Vector2 dir = (_player.transform.position - transform.position);
                if (Vector2.Angle(dir, Head.transform.right) >= Difficulty.Instance.EnemyAttackAngle / 2)
                {
                    await UniTask.Yield(_ct);
                    continue;
                }

                _attackInterval = Difficulty.Instance.EnemyAttackInterval;
                    
                float startAngle = -Difficulty.Instance.EnemyAttackAngle;
                float endAngle = -startAngle;

                while (startAngle <= endAngle)
                {
                    SignalWaveManager.Spawn(Head.position, Quaternion.Euler(0, 0, startAngle) * Head.right,
                        Difficulty.Instance.EnemyAttackSignalStartSpeed);
                    startAngle += Difficulty.Instance.EnemyAttackAngleStep;
                }

                await UniTask.Yield(_ct);
            }
        }
    }
}