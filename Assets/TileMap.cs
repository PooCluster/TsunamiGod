using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{

    // prefab
    public GameObject landTilePrefab;   // damage
    public GameObject waterTilePrefab;  // energy
    public int width;
    public int height;
    public float noiseScale;

    private GameObject[,] landTileMap;  // LandTile
    private GameObject[,] waterTileMap;  // WaterTile

    // Start is called before the first frame update
    void Start()
    {
        noiseScale = 0.5;
        if (width > 0 && height > 0)
        {
            landTileMap = new GameObject[width, height];
            waterTileMap = new GameObject[width, height];
            for (float y = 0; y < height; y++)
            {
                for (float x = 0; x < width; x++)
                {
                    float landLevel = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);
                    if (landLevel > 0.5)
                        landTileMap[(int) x, (int) y] = GameObject.Instantiate(landTilePrefab, new Vector3(x, y, landLevel), 0);
                    // else null
                    waterTileMap[(int) x, (int) y] = GameObject.Instantiate(waterTilePrefab, new Vector3(x, y, 0), new Vector2(0, 0));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
