using UnityEngine;

/// <summary>
/// 目标点
/// </summary>
public class TargetPoint : MonoBehaviour
{
    /// <summary>
    /// 敌人
    /// </summary>
    public Enemy Enemy { get; private set; }
    /// <summary>
    /// 世界位置
    /// </summary>
    public Vector3 Position => transform.position;

    const int enemyLayerMask = 1 << 9;

    static Collider[] buffer = new Collider[100];

    public static int BufferedCount { get; private set; }
    /// <summary>
    /// 获得随机目标
    /// </summary>
    public static TargetPoint RandomBuffered =>
        GetBuffered(Random.Range(0, BufferedCount));



    void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(Enemy != null, "Target point without Enemy root!", this);
        Debug.Assert(
            GetComponent<SphereCollider>() != null,
            "Target point without sphere collider!", this
        );
        Debug.Assert(gameObject.layer == 9, "Target point on wrong layer!", this);
        Enemy.TargetPointCollider = GetComponent<Collider>();
    }


    /// <summary>
    /// 范围内目标列表
    /// </summary>
    /// <param name="position"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static bool FillBuffer(Vector3 position, float range)
    {
        Vector3 top = position;
        top.y += 3f;
        BufferedCount = Physics.OverlapCapsuleNonAlloc(
            position, top, range, buffer, enemyLayerMask
        );
        return BufferedCount > 0;
    }
    /// <summary>
    /// 获得目标
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static TargetPoint GetBuffered(int index)
    {
        var target = buffer[index].GetComponent<TargetPoint>();
        Debug.Assert(target != null, "Targeted non-enemy!", buffer[0]);
        return target;
    }
}
