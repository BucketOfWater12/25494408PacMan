using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject topHalf;
    [SerializeField] private GameObject bottomHalf;

    [Header("Wall Prefabs")]
    [SerializeField] private GameObject outsideCornerPrefab;
    [SerializeField] private GameObject outsideWallPrefab;
    [SerializeField] private GameObject insideCornerPrefab;
    [SerializeField] private GameObject insideWallPrefab;
    [SerializeField] private GameObject TWallPrefab;
    [SerializeField] private GameObject ghostWallPrefab;

    [Header("Pellet Prefabs")]
    [SerializeField] private GameObject starPelletPrefab;
    [SerializeField] private GameObject powerCanisterPrefab;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;
    private int[,] levelMap =
    {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,4},
        {2,6,4,0,0,4,5,4,0,0,0,4,5,4},
        {2,5,3,4,4,3,5,3,4,4,4,3,5,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,5},
        {2,5,3,4,4,3,5,3,3,5,3,4,4,4},
        {2,5,3,4,4,3,5,4,4,5,3,4,4,3},
        {2,5,5,5,5,5,5,4,4,5,5,5,5,4},
        {1,2,2,2,2,1,5,4,3,4,4,3,0,4},
        {0,0,0,0,0,2,5,4,3,4,4,3,0,3},
        {0,0,0,0,0,2,5,4,4,0,0,0,0,0},
        {0,0,0,0,0,2,5,4,4,0,3,4,4,8},
        {2,2,2,2,2,1,5,3,3,0,4,0,0,0},
        {0,0,0,0,0,0,5,0,0,0,4,0,0,0},
    };


    private int[,] fullMap;

    private void Awake()
    {
        BuildFullMap();
    }

    void BuildFullMap()
    {
        int qRows = levelMap.GetLength(0);
        int qCols = levelMap.GetLength(1);
        int rows = qRows * 2 - 1;
        int cols = qCols * 2;

        fullMap = new int[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int qr = r < qRows ? r : rows - 1 - r;
                int qc = c < qCols ? c : cols - 1 - c;
                fullMap[r, c] = levelMap[qr, qc];
            }
        }
    }

    public int GetTile(Vector2Int cell)
    {
        int row = -cell.y;
        int col = cell.x;

        if (row < 0 || row >= fullMap.GetLength(0) || col < 0 || col >= fullMap.GetLength(1))
            return -1;

        return fullMap[row, col];
    }

    public void SetTile(Vector2Int cell, int value)
    {
        int row = -cell.y;
        int col = cell.x;

        if (row < 0 || row >= fullMap.GetLength(0) || col < 0 || col >= fullMap.GetLength(1))
            return;

        fullMap[row, col] = value;
    }

    public bool IsWalkable(Vector2Int cell, bool allowGhostWall = false)
    {
        int v = GetTile(cell);
        if (v == 0 || v == 5 || v == 6) return true;
        if (v == 8 && allowGhostWall) return true;
        return false;
    }

    public Vector2Int WorldToCell(Vector3 position)
    {
        return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(cell.x, cell.y, 0f);
    }

    void Start()
    {
        Destroy(topHalf);
        Destroy(bottomHalf);

        GenerateLevel();
        PositionCamera();
    }

    void GenerateLevel()
    {
        GenerateQuadrant(false, false); //Top left
        GenerateQuadrant(true, false); // Top right
        GenerateQuadrant(false, true); // Bottom left
        GenerateQuadrant(true, true); // Bottom right
    }

    void PositionCamera()
    {
        int rows = levelMap.GetLength(0);
        int columns = levelMap.GetLength(1);

        float levelWidth = columns * 2;
        float levelHeight = rows * 2 - 1;

        float centreX = (levelWidth -1)/ 2f;
        float centreY = -(levelHeight - 1) / 2f;

        mainCamera.transform.position = new Vector3(centreX, centreY, mainCamera.transform.position.z);

        float verticalSize = levelHeight / 2f;
        float horizontalSize = levelWidth / (2f * mainCamera.aspect);

        mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }

    void GenerateQuadrant(bool flipX, bool flipY)
    {
        int rows = levelMap.GetLength(0);
        int columns = levelMap.GetLength(1);

        int maxRow = flipY ? rows - 2 : rows - 1;

        for (int r = 0; r <= maxRow; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int value = levelMap[r, c];
                GameObject prefab = GetPrefabValue(value);
                if (prefab == null) continue;

                float x = flipX ? (2 * columns - 1 - c) : c;
                float y = flipY ? (r - 2 * (rows - 1)) : -r;

                Instantiate(prefab, new Vector3(x, y, 0f), GetRotation(r, c, value, rows, columns, flipX, flipY), transform);
            }
        }
    }

    GameObject GetPrefabValue(int value)
    {
        switch (value)
        {
            case 1: return outsideCornerPrefab;
            case 2: return outsideWallPrefab;
            case 3: return insideCornerPrefab;
            case 4: return insideWallPrefab;
            case 5: return starPelletPrefab;
            case 6: return powerCanisterPrefab;
            case 7: return TWallPrefab;
            case 8: return ghostWallPrefab;

            default: return null;
        }
    }

    bool IsWallValue(int v) => v == 1 || v == 2 || v == 3 || v == 4 || v == 7 || v == 8;
    bool HasNeighbor(int r, int c, int dr, int dc, int rows, int columns, bool allowPhantom)
    {

        int nr = r + dr, nc = c + dc;
        if (allowPhantom)
        {
            if (dc == 1 && nc == columns) return true;
            if (dr == 1 && nr == rows) return true;
        }
        if (nr < 0 || nr >= rows || nc < 0 || nc >= columns) return false;
        return IsWallValue(levelMap[nr, nc]);
    }

    Quaternion GetRotation(int r, int c, int value, int rows, int columns, bool flipX, bool flipY)
    {
        bool allowPhantom = (value == 7);

        bool up = HasNeighbor(r, c, -1, 0, rows, columns, allowPhantom);
        bool down = HasNeighbor(r, c, 1, 0, rows, columns, allowPhantom);
        bool left = HasNeighbor(r, c, 0, -1, rows, columns, allowPhantom);
        bool right = HasNeighbor(r, c, 0, 1, rows, columns, allowPhantom);

        if (value == 3 && c == columns - 1){
            int realNeighbors = 0;
            if (up) realNeighbors++;
            if (down) realNeighbors++;
            if (left) realNeighbors++;
            if (realNeighbors < 2)
            {
                right = true;
            }
            else if (up && down && left)
            {
                up = false;
            }
        }

        if (value == 3 && up && down && left && right)
        {
            int aboveValue = levelMap[r - 1, c];
            if (aboveValue == 2 || aboveValue == 4 || aboveValue == 8)
            {
                down = false;
                left = false;
            }
            else if (aboveValue == 3)
            {
                up = false;
                left = false;
            }
        }
        if (flipX) { (left, right) = (right, left); }
        if (flipY) { (up, down) = (down, up); }

        switch (value)
        {
            case 2:
            case 4:
            case 8:
                if (up && down) return Quaternion.Euler(0, 0, 90);
                if (left && right) return Quaternion.Euler(0, 0, 0);
                if (left || right) return Quaternion.Euler(0, 0, 0);
                if (up || down) return Quaternion.Euler(0, 0, 90);
                return Quaternion.identity;

            case 1:
            case 3:


                if (right && down) return Quaternion.Euler(0, 0, 180);
                if (down && left) return Quaternion.Euler(0, 0, 90);
                if (up && right) return Quaternion.Euler(0, 0, 270);
                if (left && up) return Quaternion.Euler(0, 0, 0);
                return Quaternion.identity;


            case 7:

                if (left && right && up) return Quaternion.Euler(0, 0, 0);
                if (up && down && right) return Quaternion.Euler(0, 0, 90);
                if (left && right && down) return Quaternion.Euler(0, 0, 180);
                if (up && down && left) return Quaternion.Euler(0, 0, 270);
                return Quaternion.identity;

            default:
                return Quaternion.identity;
        }
    }

    public int CountPellets()
    {
        int count = 0;
        for (int r = 0; r < fullMap.GetLength(0); r++)
        {
            for (int c = 0; c < fullMap.GetLength(1); c++)
            {
                if (fullMap[r, c] == 5 || fullMap[r, c] == 6) count ++;
            }
        }
        return count;
    }
}
