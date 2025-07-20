using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    public static System.Action<Vector2Int> OnNodeVisited;               
    public static IReadOnlyList<Vector2Int> LastVisitedNodes => _last;    

    public static List<Vector2Int> FindPath(
        int[,] grid, Vector2Int start, Vector2Int goal)
    {
   
        _last.Clear();

        int w = grid.GetLength(0);
        int h = grid.GetLength(1);

        var open = new List<Node>();
        var closed = new HashSet<Vector2Int>();

        Node startNode = new Node(start) { gCost = 0, hCost = Heuristic(start, goal) };
        open.Add(startNode);

        Dictionary<Vector2Int, Node> nodeLookup = new() { [start] = startNode };

        while (open.Count > 0)
        {
            open.Sort((a, b) => a.fCost.CompareTo(b.fCost));
            Node current = open[0];
            open.RemoveAt(0);

            if (current.Pos == goal)
                return ReconstructPath(current);

            closed.Add(current.Pos);

            foreach (var offset in Neighbours4)
            {
                Vector2Int neighPos = current.Pos + offset;
                if (neighPos.x < 0 || neighPos.x >= w || neighPos.y < 0 || neighPos.y >= h)
                    continue;
                if (grid[neighPos.x, neighPos.y] == (int)CellType.Wall ||
                    closed.Contains(neighPos))
                    continue;

                int tentativeG = current.gCost + 1;

                if (!nodeLookup.TryGetValue(neighPos, out Node neighNode))
                {
                    neighNode = new Node(neighPos)
                    {
                        gCost = tentativeG,
                        hCost = Heuristic(neighPos, goal),
                        parent = current
                    };
                    nodeLookup[neighPos] = neighNode;
                    open.Add(neighNode);

                   
                    _last.Add(neighPos);
                    OnNodeVisited?.Invoke(neighPos);
                }
                else if (tentativeG >= neighNode.gCost) continue;

                neighNode.parent = current;
                neighNode.gCost = tentativeG;
                neighNode.hCost = Heuristic(neighPos, goal);
            }
        }
        return null;
    }

    
    private class Node
    {
        public Vector2Int Pos;
        public int gCost, hCost;
        public int fCost => gCost + hCost;
        public Node parent;
        public Node(Vector2Int pos) => Pos = pos;
    }

    private static readonly Vector2Int[] Neighbours4 = {
        new Vector2Int( 1, 0), new Vector2Int(-1, 0),
        new Vector2Int( 0, 1), new Vector2Int( 0,-1)
    };

    private static readonly List<Vector2Int> _last = new();               

    static int Heuristic(Vector2Int a, Vector2Int b) =>
        Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

    static List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> path = new();
        while (node != null) { path.Add(node.Pos); node = node.parent; }
        path.Reverse();
        return path;
    }
}
