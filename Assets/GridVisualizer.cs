using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public SpriteRenderer cellPrefab;

    public Transform container;
    public float  timePresentFindPath = 0.2f;

    public Color emptyColor = Color.white;
    public Color wallColor = Color.gray;
    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color pathColor = Color.blue;

    GridManager gridManager;
    readonly Dictionary<Vector2Int, SpriteRenderer> _lookup = new();

    void Awake()
    {
        gridManager = GetComponent<GridManager>();
        if (cellPrefab == null)
        {
            Debug.LogError("[GridVisualizer] Chưa gán cellPrefab!");
            enabled = false;
            return;
        }

        if (container == null) container = new GameObject("GridCells").transform;

    }
    void OnEnable() => AStarPathfinder.OnNodeVisited += ColorVisited;
    void OnDisable() => AStarPathfinder.OnNodeVisited -= ColorVisited;
    private void Start()
    {
        SpawnCells();
        StartCoroutine(StartShowPath());
    }
    IEnumerator StartShowPath()
    {
        foreach (var pos in AStarPathfinder.LastVisitedNodes)
        {
            ColorVisited(pos);
            yield return new WaitForSeconds(timePresentFindPath);
        }
        PainPathWay();
    }
    void PainPathWay()
    {
        var path = AStarPathfinder.FindPath(
            gridManager.Grid,
            gridManager.GetStart(),
            gridManager.GetGoal());

        if (path != null)
        {
            foreach (var p in path)
            {
                if (_lookup.TryGetValue(p, out var sr))
                    sr.color = pathColor;
            }
        }
    }
    void SpawnCells()
    {
        int[,] grid = gridManager.Grid;
        int w = grid.GetLength(0);
        int h = grid.GetLength(1);

        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
            {
                SpriteRenderer sr = Instantiate(cellPrefab, container);
                sr.transform.position = gridManager.GridToWorld(x, y);
                sr.name = $"Cell_{x}_{y}";

                sr.color = GetColor((CellType)grid[x, y]);

                _lookup[new Vector2Int(x, y)] = sr;
            }
    }

    void ColorVisited(Vector2Int pos)
    {
        string name = $"Cell_{pos.x}_{pos.y}";
        Transform t = container.Find(name);
        if (t != null)
            t.GetComponent<SpriteRenderer>().color = Color.yellow;
    }
    Color GetColor(CellType type)
    {
        return type switch
        {
            CellType.Empty => emptyColor,
            CellType.Wall => wallColor,
            CellType.Start => startColor,
            CellType.Goal => goalColor,
            _ => Color.magenta
        };
    }
}
