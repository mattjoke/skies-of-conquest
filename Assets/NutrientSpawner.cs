using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientSpawner : MonoBehaviour
{
    public GameObject BlueNutrientPrefab;
    public GameObject RedNutrientPrefab;
    public GameObject YellowNutrientPrefab;
    public GameObject RockPrefab;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0;i < 10;i++)
        {
            SpawnRandom();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnRandom()
    {
        int ItemType = Random.Range(1, 4);
        GameObject ItemTypeObject = BlueNutrientPrefab;
        if (ItemType == 1)
        {
            ItemTypeObject = BlueNutrientPrefab;
        }
        else if (ItemType == 2)
        {
            ItemTypeObject = RedNutrientPrefab;
        }
        else if (ItemType == 3)
        {
            ItemTypeObject = YellowNutrientPrefab;
        }
        else if (ItemType == 4)
        {
            ItemTypeObject = RockPrefab;
        }
        Instantiate(
            ItemTypeObject,
            new Vector3(Random.Range(-10f, 10f), -Random.Range(1f, 10f), 0),
            Quaternion.identity
        );
    }
}
