using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

using Sackrany.Utils;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Game.Logic.Level.Components
{
    public class LevelGenerator : AManager<LevelGenerator>
    {
        public GameObject PlayerPrefab;
        public GameObject EnemyPrefab;

        public float CellSize = 1;
        public Vector2Int StartGridSize = new Vector2Int(64, 64);
        public GameObject[] WallPrefabs;

        public Rect LevelRect { get; private set; }

        List<GameObject> _temp = new();
        Dictionary<string, int> _wallsToType = new();
        Dictionary<int, Stack<GameObject>> _wallPools = new();
        CancellationTokenSource _cts;

        Vector2Int _gridSize;
        Vector2Int _playerPosition;
        Vector2Int _enemyPosition;

        int[,] _grid;
        GameObject[,] _walls;

        void Start()
        {
            for (int i = 0; i < WallPrefabs.Length; i++)
                _wallsToType.Add(WallPrefabs[i].name, i);

            _cts = CancellationTokenSource.CreateLinkedTokenSource(gameObject.GetCancellationTokenOnDestroy());
            Generate(_cts.Token).Forget();
        }

        async UniTaskVoid Generate(CancellationToken ct)
        {
            foreach (var t in _temp)
                Destroy(t);
            _temp.Clear();

            if (_walls != null)
                for (int y = 0; y < _gridSize.y; y++)
                    for (int x = 0; x < _gridSize.x; x++)
                        if (_walls[x, y] != null)
                            Release(_walls[x, y]);

            _gridSize = StartGridSize;
            _grid = new int[_gridSize.x, _gridSize.y];
            _walls = new GameObject[_gridSize.x, _gridSize.y];

            LevelRect = new Rect(
                (_gridSize.x / -2f) * CellSize,
                (_gridSize.y / -2f) * CellSize,
                _gridSize.x * CellSize,
                _gridSize.y * CellSize);

            _enemyPosition = new Vector2Int(_gridSize.x / 2, _gridSize.y / 2);

            await Prewarm(ct).SuppressCancellationThrow();
            if (ct.IsCancellationRequested) return;
            await Carving(ct).SuppressCancellationThrow();
            if (ct.IsCancellationRequested) return;
            await PostProcessing(ct).SuppressCancellationThrow();
            if (ct.IsCancellationRequested) return;

            Spawn();
        }

        async UniTask Prewarm(CancellationToken ct)
        {
            for (int y = 0; y < _gridSize.y; y++)
                for (int x = 0; x < _gridSize.x; x++)
                {
                    if ((x + y * _gridSize.x) % 50 == 0) await UniTask.Yield(ct);

                    var wall = GetWall(Random.Range(0, WallPrefabs.Length));
                    wall.transform.position =
                        new Vector3(x * CellSize, y * CellSize, 0)
                        - new Vector3((_gridSize.x / 2f) * CellSize, (_gridSize.y / 2f) * CellSize)
                        - new Vector3(CellSize / 2f, CellSize / 2f);
                    wall.transform.localScale = Vector3.one * CellSize;

                    _walls[x, y] = wall;
                    _grid[x, y] = 1;
                }
            await UniTask.Yield(ct);
        }
        async UniTask Carving(CancellationToken ct)
        {
            bool IsAvailableForCarving(int ox, int oy)
            {
                if (ox == 0 || oy == 0) return false;
                if (ox == _gridSize.x - 1 || oy == _gridSize.y - 1) return false;
                
                int neighbours = 0;
                for (int dy = -1; dy <= 1; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (dx * dx + dy * dy > 1) continue;
                        
                        int nx = ox + dx, ny = oy + dy;
                        if (nx < 0 || nx >= _gridSize.x || ny < 0 || ny >= _gridSize.y) continue;
                        
                        if (_grid[nx, ny] == 1) continue;
                        if (++neighbours > 1) return false;
                    }
                return true;
            }
            
            float farestD = 0;
            void CarveCell(int x, int y)
            {
                float d = Vector2.Distance(_enemyPosition, new Vector2Int(x, y));
                if (d > farestD)
                {
                    farestD = d;
                    _playerPosition = new Vector2Int(x, y);
                }
                
                Release(_walls[x, y]);
                _grid[x, y] = 0;
            }

            List<(int x, int y)> WallNeighbours(int ox, int oy)
            {
                var ret = new List<(int x, int y)>();
                for (int dy = -1; dy <= 1; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (dx * dx + dy * dy > 1) continue;
                        int nx = ox + dx, ny = oy + dy;
                        if (nx < 0 || nx >= _gridSize.x || ny < 0 || ny >= _gridSize.y) continue;
                        if (_grid[nx, ny] == 1) ret.Add((nx, ny));
                    }
                return ret;
            }
            var front = new List<(int x, int y)>
            {
                (_enemyPosition.x, _enemyPosition.y)
            };
            
            int cnt = 0;
            while (front.Count > 0)
            {
                if (cnt % 60 == 0) await UniTask.Yield(ct);

                bool carved = false;
                while (!carved && front.Count > 0)
                {
                    int num = Random.Range(0, front.Count);
                    var (fx, fy) = front[num];
                    front.RemoveAt(num);

                    if (_grid[fx, fy] == 0) continue;
                    if (!IsAvailableForCarving(fx, fy)) continue;

                    CarveCell(fx, fy);
                    front.AddRange(WallNeighbours(fx, fy));
                    carved = true;
                }

                cnt++;
            }

            await UniTask.Yield(ct);
        }
        async UniTask PostProcessing(CancellationToken ct)
        {
            bool IsAlone(int ox, int oy)
            {
                int offset = 0;
                int neighbours = 0;
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (x == 0 && y == 0) continue;
                        if (x * x + y * y > 1)
                            continue;
                        
                        var nx = ox + x;
                        var ny = oy + y;
                        
                        if (nx < 0 || nx >= _gridSize.x) continue;
                        if (ny < 0 || ny >= _gridSize.y) continue;
                        
                        if (_grid[nx, ny] == 1) continue;
                        
                        if (nx == 0) offset++;
                        if (ny == 0) offset++;
                        if (nx == _gridSize.x - 1) offset++;
                        if (ny == _gridSize.y - 1) offset++;
                        
                        neighbours++;
                        if (neighbours >= 4 - offset) return true;
                    }
                }
                return false;
            }
            
            for (int y = 0; y < _gridSize.y; y++)
                for (int x = 0; x < _gridSize.x; x++)
                {
                    if (x + y * _gridSize.x % 60 == 0) await UniTask.Yield(ct);
                    if (IsAlone(x, y))
                    {
                        Release(_walls[x, y]);
                        _grid[x, y] = 0;
                    }
                }
        }

        void Spawn()
        {
            var p = Instantiate(PlayerPrefab);
            p.transform.position = ToWorldPos(_playerPosition);
            p.SetActive(true);
            _temp.Add(p);

            var e = Instantiate(EnemyPrefab);
            e.transform.position = ToWorldPos(_enemyPosition);
            e.SetActive(true);
            _temp.Add(e);
        }

        GameObject GetWall(int num)
        {
            if (!_wallPools.TryGetValue(num, out var pool))
            {
                pool = new Stack<GameObject>();
                _wallPools.Add(num, pool);
            }
            var go = pool.Count > 0 ? pool.Pop() : CreateWall(num);
            go.SetActive(true);
            return go;
        }

        GameObject CreateWall(int num)
        {
            var go = Instantiate(WallPrefabs[num]);
            go.name = WallPrefabs[num].name;
            return go;
        }

        void Release(GameObject go)
        {
            if (go == null || !go.activeSelf) return;
            if (!_wallsToType.TryGetValue(go.name, out var num)) return;
            if (!_wallPools.TryGetValue(num, out var pool))
            {
                pool = new Stack<GameObject>();
                _wallPools.Add(num, pool);
            }
            go.SetActive(false);
            pool.Push(go);
        }

        public static Vector3 ToWorldPos(Vector2Int pos)
        {
            var ret = new Vector3(pos.x * Instance.CellSize, pos.y * Instance.CellSize, 0);
            ret -= new Vector3((Instance._gridSize.x / 2f) * Instance.CellSize, (Instance._gridSize.y / 2f) * Instance.CellSize);
            ret -= new Vector3(Instance.CellSize / 2f, Instance.CellSize / 2f);
            return ret;
        }

        public static bool IsInWall(Vector3 pos)
        {
            pos += new Vector3((Instance._gridSize.x / 2f) * Instance.CellSize, (Instance._gridSize.y / 2f) * Instance.CellSize);
            pos += new Vector3(Instance.CellSize / 2f, Instance.CellSize / 2f);

            var cellPos = new Vector2Int(
                Mathf.RoundToInt(pos.x / Instance.CellSize),
                Mathf.RoundToInt(pos.y / Instance.CellSize));

            if (cellPos.x < 0 || cellPos.x >= Instance._gridSize.x) return false;
            if (cellPos.y < 0 || cellPos.y >= Instance._gridSize.y) return false;

            return Instance._grid[cellPos.x, cellPos.y] != 0;
        }

        public static Vector3 GetNormal(Vector3 pos)
        {
            var worldPos = pos;
            pos += new Vector3((Instance._gridSize.x / 2f) * Instance.CellSize, (Instance._gridSize.y / 2f) * Instance.CellSize);
            pos += new Vector3(Instance.CellSize / 2f, Instance.CellSize / 2f);

            var cellPos = new Vector2Int(
                Mathf.RoundToInt(pos.x / Instance.CellSize),
                Mathf.RoundToInt(pos.y / Instance.CellSize));

            if (cellPos.x < 0 || cellPos.x >= Instance._gridSize.x) return Vector3.zero;
            if (cellPos.y < 0 || cellPos.y >= Instance._gridSize.y) return Vector3.zero;
            if (Instance._grid[cellPos.x, cellPos.y] == 0) return Vector3.zero;

            var wallCenter = Instance._walls[cellPos.x, cellPos.y].transform.position;
            return (worldPos - wallCenter).normalized;
        }

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
            var rect = new Rect(0, 0, 1, 1);
            foreach (var wall in _walls)
            {
                if (wall == null || !wall.activeSelf) continue;
                if (wall.transform.position.x < rect.xMin) rect.xMin = wall.transform.position.x;
                if (wall.transform.position.x > rect.xMax) rect.xMax = wall.transform.position.x;
                if (wall.transform.position.y < rect.yMin) rect.yMin = wall.transform.position.y;
                if (wall.transform.position.y > rect.yMax) rect.yMax = wall.transform.position.y;
            }
            Gizmos.DrawWireCube(rect.center, rect.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(LevelRect.center, LevelRect.size);
#endif
        }
    }
}