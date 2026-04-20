using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Game.Logic.Level;
using Game.Logic.SignalWave;
using Game.Logic.UI.World;

using Sackrany.Actor.EventBus;
using Sackrany.Actor.Managers;
using Sackrany.Actor.Traits.Damage;
using Sackrany.Actor.Traits.Tags;
using Sackrany.Actor.UnitMono;
using Sackrany.Extensions;
using Sackrany.Utils;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Game.Logic.Enemy
{
    public class EnemyComponent : MonoBehaviour
    {
        public Transform Head;
        public Transform Body;

        float _health;

        SegmentedBar _healthBar;
        bool _isInited;
        bool _isAttacking;
        Unit _player;
        Unit _me;
        float _attackInterval;
        int _layer;

        CancellationToken _ct;
        
        void Start()
        {
            _healthBar = GetComponentInChildren<SegmentedBar>();
            _healthBar.ConnectLine(transform);
            _health = Difficulty.Instance.GetEnemyHealth();
            _healthBar.Spawn(Mathf.RoundToInt(_health));
            
            _layer = LayerMask.GetMask("Player", "Obstacle");
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
            Body.localRotation *= Quaternion.Euler(0f, 0, Random.Range(0, 180));

            _me = GetComponent<Unit>();
            _me.Event.Subscribe<Events.OnDamage, DamageInfo>((d) =>
            {
                _health -= d.Damage;
                for (int i = 0; i < d.Damage; i++)
                    _healthBar.Hit();
                CheckIfDead();
            });
        }

        void CheckIfDead()
        {
            if (_health > 0) return;
            GameLevelManager.MarkWon();
            _isInited = false;
        }
        
        void Update()
        {
            if (!_isInited) return;
            
            var dir = _player.transform.position.With(z: 0) - transform.position.With(z: 0);
            var hit = Physics2D.Raycast(transform.position.With(z: 0), dir, Mathf.Infinity, _layer);
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
                Difficulty.Instance.GetEnemyHeadRotateSpeed() * Time.deltaTime
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
                if (Vector2.Angle(dir, Head.transform.right) >= Difficulty.Instance.GetEnemyAttackAngle() / 2)
                {
                    await UniTask.Yield(_ct);
                    continue;
                }

                _attackInterval = Difficulty.Instance.GetEnemyAttackInterval();
                    
                float startAngle = -Difficulty.Instance.GetEnemyAttackAngle();
                float endAngle = -startAngle;

                while (startAngle <= endAngle)
                {
                    SignalWaveManager.Spawn(Head.position, Quaternion.Euler(0, 0, startAngle) * Head.right,
                        Difficulty.Instance.GetEnemyAttackSignalStartSpeed());
                    startAngle += Difficulty.Instance.GetEnemyAttackAngleStep();
                }

                await UniTask.Yield(_ct);
            }
        }
    }
}