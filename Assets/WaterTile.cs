using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : MonoBehaviour
{

    public float energy { get; set; }  // Vector3, because Vector3.Normalize requires it

    private TileMap map;
    private float gaussianSum, gaussianRange, gaussianWidth;
    private double lastTick;

    public void Init(Vector3 position)
    {
        map = GameObject.FindObjectOfType<TileMap>() as TileMap;
        transform.position = position;
        ChangeEnergy(0f);
        lastTick = Time.fixedTime;
        gaussianRange = 1f;
        gaussianWidth = 10f;
        gaussianSum = GaussianSum(gaussianRange, gaussianWidth);
    }

    public void ChangeEnergy(float energy)
    {
        this.energy = energy;
        transform.position = new Vector3(transform.position.x,
                                         transform.position.y,
                                         energy);  // can determine some funciton here for height
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    float GaussianFilter(int x, int y, float a, float b, float c)
    {
        return a * Mathf.Exp(- (Mathf.Pow(x - b, 2) + Mathf.Pow(y, 2)) / (2 * c * c));
    }

    float GaussianSum(float range, float width)
    {
        float gaussianSum = 0f;
        float offset = 0f;
        for (float yKernel = -map.kernelSize; yKernel <= map.kernelSize; yKernel++)
        {
            for (float xKernel = -map.kernelSize; xKernel <= map.kernelSize; xKernel++)
            {
                if ((transform.position.x + xKernel < -map.width / 2 || transform.position.x + xKernel >= map.width / 2 ||
                     transform.position.y + yKernel < -map.height / 2 || transform.position.y + yKernel >= map.height / 2))
                    continue;
                gaussianSum += GaussianFilter((int) xKernel, (int) yKernel, range, offset, width);
            }
        }
        return gaussianSum;
    }

    float GaussianKernel(int x, int y)
    {
        return GaussianFilter(x, y, gaussianRange, 0, gaussianWidth) / gaussianSum;
    }

    // Update is called once per frame
    void Update()
    {
        // discretize the filtering so we can see the wave effect
        // if (Time.fixedTime - lastTick < 0.2)
        //     return;
        // lastTick = Time.fixedTime;

        // calculate Gaussian as weight factor for height
        // store new energy
        float newEnergy = 0f;
        for (int y = -map.kernelSize; y <= map.kernelSize; y++)
        {
            for (int x = -map.kernelSize; x <= map.kernelSize; x++)
            {
                if ((transform.position.x + x < -map.width / 2 || transform.position.x + x >= map.width / 2 ||
                     transform.position.y + y < -map.height / 2 || transform.position.y + y >= map.height / 2))
                    continue;
                float tileEnergy = map.waterTileMapEnergy[x + (int) transform.position.x + map.width  / 2,
                                                          y + (int) transform.position.y + map.height / 2];
                newEnergy += GaussianKernel(x, y) * tileEnergy;
            }
        }
        ChangeEnergy(energy - (energy - newEnergy) * Time.deltaTime);
        // more realistic water simulation additive attempt:
            // distribute amongst up, down, left, and right
            // let's distribute half of the energy into potential fourths?

    }
}
