using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Plänt : MonoBehaviour
{

    public GameObject stemPrefab;
    public GameObject leafPrefab;

    private int highestLevel = 0;


    private Vector2 currentPos;
    private Vector2 mousePos;

    private String direction = "none";


    private List<PlantLevel> stack = new();


    private GameObject ghost = null;

    // Start is called before the first frame update
    void Start()
    {
        PlantLevel firstLevel = new PlantLevel(gameObject, highestLevel);
        stack.Add(firstLevel);
        highestLevel += 1;
        firstLevel = new PlantLevel(firstLevel.GetMiddle(), highestLevel);
        Vector2 leftLeaf = gameObject.transform.position;
        leftLeaf.x -= 2;
        leftLeaf.y += gameObject.transform.lossyScale.y;
        firstLevel.AddLeaf(0, Instantiate(leafPrefab, leftLeaf, Quaternion.identity));
        Vector2 rightLeaf = gameObject.transform.position;
        rightLeaf.x += 2;
        rightLeaf.y += gameObject.transform.lossyScale.y;
        firstLevel.AddLeaf(1, Instantiate(leafPrefab, rightLeaf, Quaternion.identity));
        stack.Add(firstLevel);
        highestLevel += 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = "none";
            ghost = null;
        }

        if (Input.GetMouseButton(0))
        {

            // Detect the drag direction up, left, right (and then create new stem, leaft on the left and leaf on the right)
            // Up
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 diff = mousePos - currentPos;
            Debug.Log(diff);
            if (diff.y > currentPos.y && diff.y > Math.Abs(diff.x))
            {
                if (ghost == null || direction != "up")
                {
                    // Create a "ghost" stem 
                    Destroy(ghost);
                    Transform middle = stack[highestLevel - 1].GetTransformMiddle();
                    Vector2 position = middle.position;
                    position.y += middle.lossyScale.y;
                    ghost = GameObject.Instantiate(stemPrefab, position, Quaternion.identity);
                    Color color = ghost.GetComponent<SpriteRenderer>().material.color;
                    ghost.GetComponent<SpriteRenderer>().material.color = new Color(color.r, color.g, color.b, 0.3f);
                }
                direction = "up";
            }
            else if (mousePos.x < (currentPos.x - 2))
            {
                if (ghost == null || direction != "left")
                {
                    Destroy(ghost);
                    Transform middle = stack[highestLevel - 1].GetTransformMiddle();
                    Vector2 position = middle.position;
                    position.x -= 2;
                    position.y = -1;
                    ghost = GameObject.Instantiate(leafPrefab, position, Quaternion.identity);
                    Color color = ghost.GetComponent<SpriteRenderer>().material.color;
                    ghost.GetComponent<SpriteRenderer>().material.color = new Color(color.r, color.g, color.b, 0.3f);
                }
                direction = "left";
            }
            else if ((currentPos.x + 2) < mousePos.x)
            {
                if (ghost == null || direction != "right")
                {
                    Destroy(ghost);
                    Transform middle = stack[highestLevel - 1].GetTransformMiddle();
                    Vector2 position = middle.position;
                    position.x += 2;
                    position.y += middle.lossyScale.y;
                    ghost = GameObject.Instantiate(leafPrefab, position, Quaternion.identity);
                    Color color = ghost.GetComponent<SpriteRenderer>().material.color;
                    ghost.GetComponent<SpriteRenderer>().material.color = new Color(color.r, color.g, color.b, 0.3f);
                }
                direction = "right";
            }
            Debug.Log(direction);
        }
        if (Input.GetMouseButtonUp(0))
        {
            Destroy(ghost);
            Transform middle;
            Vector2 position;
            GameObject newLeaf;
            switch (direction)
            {
                case "up":
                    middle = stack[highestLevel - 1].GetTransformMiddle();
                    position = middle.position;
                    position.y += middle.lossyScale.y;
                    GameObject newStem = Instantiate(stemPrefab, position, Quaternion.identity);
                    stack.Add(new PlantLevel(newStem, highestLevel));
                    highestLevel += 1;
                    break;
                case "left":
                    middle = stack[highestLevel - 1].GetTransformMiddle();
                    position = middle.position;
                    position.x -= 2;
                    position.y += middle.lossyScale.y;
                    newLeaf = Instantiate(leafPrefab, position, Quaternion.identity);
                    stack[highestLevel - 1].AddLeaf(0, newLeaf);
                    break;
                case "right":
                    middle = stack[highestLevel - 1].GetTransformMiddle();
                    position = middle.position;
                    position.x += 2;
                    position.y += middle.lossyScale.y;
                    newLeaf = Instantiate(leafPrefab, position, Quaternion.identity);
                    stack[highestLevel - 1].AddLeaf(1, newLeaf);
                    break;
                default:
                    Debug.Log("no change");
                    break;
            }
        }
    }
}

class PlantLevel
{
    private readonly int level = 0;
    private List<GameObject> leftObjects = new();
    private List<GameObject> rightObjects = new();
    private GameObject middle = null;

    public PlantLevel(GameObject middle, int level)
    {
        this.middle = middle;
        this.level = level;
    }

    // 1 - right, 0- left
    public void AddLeaf(int side, GameObject leaf)
    {
        if (side == 0)
        {
            Vector3 newScale = leaf.transform.localScale;
            newScale.x *= -1;
            leaf.transform.localScale = newScale;

            leftObjects.Add(leaf);
            return;
        }
        rightObjects.Add(leaf);
    }

    public Transform GetTransformMiddle()
    {
        return this.middle.transform;
    }

    public GameObject GetMiddle()
    {
        return this.middle;
    }
}
