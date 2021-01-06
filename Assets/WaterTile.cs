using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : MonoBehaviour
{

    public float energy { get; set; }  // Vector3, because Vector3.Normalize requires it

    private TileMap map;
    private float gaussianSum, gaussianRange, gaussianWidth;

    public void Init(Vector3 position)
    {
        map = GameObject.FindObjectOfType<TileMap>() as TileMap;
        transform.position = position;
        ChangeEnergy(0f);
        // https://upload.wikimedia.org/wikipedia/commons/thumb/7/74/Normal_Distribution_PDF.svg/720px-Normal_Distribution_PDF.svg.png
        gaussianWidth = Mathf.Sqrt(2f);
        gaussianSum = GaussianSum(gaussianWidth);
    }

    public void ChangeEnergy(float energy)
    {
        this.energy = energy;
        if (this.energy < 0f)
            this.energy = 0f;
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         this.energy);  // can determine some funciton here for height
    }

    public void AddEnergy(float energy)
    {
        ChangeEnergy(this.energy + energy);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    float GaussianFilter(int x, int y, float b, float c)
    {
        // https://en.wikipedia.org/wiki/Gaussian_function
        // return a * Mathf.Exp(- (Mathf.Pow(x - b, 2) + Mathf.Pow(y, 2)) / (2 * c * c));
        return (1f / (c * Mathf.Sqrt(2f * Mathf.PI))) * Mathf.Exp(-(Mathf.Pow(x - b, 2) + Mathf.Pow(y, 2)) / (2 * c * c));
    }

    float GaussianSum(float width)
    {
        float gaussianSum = 0f;
        float offset = 0f;
        for (float yKernel = -map.kernelSize; yKernel <= map.kernelSize; yKernel++)
        {
            for (float xKernel = -map.kernelSize; xKernel <= map.kernelSize; xKernel++)
            {
                if ((transform.position.x + xKernel < -map.width  / 2 || transform.position.x + xKernel >= map.width  / 2 ||
                     transform.position.y + yKernel < -map.height / 2 || transform.position.y + yKernel >= map.height / 2))
                    continue;
                gaussianSum += GaussianFilter((int) xKernel, (int) yKernel, offset, width);
            }
        }
        return gaussianSum;
    }

    float GaussianKernel(int x, int y)
    {
        return GaussianFilter(x, y, 0, gaussianWidth) / gaussianSum;
    }

    // Update is called once per frame
    void Update()
    {
        // calculate Gaussian as weight factor for height
        // store new energy
        float newEnergy = 0f;
        for (int y = -map.kernelSize; y <= map.kernelSize; y++)
        {
            for (int x = -map.kernelSize; x <= map.kernelSize; x++)
            {
                if ((transform.position.x + x < -map.width  / 2 || transform.position.x + x >= map.width  / 2 ||
                     transform.position.y + y < -map.height / 2 || transform.position.y + y >= map.height / 2))
                    continue;
                float tileEnergy = map.waterTileMapEnergy[x + (int) transform.position.x + map.width  / 2,
                                                          y + (int) transform.position.y + map.height / 2];
                newEnergy += GaussianKernel(x, y) * tileEnergy;
            }
        }
        AddEnergy(-(energy - newEnergy) * Time.deltaTime);
        // more realistic water simulation additive attempt:
            // distribute amongst up, down, left, and right
            // let's distribute Time.deltaTime worth of energy
            // conditions affecting behavior:
                // if the adjacent water tile is lower (if it's higher, no distribution)
                // if there is a land tile present (apply a resistance based on land tile height)
        // if (true)
        //     return;

        float deltaTimeScale = 0.01f * Time.deltaTime;
        float energyToDistribute = (deltaTimeScale > energy) ? energy : deltaTimeScale;
        float energyPerTile = energyToDistribute / 4;
        for (int i = 0; i < 4; i++)
        {
            // x and y in TileMap array space
            int x = (int) transform.position.x + map.width  / 2;
            int y = (int) transform.position.y + map.height / 2;
            if (i < 2)  // do left right
                x += (i % 2 == 0) ? -1 : 1;
            else        // do up down
                y += (i % 2 == 0) ? 1 : -1;
            if (x < 0 || x >= map.width || y < 0 || y >= map.height ||
                map.waterTileMapEnergy[x, y] > energy)
                continue;
            float energyToTransfer = energyPerTile;
            if (map.landTileMap[x, y] != null)
                energyToTransfer *= TileMap.LAND_TRANSLATE - map.landTileMap[x, y].resistance;
            map.waterTileMap[x, y].AddEnergy(energyToTransfer);
        }
        AddEnergy(-energyToDistribute);
    }
}
