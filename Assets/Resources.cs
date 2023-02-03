using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public float Sunlight;
    public float Energy;
    public int Nutrients;
    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        Sunlight = 1;
        Energy = 1;
        Nutrients = 10;
    }
}
