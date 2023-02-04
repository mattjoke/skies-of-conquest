using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Root : MonoBehaviour
{
    public GameObject rootPrefab;

    private int currentLevel = 0;

    private Vector2 currentPos;
    private Vector2 mousePos;

    private String direction = "none";
    private List<RootLevel> stackRoot = new();
    private GameObject ghost = null;
    
    void Start()
    {
        RootLevel firstRoot = new RootLevel(rootPrefab, currentLevel);
        stackRoot.Add(firstRoot);
        currentLevel += 1;
    }

    void Update()
    {
        CreateGhost();
        LeftButtonUp();
    }
    void CreateGhost(){
        if (Input.GetMouseButton(0)){
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Transform middle = stackRoot[currentLevel - 1].GetTransformMiddle();
            if(mousePos.y < middle.position.y && mousePos.y > middle.position.y - middle.lossyScale.y && mousePos.x > middle.position.x && mousePos.x < middle.position.x + middle.lossyScale.x){
                if (ghost == null && direction != "down"){
                    // Create a "ghost" root
                    Destroy(ghost);
                    Vector2 position = middle.position;
                    position.y -= middle.lossyScale.y;
                    ghost = GameObject.Instantiate(rootPrefab, position, Quaternion.identity);
                    Color color = ghost.GetComponent<SpriteRenderer>().material.color;
                    ghost.GetComponent<SpriteRenderer>().material.color = new Color(color.r, color.g, color.b, 0.3f);
                    
                }
                direction = "down";
            }
            
        } 
    }

    
    void LeftButtonUp(){
        if(Input.GetMouseButtonUp(0)){
            Destroy(ghost);
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Transform middle = stackRoot[currentLevel - 1].GetTransformMiddle();;
            Vector2 position = middle.position;
            if(mousePos.y < middle.position.y && mousePos.y > middle.position.y - middle.lossyScale.y && mousePos.x > middle.position.x && mousePos.x < middle.position.x + middle.lossyScale.x){
                switch(direction){
                    case "down":
                        position.y -= middle.lossyScale.y;
                        GameObject newRoot = Instantiate(rootPrefab, position, Quaternion.identity);
                        stackRoot.Add(new RootLevel(newRoot, currentLevel));
                        currentLevel += 1;
                        break;
                    default:
                        Debug.Log("no change");
                        break;
                }
                direction = "none";
                return;
            }
        }    
    }

}

// Or root
class RootLevel 
{
    private int level = 0;
    private GameObject middle = null;

    public RootLevel(GameObject middle, int level){
        this.middle = middle;
        this.level = level;
    }

    public Transform GetTransformMiddle(){
        return this.middle.transform;
    }

    public GameObject GetMiddle(){
        return this.middle;
    }
}

