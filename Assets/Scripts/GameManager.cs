using UnityEngine;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public LevelData currentLevel;
    public Transform[] TilePrefabs;

    [Range(0, 1)]
    public float outlinePercent;

    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        var width = currentLevel.Tiles.GetLength(0);
        var height = currentLevel.Tiles.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var currentTile = currentLevel.Tiles[x, y];
                var currentTilePrefab = TilePrefabs[(int)currentTile];

                Vector3 tilePosition = new Vector3(-width / 2 + 0.5f + x, 0, -height / 2 + 0.5f + y);
                Transform newTile = Instantiate(currentTilePrefab, tilePosition, Quaternion.identity);
                newTile.localScale *= (1 - outlinePercent);
                newTile.SetParent(mapHolder, true);
            }
        }

    }
}