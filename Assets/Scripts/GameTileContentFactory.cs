using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

/// <summary>
/// 地格内容工厂
/// </summary>
[CreateAssetMenu]
public class GameTileContentFactory : GameObjectFactory
{
    /// <summary>
    /// 目的地预制体
    /// </summary>
    [SerializeField]
    GameTileContent destinationPrefab = default;
    /// <summary>
    /// 空预制体
    /// </summary>
    [SerializeField]
    GameTileContent emptyPrefab = default;
    /// <summary>
    /// 墙预制体
    /// </summary>
    [SerializeField]
    GameTileContent wallPrefab = default;
    /// <summary>
    /// 敌人生成点预制体
    /// </summary>
    [SerializeField]
    GameTileContent spawnPointPrefab = default;
    /// <summary>
    /// 塔预制体
    /// </summary>
    [SerializeField]
    Tower[] towerPrefabs = default;

    /// <summary>
    /// 回收
    /// </summary>
    /// <param name="content"></param>
    public void Reclaim(GameTileContent content)
    {
        Debug.Assert(content.OriginFactory == this, "Wrong factory reclaimed!");
        Destroy(content.gameObject);
    }
    /// <summary>
    /// 获得实例
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    T Get<T>(T prefab) where T : GameTileContent
    {
        T instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        return instance;
    }

    /// <summary>
    /// 获得预制体实例
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameTileContent Get(GameTileContentType type)
    {
        switch (type)
        {
            case GameTileContentType.Destination: return Get(destinationPrefab);
            case GameTileContentType.Empty: return Get(emptyPrefab);
            case GameTileContentType.Wall: return Get(wallPrefab);
            case GameTileContentType.SpawnPoint: return Get(spawnPointPrefab);
            //case GameTileContentType.Tower: return Get(towerPrefab);
        }
        Debug.Assert(false, "Unsupported non-tower type: " + type);
        return null;
    }
    /// <summary>
    /// 获得塔实例
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Tower Get(TowerType type)
    {
        Debug.Assert((int)type < towerPrefabs.Length, "Unsupported tower type!");
        Tower prefab = towerPrefabs[(int)type];
        Debug.Assert(type == prefab.TowerType, "Tower prefab at wrong index!");
        return Get(prefab);
    }
}
