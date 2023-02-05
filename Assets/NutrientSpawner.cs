using System;
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

    float SpawnerDepth = 5;

    void Update()
    {
        while (SpawnerDepth < -MapCamera.transform.position.y + 50f)
        {
            SpawnRandom();
        }
    }

    void SpawnRandom()
    {
        SpawnerDepth += UnityEngine.Random.Range(0.1f, 1.1f);

        // Create the wall of rocks every 30 units
        if (Math.Round(SpawnerDepth) % 30 == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Instantiate(RockPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), -SpawnerDepth, 0), Quaternion.identity);
            }
            return;
        }

        int ItemType = UnityEngine.Random.Range(1, 100);
        GameObject ItemTypeObject = BlueNutrientPrefab;
        switch (ItemType)
        {
            case <= 50:
                ItemTypeObject = RockPrefab;
                break;
            case <= 80:
                ItemTypeObject = BlueNutrientPrefab;
                break;
            case <= 95:
                ItemTypeObject = RedNutrientPrefab;
                break;
            case <= 100:
                ItemTypeObject = YellowNutrientPrefab;
                break;
        }

        GameObject NewDeposit = Instantiate(
            ItemTypeObject,
            new Vector3(UnityEngine.Random.Range(-10f, 10f), -SpawnerDepth, 0),
            Quaternion.identity
        );
        if (ItemType > 50)
        {
            ResourceTracker.NutrientDeposits.Add(NewDeposit);
        }
    }
}
