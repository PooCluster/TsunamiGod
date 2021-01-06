using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandTile : MonoBehaviour
{

    public Buildings buildingsPrefab;

    public float resistance { get; private set; }

    public void Init(Vector3 position, int populationLevel)
    {
        transform.position = position;
        resistance = position.z;  // can add height scalar based on function
        Buildings buildings = GameObject.Instantiate(buildingsPrefab, transform) as Buildings;
        buildings.Init(populationLevel);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
