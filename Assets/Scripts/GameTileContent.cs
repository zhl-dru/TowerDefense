using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地格内容
/// </summary>
[SelectionBase]
public class GameTileContent : MonoBehaviour
{
    /// <summary>
    /// 地格类型
    /// </summary>
    [SerializeField]
    GameTileContentType type = default;
    /// <summary>
    /// 初始工厂
    /// </summary>
    GameTileContentFactory originFactory;
    /// <summary>
    /// 地格类型
    /// </summary>
    public GameTileContentType Type => type;
    /// <summary>
    /// 初始工厂
    /// </summary>
    public GameTileContentFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }
    /// <summary>
    /// 是否阻碍道路
    /// </summary>
    public bool BlocksPath =>
        Type == GameTileContentType.Wall || Type == GameTileContentType.Tower;

    public void Recycle()
    {
        originFactory.Reclaim(this);
    }

    public virtual void GameUpdate() { }
}
