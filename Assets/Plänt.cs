using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plänt : MonoBehaviour
{

    public GameObject stemPrefab;
    public GameObject leafPrefab;

    private int highestLevel = 0;

    private List<PlantLevel> stack = new();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Add a new stem
            Transform middle = stack[highestLevel - 1].GetTransformMiddle();
            Vector2 position = middle.position;
            position.y += middle.lossyScale.y;
            GameObject newStem = Instantiate(stemPrefab, position, Quaternion.identity);
            stack.Add(new PlantLevel(newStem, highestLevel));
            highestLevel += 1;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Add a new leaf to the left
            Transform middle = stack[highestLevel - 1].GetTransformMiddle();
            Vector2 position = middle.position;
            position.x -= 2;
            position.y += middle.lossyScale.y;
            GameObject newLeaf = Instantiate(leafPrefab, position, Quaternion.identity);
            stack[highestLevel - 1].AddLeaf(0, newLeaf);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Add a new leaf to the right
            Transform middle = stack[highestLevel - 1].GetTransformMiddle();
            Vector2 position = middle.position;
            position.x += 2;
            position.y += middle.lossyScale.y;
            GameObject newLeaf = Instantiate(leafPrefab, position, Quaternion.identity);
            stack[highestLevel - 1].AddLeaf(1, newLeaf);
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
