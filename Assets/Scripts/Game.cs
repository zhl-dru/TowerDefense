using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    /// <summary>
    /// 场地大小
    /// </summary>
    [SerializeField]
    Vector2Int boardSize = new Vector2Int(11, 11);
    /// <summary>
    /// 场地
    /// </summary>
    [SerializeField]
    GameBoard board = default;
    /// <summary>
    /// 地格内容工厂
    /// </summary>
    [SerializeField]
    GameTileContentFactory tileContentFactory = default;
    /// <summary>
    /// 敌人工厂
    /// </summary>
    //[SerializeField]
    //EnemyFactory enemyFactory = default;
    /// <summary>
    /// 武器工厂
    /// </summary>
    [SerializeField]
    WarFactory warFactory = default;
    /// <summary>
    /// 敌人生成速度
    /// </summary>
    //[SerializeField, Range(0.1f, 10f)]
    //float spawnSpeed = 1f;
    /// <summary>
    /// 敌人生成进度
    /// </summary>
    //float spawnProgress;
    /// <summary>
    /// 控制射线
    /// </summary>
    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);
    /// <summary>
    /// 敌人集合
    /// </summary>
    GameBehaviorCollection enemies = new GameBehaviorCollection();
    /// <summary>
    /// 非敌人集合
    /// </summary>
    GameBehaviorCollection nonEnemies = new GameBehaviorCollection();
    /// <summary>
    /// 场景
    /// </summary>
    [SerializeField]
    GameScenario scenario = default;
    /// <summary>
    /// 游戏速度
    /// </summary>
    [SerializeField, Range(1f, 10f)]
    float playSpeed = 1f;
    /// <summary>
    /// 玩家生命
    /// </summary>
    [SerializeField, Range(0, 100)]
    int startingPlayerHealth = 10;


    TowerType selectedTowerType;

    static Game instance;

    GameScenario.State activeScenario;
    /// <summary>
    /// 玩家生命
    /// </summary>
    int playerHealth;
    /// <summary>
    /// 暂停标记
    /// </summary>
    const float pausedTimeScale = 0f;

    private void Awake()
    {
        playerHealth = startingPlayerHealth;
        board.Initialize(boardSize, tileContentFactory);
        board.ShowGrid = true;
        activeScenario = scenario.Begin();
    }

    private void OnValidate()
    {
        if (boardSize.x < 2)
        {
            boardSize.y = 2;
        }
        if (boardSize.y < 2)
        {
            boardSize.y = 2;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
        else if (Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPaths = !board.ShowPaths;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedTowerType = TowerType.Laser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedTowerType = TowerType.Mortar;
        }

        //spawnProgress += spawnSpeed * Time.deltaTime;
        //while (spawnProgress >= 1f)
        //{
        //    spawnProgress -= 1f;
        //    SpawnEnemy();
        //}

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale =
                Time.timeScale > pausedTimeScale ? pausedTimeScale : playSpeed;
        }
        else if (Time.timeScale > pausedTimeScale)
        {
            Time.timeScale = playSpeed;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BeginNewGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (playerHealth <= 0 && startingPlayerHealth > 0)
        {
            Debug.Log("Defeat!");
            BeginNewGame();
        }

        if (!activeScenario.Progress() && enemies.IsEmpty)
        {
            Debug.Log("Victory!");
            BeginNewGame();
            activeScenario.Progress();
        }

        enemies.GameUpdate();
        Physics.SyncTransforms();
        board.GameUpdate();
        nonEnemies.GameUpdate();
    }

    void OnEnable()
    {
        instance = this;
    }





    /// <summary>
    /// 生成敌人
    /// </summary>
    //void SpawnEnemy()
    //{
    //    GameTile spawnPoint =
    //        board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
    //    Enemy enemy = enemyFactory.Get((EnemyType)(Random.Range(0, 3)));
    //    enemy.SpawnOn(spawnPoint);
    //    enemies.Add(enemy);
    //}
    public static void SpawnEnemy(EnemyFactory factory, EnemyType type)
    {
        GameTile spawnPoint = instance.board.GetSpawnPoint(
            Random.Range(0, instance.board.SpawnPointCount)
        );
        Enemy enemy = factory.Get(type);
        enemy.SpawnOn(spawnPoint);
        instance.enemies.Add(enemy);
    }
    /// <summary>
    /// 生成炮弹
    /// </summary>
    /// <returns></returns>
    public static Shell SpawnShell()
    {
        Shell shell = instance.warFactory.Shell;
        instance.nonEnemies.Add(shell);
        return shell;
    }
    /// <summary>
    /// 生成爆炸
    /// </summary>
    /// <returns></returns>
    public static Explosion SpawnExplosion()
    {
        Explosion explosion = instance.warFactory.Explosion;
        instance.nonEnemies.Add(explosion);
        return explosion;
    }
    /// <summary>
    /// 敌人到达
    /// </summary>
    public static void EnemyReachedDestination()
    {
        instance.playerHealth -= 1;
    }




    /// <summary>
    /// 控制1
    /// </summary>
    void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            //tile.Content = tileContentFactory.Get(GameTileContentType.Destination);
            //board.ToggleWall(tile);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleTower(tile, selectedTowerType);
            }
            else
            {
                board.ToggleWall(tile);
            }
        }
    }
    /// <summary>
    /// 控制2
    /// </summary>
    void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if (tile != null)
        {
            //board.ToggleDestination(tile);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleDestination(tile);
            }
            else
            {
                board.ToggleSpawnPoint(tile);
            }
        }
    }


    /// <summary>
    /// 开始新游戏
    /// </summary>
    void BeginNewGame()
    {
        playerHealth = startingPlayerHealth;
        enemies.Clear();
        nonEnemies.Clear();
        board.Clear();
        activeScenario = scenario.Begin();
    }
}
