using UnityEngine;

/// <summary>
/// 敌人工厂
/// </summary>
[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [System.Serializable]
    class EnemyConfig
    {
        public Enemy prefab = default;

        /// <summary>
        /// 规模
        /// </summary>
        [FloatRangeSlider(0.5f, 2f)]
        public FloatRange scale = new FloatRange(1f);

        /// <summary>
        /// 速度
        /// </summary>
        [FloatRangeSlider(0.2f, 5f)]
        public FloatRange speed = new FloatRange(1f);

        /// <summary>
        /// 路径偏移量
        /// </summary>
        [FloatRangeSlider(-0.4f, 0.4f)]
        public FloatRange pathOffset = new FloatRange(0f);

        /// <summary>
        /// 生命
        /// </summary>
        [FloatRangeSlider(10f, 1000f)]
        public FloatRange health = new FloatRange(100f);
    }

    [SerializeField]
    EnemyConfig small = default, medium = default, large = default;

    

    //[SerializeField]
    //Enemy prefab = default;

    //[SerializeField, FloatRangeSlider(0.5f, 2f)]
    //FloatRange scale = new FloatRange(1f);

    //[SerializeField, FloatRangeSlider(-0.4f, 0.4f)]
    //FloatRange pathOffset = new FloatRange(0f);

    //[SerializeField, FloatRangeSlider(0.2f, 5f)]
    //FloatRange speed = new FloatRange(1f);


    /// <summary>
    /// 获得配置
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    EnemyConfig GetConfig(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Small: return small;
            case EnemyType.Medium: return medium;
            case EnemyType.Large: return large;
        }
        Debug.Assert(false, "Unsupported enemy type!");
        return null;
    }

    public Enemy Get(EnemyType type = EnemyType.Medium)
    {
        EnemyConfig config = GetConfig(type);
        Enemy instance = CreateGameObjectInstance(config.prefab);
        instance.OriginFactory = this;
        //instance.Initialize(scale.RandomValueInRange);
        instance.Initialize(
            config.scale.RandomValueInRange,
            config.speed.RandomValueInRange,
            config.pathOffset.RandomValueInRange,
            config.health.RandomValueInRange
        );
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
        Destroy(enemy.gameObject);
    }
}
