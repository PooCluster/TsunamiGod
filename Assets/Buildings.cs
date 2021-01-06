using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public int populationLevel;

    private GameObject smallBuilding;
    private GameObject mediumBuilding;
    private GameObject bigBuilding;

    // Start is called before the first frame update
    void Start()
    {
        // smallBuilding = GameObject.Instantiate(transform.GetChild(0).gameObject);
        // mediumBuilding = GameObject.Instantiate(transform.GetChild(1).gameObject);
        // bigBuilding = GameObject.Instantiate(transform.GetChild(2).gameObject);
    }

    public void Init(int populationLevel)
    {
        this.populationLevel = populationLevel;
        switch (populationLevel)
        {
            case 0:  // no population
                Destroy(transform.GetChild(2).gameObject);
                Destroy(transform.GetChild(1).gameObject);
                Destroy(transform.GetChild(0).gameObject);
                break;
            case 1:  // add small building
                Destroy(transform.GetChild(2).gameObject);
                Destroy(transform.GetChild(1).gameObject);
                // Destroy(mediumBuilding);
                // Destroy(bigBuilding);
                break;
            case 2:  // add medium building as well
                Destroy(transform.GetChild(2).gameObject);
                // Destroy(bigBuilding);
                break;
            default: // all buildings (populationLevel >= 3)
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
