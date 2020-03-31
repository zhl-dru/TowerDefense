using UnityEngine;

/// <summary>
/// 敌人生成序列
/// </summary>
[System.Serializable]
public class EnemySpawnSequence
{
    /// <summary>
    /// 工厂
    /// </summary>
    [SerializeField]
    EnemyFactory factory = default;
    /// <summary>
    /// 类型
    /// </summary>
    [SerializeField]
    EnemyType type = EnemyType.Medium;
    /// <summary>
    /// 数量
    /// </summary>
    [SerializeField, Range(1, 100)]
    int amount = 1;
    /// <summary>
    /// 冷却
    /// </summary>
    [SerializeField, Range(0.1f, 10f)]
    float cooldown = 1f;


    public State Begin() => new State(this);

    [System.Serializable]
    public struct State
    {
        int count;

        float cooldown;

        EnemySpawnSequence sequence;

        public State(EnemySpawnSequence sequence)
        {
            this.sequence = sequence;
            count = 0;
            cooldown = sequence.cooldown;
        }

        public float Progress(float deltaTime)
        {
            cooldown += deltaTime;
            while (cooldown >= sequence.cooldown)
            {
                cooldown -= sequence.cooldown;
                if (count >= sequence.amount)
                {
                    return cooldown;
                }
                count += 1;
                Game.SpawnEnemy(sequence.factory, sequence.type);
            }
            return -1f;
        }
    }
}
