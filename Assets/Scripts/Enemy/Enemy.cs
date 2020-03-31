using UnityEngine;

/// <summary>
/// 敌人
/// </summary>
public class Enemy : GameBehavior
{

    EnemyFactory originFactory;

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress, progressFactor;

    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;

    [SerializeField]
    Transform model = default;

    [SerializeField]
    EnemyAnimationConfig animationConfig = default;

    EnemyAnimator animator;

    Collider targetPointCollider;

    float pathOffset;
    float speed;
    /// <summary>
    /// 生命
    /// </summary>
    float Health { get; set; }

    /// <summary>
    /// 规模
    /// </summary>
    public float Scale { get; private set; }
    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsValidTarget => animator.CurrentClip == EnemyAnimator.Clip.Move;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public Collider TargetPointCollider
    {
        set
        {
            Debug.Assert(targetPointCollider == null, "Redefined collider!");
            targetPointCollider = value;
        }
    }





    void Awake()
    {
        animator.Configure(
            model.GetChild(0).gameObject.AddComponent<Animator>(),
            animationConfig
        );
    }

    public void SpawnOn(GameTile tile)
    {
        //transform.localPosition = tile.transform.localPosition;
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        //positionFrom = tileFrom.transform.localPosition;
        //positionTo = tileTo.transform.localPosition;
        //positionTo = tileFrom.ExitPoint;
        //transform.localRotation = tileFrom.PathDirection.GetRotation();
        progress = 0f;
        PrepareIntro();
    }

    public override bool GameUpdate()
    {
        animator.GameUpdate();

        if (animator.CurrentClip == EnemyAnimator.Clip.Intro)
        {
            if (!animator.IsDone)
            {
                return true;
            }
            animator.PlayMove(speed / Scale);
            targetPointCollider.enabled = true;
        }
        else if (animator.CurrentClip >= EnemyAnimator.Clip.Outro)
        {
            if (animator.IsDone)
            {
                Recycle();
                return false;
            }
            return true;
        }


        if (Health <= 0f)
        {
            //OriginFactory.Reclaim(this);
            //Recycle();
            animator.PlayDying();
            targetPointCollider.enabled = false;
            return true;
        }

        progress += Time.deltaTime * progressFactor;
        while (progress >= 1f)
        {
            //tileFrom = tileTo;
            //tileTo = tileTo.NextTileOnPath;
            if (tileTo == null)
            {
                //OriginFactory.Reclaim(this);
                Game.EnemyReachedDestination();
                //Recycle();
                animator.PlayOutro();
                targetPointCollider.enabled = false;
                return true;
            }
            //positionFrom = positionTo;
            //positionTo = tileTo.transform.localPosition;
            //positionTo = tileFrom.ExitPoint;
            //transform.localRotation = tileFrom.PathDirection.GetRotation();
            //progress -= 1f;
            progress = (progress - 1f) / progressFactor;
            PrepareNextState();
            progress *= progressFactor;
        }
        //transform.localPosition += Vector3.forward * Time.deltaTime;
        //transform.localPosition =
        //    Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        if (directionChange == DirectionChange.None)
        {
            transform.localPosition =
                Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        }
        else
        {
            float angle = Mathf.LerpUnclamped(
                directionAngleFrom, directionAngleTo, progress
            );
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        transform.localPosition = positionFrom;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        positionFrom = positionTo;
        if (tileTo == null)
        {
            PrepareOutro();
            return;
        }
        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;

        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        progressFactor = speed;
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
        model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
    }

    void PrepareTurnAround()
    {
        directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localPosition = positionFrom;
        progressFactor =
            speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
    }

    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        progressFactor = 2f * speed;
    }

    /// <summary>
    /// 敌人初始化
    /// </summary>
    /// <param name="scale"></param>
    public void Initialize(float scale, float speed, float pathOffset, float health)
    {
        Scale = scale;
        model.localScale = new Vector3(scale, scale, scale);
        this.speed = speed;
        this.pathOffset = pathOffset;
        Health = health;
        animator.PlayIntro();
        targetPointCollider.enabled = false;
    }
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    public void ApplyDamage(float damage)
    {
        Debug.Assert(damage >= 0f, "Negative damage applied.");
        Health -= damage;
    }

    public override void Recycle()
    {
        animator.Stop();
        OriginFactory.Reclaim(this);
    }
}
