using UnityEngine;

/// <summary>
/// 武器工厂
/// </summary>
[CreateAssetMenu]
public class WarFactory : GameObjectFactory
{
    /// <summary>
    /// 炮弹预制体
    /// </summary>
    [SerializeField]
    Shell shellPrefab = default;
    /// <summary>
    /// 爆炸预制体
    /// </summary>
    [SerializeField]
    Explosion explosionPrefab = default;

    public Shell Shell => Get(shellPrefab);
    public Explosion Explosion => Get(explosionPrefab);

    T Get<T>(T prefab) where T : WarEntity
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }

    public void Reclaim(WarEntity entity)
    {
        Debug.Assert(entity.OriginFactory == this, "Wrong factory reclaimed!");
        Destroy(entity.gameObject);
    }
}
