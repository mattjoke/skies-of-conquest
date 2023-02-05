using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class EvilStemRight : MonoBehaviour
{
    bool Active = true;
    public bool Root = false;
    public GameObject LeafPrefab;
    public GameObject ShadePrefab;
    public PlantBuilder PlantBuilder;

    public AudioSource LeafSource;
    public AudioClip LeafClip;

    GameObject CurrentLeaf;
    GameObject CurrentShade;

    public float TargetPoint;

    private void Start()
    {
        TargetPoint = Random.Range(0f, 10f) + 10f;
    }

    void Update()
    {
        if(Active)
        {
            transform.position = new Vector3(transform.position.x - Time.deltaTime, transform.position.y, transform.position.z);
            if(transform.GetChild(1).transform.position.x < TargetPoint)
            {
                if(!Root)
                {
                    CreateLeaf(transform.GetChild(0).transform.position);
                    PlaceLeaf(transform.GetChild(0).transform.position);
                    FinishLeaf();
                    LeafSource.PlayOneShot(LeafClip);
                }
                Active = false;
            }
        }
    }

    void CreateLeaf(Vector3 Position)
    {
        CurrentLeaf = Instantiate(LeafPrefab, Position, Quaternion.identity);
        CurrentShade = Instantiate(ShadePrefab, Position, Quaternion.identity);
        CurrentLeaf.GetComponent<Leaf>().Shade = CurrentShade;
    }

    void PlaceLeaf(Vector3 position)
    {
        CurrentLeaf.transform.position = new Vector3(position.x - CurrentLeaf.transform.localScale.x * 1.8f, position.y);
        CurrentShade.transform.position = new Vector3(CurrentLeaf.transform.position.x, CurrentLeaf.transform.position.y / 2, position.y);
        CurrentShade.transform.localScale = new Vector3(12, CurrentLeaf.transform.position.y * 6.637f, 1);
        CurrentLeaf.GetComponent<Leaf>().LeftEdgeStart = CurrentLeaf.transform.position.x - 1.65f;
        CurrentLeaf.GetComponent<Leaf>().RightEdgeStart = CurrentLeaf.transform.position.x + 1.65f;
    }
    void FinishLeaf()
    {
        CurrentLeaf.GetComponent<Leaf>().LeftEdge = CurrentLeaf.GetComponent<Leaf>().LeftEdgeStart;
        CurrentLeaf.GetComponent<Leaf>().RightEdge = CurrentLeaf.GetComponent<Leaf>().RightEdgeStart;
        PlantBuilder.AllLeaves.Add(CurrentLeaf);
    }
}
