using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Root : MonoBehaviour
{
    public GameObject root;

    private Vector2 startingPos;
    private Vector2 currentPos;
    private Vector2 mousePos;

    private LineRenderer lineRenderer;
    private List<Vector2> lines = new();
    
    void Start()
    {
        startingPos = root.transform.position;
        currentPos = startingPos;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
    }

    void Update()
    {
        // MouseButtonDown();
        // MouseButton();
        // MouseButtonUp();
        
    }

    void MouseButtonDown(){
        // Redraw all lines       
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(Vector2.Distance(mousePos, startingPos));

            // Check if we need to add new root or sprout a new one from existing line
            if (Vector2.Distance(mousePos, startingPos) < 0.5f)
            {
                // Add new root
                lines.Add(new Vector2(mousePos.x, mousePos.y));
                currentPos = mousePos;
            }
            else
            {
                // Find closest line
                var closestLine = lines.Select((line, index) => new { line, index })
                    .OrderBy(x => Vector2.Distance(x.line, mousePos))
                    .First();

                // Check if we are close enough to the line
                if (Vector2.Distance(mousePos, closestLine.line) < 0.5f)
                {
                    // Add new root
                    lines.Add(new Vector2(mousePos.x, mousePos.y));
                    currentPos = mousePos;
                }
                else
                {
                    // Sprout new root
                    var newRoot = Instantiate(root, mousePos, Quaternion.identity);
                    newRoot.GetComponent<Root>().lines = lines.Skip(closestLine.index).ToList();
                    lines = lines.Take(closestLine.index).ToList();
                    lines.Add(new Vector2(mousePos.x, mousePos.y));
                    currentPos = mousePos;
                }
            }
            
        }
    }

    void MouseButton(){
        if (Input.GetMouseButton(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRenderer.positionCount = lines.Count + 2;
            if (lines.Count == 0)
            {
                lineRenderer.SetPosition(0, new Vector3(startingPos.x, startingPos.y, 0));
                lineRenderer.SetPosition(1, new Vector3(mousePos.x, mousePos.y, 0));
                return;
            }
            lineRenderer.SetPosition(0, new Vector3(startingPos.x, startingPos.y, 0));
            lineRenderer.SetPosition(1, new Vector3(lines[0].x, lines[0].y, 0));
            for (int i = 1; i < lines.Count; i++)
            {
                lineRenderer.SetPosition(i + 1, new Vector3(lines[i].x, lines[i].y, 0));
            }
            lineRenderer.SetPosition(lines.Count + 1, new Vector3(mousePos.x, mousePos.y, 0));
        }
    }

    void MouseButtonUp(){
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("RELEASE");
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lines.Add(new Vector2(mousePos.x, mousePos.y));
            currentPos = mousePos;
        }
    }
}
