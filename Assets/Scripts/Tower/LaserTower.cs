using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
    /// <summary>
    /// 塔预制体
    /// </summary>
    Tower towerPrefab = default;

    [SerializeField]
    Transform turret = default, laserBeam = default;
    /// <summary>
    /// 造成伤害
    /// </summary>
    [SerializeField, Range(1f, 100f)]
    float damagePerSecond = 10f;

    public override TowerType TowerType => TowerType.Laser;

    /// <summary>
    /// 目标
    /// </summary>
    TargetPoint target;

    Vector3 laserBeamScale;



    void Awake()
    {
        laserBeamScale = laserBeam.localScale;
    }

    public override void GameUpdate()
    {
        if (TrackTarget(ref target) || AcquireTarget(out target))
        {
            Shoot();
        }
        else
        {
            laserBeam.localScale = Vector3.zero;
        }
    }
    
    /// <summary>
    /// 射击
    /// </summary>
    void Shoot()
    {
        Vector3 point = target.Position;
        turret.LookAt(point);
        laserBeam.localRotation = turret.localRotation;

        float d = Vector3.Distance(turret.position, point);
        laserBeamScale.z = d;
        laserBeam.localScale = laserBeamScale;
        laserBeam.localPosition =
            turret.localPosition + 0.5f * d * laserBeam.forward;
        target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
    }
}
