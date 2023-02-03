using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientSpawner : MonoBehaviour
{
    public GameObject BlueNutrientPrefab;
    public GameObject RedNutrientPrefab;
    public GameObject YellowNutrientPrefab;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i < 10;i++)
        {
            Instantiate(BlueNutrientPrefab, new Vector3(Random.Range(-10,10), -Random.Range(1,10), 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
