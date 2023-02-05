using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RightOpponent : MonoBehaviour
{
    public float ActionCountdown = 0;
    public PlantBuilder PlantBuilder;
    public GameObject StemPrefab;
    public GameObject RootPrefab;
    public Resources ResourceTracker;

    public float Height = 10;
    public float Depth = -10;

    GameObject CurrentStem;

    void Update()
    {
        if (ResourceTracker.RightOpponentNutrients >= 10)
        {
            int Action = Random.Range(1, 5);
            if(Action == 1 & Random.Range(1,5) == 1)
            {
                CurrentStem = Instantiate(StemPrefab, new Vector3(15, Height, 0), Quaternion.Euler(0,0,Random.Range(45f,90f)));
                CurrentStem.GetComponent<EvilStemRight>().PlantBuilder = PlantBuilder;
            }
            else if (Action == 2)
            {
                Height += Random.Range(2, 5);
            }
            else if (Action == 3)
            {
                Depth -= Random.Range(2, 5);
            }
            else if (Action == 4 & Random.Range(1, 5) == 1)
            {
                CurrentStem = Instantiate(RootPrefab, new Vector3(15, Depth, 0), Quaternion.Euler(0, 0, Random.Range(90f, 135f)));
                CurrentStem.GetComponent<EvilStemRight>().PlantBuilder = PlantBuilder;
                CurrentStem.GetComponent<EvilStemRight>().Root = true;
            }
            ResourceTracker.RightOpponentNutrients -= 10;
        }
        else
        {
            ActionCountdown -= Time.deltaTime;
            if(ActionCountdown <= 0)
            {
                ResourceTracker.RightOpponentNutrients += Random.Range(5, 10);
                ActionCountdown = Random.Range(1f, 5f);
            }
        }
    }
}
