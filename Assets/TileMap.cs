using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{

    public const float LAND_TRANSLATE = 0.49f;

    // prefab
    public LandTile landTilePrefab;   // damage
    public WaterTile waterTilePrefab;  // energy
    public int width;
    public int height;
    public int kernelSize;  // defined as number of pixels from the center pixel
    public float noiseScale;

    public LandTile[,] landTileMap { get; private set; } // LandTile
    public WaterTile[,] waterTileMap { get; private set; }  // WaterTile
    public float[,] waterTileMapEnergy { get; private set; } // stores copy of waterTileMap positions


    // Start is called before the first frame update
    void Start()
    {
        if (width > 0 && height > 0)
        {
            landTileMap = new LandTile[width, height];
            waterTileMap = new WaterTile[width, height];
            waterTileMapEnergy = new float[width, height];
            for (float y = 0; y < height; y++)
            {
                for (float x = 0; x < width; x++)
                {
                    
                    // land tile stuff
                    float landLevel = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);  // 0 - 1
                    if (landLevel > 0.5f)
                    {
                        landTileMap[(int) x, (int) y] = Instantiate(landTilePrefab) as LandTile;
                        Vector3 landPosition = new Vector3(x - width / 2, y - height / 2, landLevel - LAND_TRANSLATE);
                        landTileMap[(int) x, (int) y].GetComponent<LandTile>().Init(landPosition);
                    }
                    // else null
                    // water tile stuff
                    waterTileMap[(int) x, (int) y] = Instantiate(waterTilePrefab) as WaterTile;
                    Vector3 waterPosition = new Vector3(x - width / 2, y - height / 2, 0);
                    waterTileMap[(int) x, (int) y].GetComponent<WaterTile>().Init(waterPosition);
                }
            }
            CopyWaterTileEnergies();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CopyWaterTileEnergies();
        if (Input.GetKey(KeyCode.W))
        {
            // Let's hardcode a point first just to test the wave motion.
            for (int y = -8; y < -6; y++)
            {
                for (int x = -8; x < -6; x++)
                {
                    waterTileMap[width / 2 + x, height / 2 + y].GetComponent<WaterTile>().
                        ChangeEnergy(waterTileMap[width / 2 + x, height / 2 + y].
                            GetComponent<WaterTile>().energy + Time.deltaTime);
                }
            }
        }
    }

    private void CopyWaterTileEnergies()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                waterTileMapEnergy[x, y] = waterTileMap[x, y].GetComponent<WaterTile>().energy;
            }
        }
    }
}
