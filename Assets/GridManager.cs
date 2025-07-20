using UnityEngine;

public enum CellType { Empty = 0, Wall = 1, Start = 2, Goal = 3 }

public class GridManager : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    [Range(0f, 0.5f)] public float obstaclePercent = 0.2f;
    public bool regenerateUntilPath = true;

    public int[,] Grid { get; private set; }

    private void Awake() => GenerateValidGrid();

    #region Grid generation
    void GenerateValidGrid()
    {
        int safety = 0;
        // gen den bao gio co duong di thi ok ko gen lai
        do
        {
            GenerateRandomGrid();
            safety++;
        } while (regenerateUntilPath &&
                 AStarPathfinder.FindPath(Grid, GetStart(), GetGoal()) == null &&
                 safety < 25);
    }

    void GenerateRandomGrid()
    {
        Grid = new int[width, height];


        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                Grid[x, y] = Random.value < obstaclePercent ? (int)CellType.Wall : (int)CellType.Empty;


        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int goal = new Vector2Int(width - 1, height - 1);
        Grid[start.x, start.y] = (int)CellType.Start;
        Grid[goal.x, goal.y] = (int)CellType.Goal;
    }
    #endregion

    public Vector2Int GetStart()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (Grid[x, y] == (int)CellType.Start) return new Vector2Int(x, y);
        return Vector2Int.zero;
    }

    public Vector2Int GetGoal()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (Grid[x, y] == (int)CellType.Goal) return new Vector2Int(x, y);
        return Vector2Int.zero;
    }

    // Chuyển toạ độ lưới -> vị trí thế giới (mỗi ô = 1 unit)
    public Vector3 GridToWorld(int x, int y) => new Vector3(x + 0.5f, 0, y + 0.5f);
}
