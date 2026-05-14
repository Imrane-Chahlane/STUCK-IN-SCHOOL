using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("=== GRILLE ===")]
    public int gridSize = 6;
    public float cellSize = 2f;

    [Header("=== PREFABS (optionnel) ===")]
    public GameObject normalCellPrefab;
    public GameObject wallCellPrefab;
    public GameObject startCellPrefab;
    public GameObject exitCellPrefab;

    [Header("=== VOLTAGES ===")]
    public int minVoltage = -5;
    public int maxVoltage = 5;

    [Header("=== PARENT ===")]
    public Transform gridParent;

    public CellData[,] grid;

    public enum CellType { Normal, Wall, Start, Exit }

    public class CellData
    {
        public CellType type;
        public int voltage;
        public GameObject cellObject;
        public int row, col;
    }

    public Vector2Int startPos = new Vector2Int(0, 0);
    public Vector2Int exitPos;

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        ClearGrid();

        grid = new CellData[gridSize, gridSize];
        exitPos = new Vector2Int(gridSize - 1, gridSize - 1);

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                CellData cell = new CellData();
                cell.row = row;
                cell.col = col;

                if (row == startPos.x && col == startPos.y)
                {
                    cell.type = CellType.Start;
                    cell.voltage = 0;
                }
                else if (row == exitPos.x && col == exitPos.y)
                {
                    cell.type = CellType.Exit;
                    cell.voltage = 0;
                }
                else
                {
                    bool isWall = Random.value < 0.25f;
                    cell.type = isWall ? CellType.Wall : CellType.Normal;
                    cell.voltage = isWall ? 0 : Random.Range(minVoltage, maxVoltage + 1);
                    if (!isWall && cell.voltage == 0) cell.voltage = 1;
                }

                grid[row, col] = cell;
                SpawnCell(cell);
            }
        }

        EnsurePathExists();
        Debug.Log("[GridManager] Grille generee !");
    }

    private void SpawnCell(CellData cell)
    {
        Vector3 pos = new Vector3(
            transform.position.x + cell.col * cellSize,
            transform.position.y,
            transform.position.z + cell.row * cellSize
        );

        GameObject obj = null;

        if (cell.type == CellType.Normal && normalCellPrefab != null)
            obj = Instantiate(normalCellPrefab, pos, Quaternion.identity);
        else if (cell.type == CellType.Wall && wallCellPrefab != null)
            obj = Instantiate(wallCellPrefab, pos, Quaternion.identity);
        else if (cell.type == CellType.Start && startCellPrefab != null)
            obj = Instantiate(startCellPrefab, pos, Quaternion.identity);
        else if (cell.type == CellType.Exit && exitCellPrefab != null)
            obj = Instantiate(exitCellPrefab, pos, Quaternion.identity);
        else
        {
            // Pas de prefab → cube basique coloré
            obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(cellSize - 0.1f, 0.1f, cellSize - 0.1f);

            MeshRenderer mr = obj.GetComponent<MeshRenderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            switch (cell.type)
            {
                case CellType.Normal: mat.color = Color.white;  break;
                case CellType.Wall:   mat.color = Color.black;  break;
                case CellType.Start:  mat.color = Color.green;  break;
                case CellType.Exit:   mat.color = Color.blue;   break;
            }

            mr.material = mat;
        }

        if (gridParent != null)
            obj.transform.SetParent(gridParent);

        obj.name = "Cell_" + cell.row + "_" + cell.col;
        cell.cellObject = obj;
    }

    private void EnsurePathExists()
    {
        List<Vector2Int> path = FindPath(startPos, exitPos);

        if (path == null || path.Count == 0)
        {
            Debug.Log("[GridManager] Aucun chemin, creation chemin de secours...");

            for (int col = 0; col < gridSize; col++)
            {
                if (grid[0, col].type == CellType.Wall)
                {
                    grid[0, col].type = CellType.Normal;
                    UpdateCellColor(grid[0, col], Color.white);
                }
            }
            for (int row = 0; row < gridSize; row++)
            {
                if (grid[row, gridSize - 1].type == CellType.Wall)
                {
                    grid[row, gridSize - 1].type = CellType.Normal;
                    UpdateCellColor(grid[row, gridSize - 1], Color.white);
                }
            }
        }
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == end)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                while (current != start)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Reverse();
                return path;
            }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (IsInBounds(next) && !cameFrom.ContainsKey(next) &&
                    grid[next.x, next.y].type != CellType.Wall)
                {
                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        return null;
    }

    private void UpdateCellColor(CellData cell, Color color)
    {
        if (cell.cellObject == null) return;
        MeshRenderer mr = cell.cellObject.GetComponent<MeshRenderer>();
        if (mr != null) mr.material.color = color;
    }

    private void ClearGrid()
    {
        if (grid == null) return;
        foreach (CellData cell in grid)
        {
            if (cell?.cellObject != null)
                Destroy(cell.cellObject);
        }
    }

    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize &&
               pos.y >= 0 && pos.y < gridSize;
    }

    public CellData GetCell(int row, int col)
    {
        if (row < 0 || row >= gridSize || col < 0 || col >= gridSize) return null;
        return grid[row, col];
    }
}
