using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandTile : MonoBehaviour
{

    public float resistance { get; private set; }

    public void Init(Vector3 position)
    {
        transform.position = position;
        resistance = position.z;  // can add height scalar based on function
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
