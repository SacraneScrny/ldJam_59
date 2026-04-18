using R3;

using Sackrany.Utils;

namespace Game.Logic
{
    public class Difficulty : AManager<Difficulty>
    {
        public ReactiveProperty<int> CurrentDifficulty { get; private set; }

        public float EnemyAttackInterval = 1f;
        public float EnemyAttackAngle = 15;
        public float EnemyAttackAngleStep = .5f;
        public float EnemyAttackSignalStartSpeed = 6;
        public float EnemyHeadRotateSpeed = 5f;
    }
}