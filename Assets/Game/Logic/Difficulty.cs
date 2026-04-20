using R3;

using Sackrany.Utils;

using UnityEngine;

namespace Game.Logic
{
    public class Difficulty : AManager<Difficulty>
    {
        public ReactiveProperty<int> CurrentDifficulty { get; private set; } = new(1);

        [SerializeField] int _levelSize = 6;
        [SerializeField] float _enemyHealth = 1;
        [SerializeField] float _enemyAttackInterval = 3;
        [SerializeField] float _enemyAttackAngle = 30;
        [SerializeField] float _enemyAttackAngleStep = 0.5f;
        [SerializeField] float _enemyAttackSignalStartSpeed = 4.5f;
        [SerializeField] float _enemyHeadRotateSpeed = 1;

        int _difficulty => CurrentDifficulty.Value;

        public int GetLevelSize() => _levelSize + _difficulty;
        public float GetEnemyHealth()
        {
            return _enemyHealth + Mathf.RoundToInt(Mathf.Pow(_difficulty - 1, 0.6f));
        }
        public float GetEnemyAttackInterval() => _enemyAttackInterval / Mathf.Sqrt(_difficulty);
        public float GetEnemyAttackAngle() => _enemyAttackAngle + (_difficulty - 1) * 5f;
        public float GetEnemyAttackAngleStep() => _enemyAttackAngleStep * (1f / Mathf.Pow(_difficulty, 0.2f));
        public float GetEnemyAttackSignalStartSpeed() => _enemyAttackSignalStartSpeed + (_difficulty - 1) * 0.1f;
        public float GetEnemyHeadRotateSpeed() => _enemyHeadRotateSpeed + (_difficulty - 1) * 0.1f;
        
        protected override void OnManagerDestroy()
        {
            CurrentDifficulty.Dispose();
        }
    }
}