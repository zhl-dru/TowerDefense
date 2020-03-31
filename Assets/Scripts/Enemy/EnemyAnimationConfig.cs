using UnityEngine;

/// <summary>
/// 敌人动画配置
/// </summary>
[CreateAssetMenu]
public class EnemyAnimationConfig : ScriptableObject
{

    [SerializeField]
    AnimationClip move = default, intro = default, outro = default, dying = default;

    public AnimationClip Move => move;

    public AnimationClip Intro => intro;

    public AnimationClip Outro => outro;

    public AnimationClip Dying => dying;
}
