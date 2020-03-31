using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    /// <summary>
    /// 箭头
    /// </summary>
    [SerializeField]
    Transform arrow = default;

    /// <summary>
    /// 方位格子
    /// </summary>
    GameTile north, east, south, west, nextOnPath;
    /// <summary>
    /// 箭头旋转
    /// </summary>
    static Quaternion
        northRotation = Quaternion.Euler(90f, 0f, 0f),
        eastRotation = Quaternion.Euler(90f, 90f, 0f),
        southRotation = Quaternion.Euler(90f, 180f, 0f),
        westRotation = Quaternion.Euler(90f, 270f, 0f);
    /// <summary>
    /// 地格内容
    /// </summary>
    GameTileContent content;

    /// <summary>
    /// 距目的地的距离
    /// </summary>
    int distance;

    /// <summary>
    /// 是否在路径上
    /// </summary>
    public bool HasPath => distance != int.MaxValue;
    /// <summary>
    /// 路径修正
    /// </summary>
    public bool IsAlternative { get; set; }
    /// <summary>
    /// 地格内容
    /// </summary>
    public GameTileContent Content
    {
        get => content;
        set
        {
            Debug.Assert(value != null, "Null assigned to content!");
            if (content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }
    /// <summary>
    /// 路径上下一地格
    /// </summary>
    public GameTile NextTileOnPath => nextOnPath;
    /// <summary>
    /// 出口点
    /// </summary>
    public Vector3 ExitPoint { get; private set; }
    /// <summary>
    /// 路径方向
    /// </summary>
    public Direction PathDirection { get; private set; }



    /// <summary>
    /// 设置东西方邻居
    /// </summary>
    /// <param name="east"></param>
    /// <param name="west"></param>
    public static void MakeEastWestNeighbors(GameTile east,GameTile west)
    {
        Debug.Assert(west.east == null && east.west == null, "Redefined neighbors!");

        west.east = east;
        east.west = west;
    }
    /// <summary>
    /// 设置南北方邻居
    /// </summary>
    /// <param name="north"></param>
    /// <param name="south"></param>
    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        Debug.Assert(
            south.north == null && north.south == null, "Redefined neighbors!"
        );
        south.north = north;
        north.south = south;
    }
    /// <summary>
    /// 清除路径
    /// </summary>
    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }
    /// <summary>
    /// 成为目的地
    /// </summary>
    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }
    /// <summary>
    /// 向邻居扩展路径
    /// </summary>
    /// <param name="neighbor"></param>
    /// <returns></returns>
    GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        if (!HasPath || neighbor == null || neighbor.HasPath)
        {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint =
            (neighbor.transform.localPosition + transform.localPosition) * 0.5f;
        neighbor.PathDirection = direction;
        //return neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null;
        return neighbor.Content.BlocksPath ? null : neighbor;
    }
    /// <summary>
    /// 向北扩展路径
    /// </summary>
    /// <returns></returns>
    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
    /// <summary>
    /// 向东扩展路径
    /// </summary>
    /// <returns></returns>
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    /// <summary>
    /// 向南扩展路径
    /// </summary>
    /// <returns></returns>
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    /// <summary>
    /// 向西扩展路径
    /// </summary>
    /// <returns></returns>
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);
    /// <summary>
    /// 显示路径
    /// </summary>
    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
            nextOnPath == north ? northRotation :
            nextOnPath == east ? eastRotation :
            nextOnPath == south ? southRotation :
            westRotation;
    }
    /// <summary>
    /// 隐藏路径
    /// </summary>
    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }
}
