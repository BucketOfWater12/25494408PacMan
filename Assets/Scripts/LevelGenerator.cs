using System;
using UnityEditorInternal;
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

    void Start()
    {
        Destroy(topHalf);
        Destroy(bottomHalf);

        GenerateLevel();
    }

    void GenerateLevel()
    {
        GenerateQuadrant(false, false); //Top left
        GenerateQuadrant(true, false); // Top right
        GenerateQuadrant(false, true); // Bottom left
        GenerateQuadrant(true, true); // Bottom right
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

                Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity, transform);
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
}
