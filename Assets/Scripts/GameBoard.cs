using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    /// <summary>
    /// 地面
    /// </summary>
    [SerializeField]
    Transform ground = default;
    /// <summary>
    /// 地格预制体
    /// </summary>
    [SerializeField]
    GameTile tilePrefab = default;
    /// <summary>
    /// 网格纹理
    /// </summary>
    [SerializeField]
    Texture2D gridTexture = default;

    /// <summary>
    /// 地面大小
    /// </summary>
    Vector2Int size;
    /// <summary>
    /// 地格对象
    /// </summary>
    GameTile[] tiles;
    /// <summary>
    /// 地格内容工厂
    /// </summary>
    GameTileContentFactory contentFactory;
    /// <summary>
    /// 搜索队列
    /// </summary>
    Queue<GameTile> searchFrontier = new Queue<GameTile>();
    /// <summary>
    /// 生成点列表
    /// </summary>
    List<GameTile> spawnPoints = new List<GameTile>();
    /// <summary>
    /// 更新内容列表
    /// </summary>
    List<GameTileContent> updatingContent = new List<GameTileContent>();
    /// <summary>
    /// 是否显示
    /// </summary>
    bool showGrid, showPaths;
    /// <summary>
    /// 是否显示路径
    /// </summary>
    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (GameTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (GameTile tile in tiles)
                {
                    tile.HidePath();
                }
            }
        }
    }
    /// <summary>
    /// 是否显示网格
    /// </summary>
    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;
            Material m = ground.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTexture;
                m.SetTextureScale("_MainTex", size);
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }
    /// <summary>
    /// 生成点数量
    /// </summary>
    public int SpawnPointCount => spawnPoints.Count;







    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="size"></param>
    /// <param name="contentFactory"></param>
    public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
    {
        this.size = size;
        this.contentFactory = contentFactory;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        //for(int y = 0; y < size.y; y++)
        //{
        //    for(int x = 0; x < size.x; x++)
        //    {
        //        GameTile tile = Instantiate(tilePrefab);
        //        tile.transform.SetParent(transform, false);
        //        tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
        //    }
        //}

        tiles = new GameTile[size.x * size.y];
        for(int i = 0, y = 0; y < size.y; y++)
        {
            for(int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = tiles[i] = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
                }
                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }
                //tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }

        //FindPaths();
        //ToggleDestination(tiles[tiles.Length / 2]);
        //ToggleSpawnPoint(tiles[0]);
        Clear();
    }
    /// <summary>
    /// 查找路径
    /// </summary>
    /// <returns></returns>
    bool FindPaths()
    {
        foreach (GameTile tile in tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }
        //tiles[tiles.Length / 2].BecomeDestination();
        //searchFrontier.Enqueue(tiles[tiles.Length / 2]);
        if (searchFrontier.Count == 0)
        {
            return false;
        }

        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathWest());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathNorth());
                }
            }
        }

        foreach (GameTile tile in tiles)
        {
            if (!tile.HasPath)
            {
                return false;
            }
        }

        if (showPaths)
        {
            foreach (GameTile tile in tiles)
            {
                tile.ShowPath();
            }
        }

        return true;
    }
    /// <summary>
    /// 获得地格
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return tiles[x + y * size.x];
            }
        }
        return null;
    }
    /// <summary>
    /// 获得生成点
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameTile GetSpawnPoint(int index)
    {
        return spawnPoints[index];
    }

    /// <summary>
    /// 切换目的地
    /// </summary>
    /// <param name="tile"></param>
    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content =
                    contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }
    /// <summary>
    /// 切换墙
    /// </summary>
    /// <param name="tile"></param>
    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }
    /// <summary>
    /// 切换敌人生成点
    /// </summary>
    /// <param name="tile"></param>
    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if (spawnPoints.Count > 1)
            {
                spawnPoints.Remove(tile);
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
            spawnPoints.Add(tile);
        }
    }
    /// <summary>
    /// 切换塔
    /// </summary>
    /// <param name="tile"></param>
    public void ToggleTower(GameTile tile, TowerType towerType)
    {
        if (tile.Content.Type == GameTileContentType.Tower)
        {
            updatingContent.Remove(tile.Content);
            if (((Tower)tile.Content).TowerType == towerType)
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
            else
            {
                tile.Content = contentFactory.Get(towerType);
                updatingContent.Add(tile.Content);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(towerType);
            if(FindPaths()) {
                updatingContent.Add(tile.Content);
            }
            else
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
        else if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(towerType);
            updatingContent.Add(tile.Content);
        }
    }

    public void GameUpdate()
    {
        for (int i = 0; i < updatingContent.Count; i++)
        {
            updatingContent[i].GameUpdate();
        }
    }

    public void Clear()
    {
        foreach (GameTile tile in tiles)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
        }
        spawnPoints.Clear();
        updatingContent.Clear();
        ToggleDestination(tiles[tiles.Length / 2]);
        ToggleSpawnPoint(tiles[0]);
    }
}
