using UnityEngine;

public abstract class Tower : GameTileContent
{
    //const int enemyLayerMask = 1 << 9;

    //static Collider[] targetsBuffer = new Collider[100];

    [SerializeField, Range(1.5f, 10.5f)]
    protected float targetingRange = 1.5f;

    public abstract TowerType TowerType { get; }

    /// <summary>
    /// 获得目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected bool AcquireTarget(out TargetPoint target)
    {
        //Vector3 a = transform.localPosition;
        //Vector3 b = a;
        //b.y += 3f;
        //int hits = Physics.OverlapCapsuleNonAlloc(
        //    a, b, targetingRange, targetsBuffer, enemyLayerMask
        //);
        //if (hits > 0)
        //{
        //    target = targetsBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
        //    Debug.Assert(target != null, "Targeted non-enemy!", targetsBuffer[0]);
        //    return true;
        //}
        if (TargetPoint.FillBuffer(transform.localPosition, targetingRange))
        {
            target = TargetPoint.RandomBuffered;
            return true;
        }
        target = null;
        return false;
    }
    /// <summary>
    /// 跟踪目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected bool TrackTarget(ref TargetPoint target)
    {
        if (target == null || !target.Enemy.IsValidTarget)
        {
            return false;
        }
        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;
        float x = a.x - b.x;
        float z = a.z - b.z;
        float r = targetingRange + 0.125f * target.Enemy.Scale;
        if (x * x + z * z > r * r)
        {
            target = null;
            return false;
        }
        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);
    }
}
