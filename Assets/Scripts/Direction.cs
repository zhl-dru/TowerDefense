using UnityEngine;

/// <summary>
/// 方向
/// </summary>
public enum Direction
{
    /// <summary>
    /// 北
    /// </summary>
    North,
    /// <summary>
    /// 东
    /// </summary>
    East,
    /// <summary>
    /// 南
    /// </summary>
    South,
    /// <summary>
    /// 西
    /// </summary>
    West
}


/// <summary>
/// 转变方向
/// </summary>
public enum DirectionChange
{
    /// <summary>
    /// 不变
    /// </summary>
    None,
    /// <summary>
    /// 向右转
    /// </summary>
    TurnRight,
    /// <summary>
    /// 向左转
    /// </summary>
    TurnLeft,
    /// <summary>
    /// 向后转
    /// </summary>
    TurnAround
}

/// <summary>
/// 方向扩展方法
/// </summary>
public static class DirectionExtensions
{

    static Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0f, 90f, 0f),
        Quaternion.Euler(0f, 180f, 0f),
        Quaternion.Euler(0f, 270f, 0f)
    };

    static Vector3[] halfVectors = {
        Vector3.forward * 0.5f,
        Vector3.right * 0.5f,
        Vector3.back * 0.5f,
        Vector3.left * 0.5f
    };



    /// <summary>
    /// 获得用于旋转的四元数
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }
    /// <summary>
    /// 获得变更的方向
    /// </summary>
    /// <param name="current"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public static DirectionChange GetDirectionChangeTo(
        this Direction current, Direction next
    )
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }
    /// <summary>
    /// 获得角度
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static float GetAngle(this Direction direction)
    {
        return (float)direction * 90f;
    }

    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int)direction];
    }
}
