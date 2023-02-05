using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutrientSpawner : MonoBehaviour
{
    public GameObject BlueNutrientPrefab;
    public GameObject RedNutrientPrefab;
    public GameObject YellowNutrientPrefab;
    public GameObject RockPrefab;
    public Resources ResourceTracker;
    public Camera MapCamera;

    float SpawnerDepth = 1;

    void Update()
    {
        while(SpawnerDepth < -MapCamera.transform.position.y + 50f)
        {
            SpawnRandom();
        }
    }

    void SpawnRandom()
    {
        SpawnerDepth += Random.Range(0.1f, 1.1f);
        int ItemType = Random.Range(1, 5);
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
        GameObject NewDeposit = Instantiate(
            ItemTypeObject,
            new Vector3(Random.Range(-10f, 10f), -SpawnerDepth, 0),
            Quaternion.identity
        );
        if(ItemType != 4)
        {
            ResourceTracker.NutrientDeposits.Add(NewDeposit);
        }
    }
}
