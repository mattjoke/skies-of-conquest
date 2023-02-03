using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nutrient : MonoBehaviour
{
    public int NutrientsLeft;
    public int DrainSpeed;
    public bool Unlimited;
    // Start is called before the first frame update
    void Start()
    {
        NutrientsLeft = 100;
        DrainSpeed = 5; 
        Unlimited = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
