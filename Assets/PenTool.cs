using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenTool : MonoBehaviour
{
    [Header("Dots")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] Transform dotParent;

    [Header("Lines")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] Transform lineParent;
    private LineController currentLine;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(currentLine == null)
            {
                currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineParent).GetComponent<LineController>();
            }

            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject dot = Instantiate(dotPrefab, getMousePosition(), Quaternion.identity);
            Debug.Log(dot.transform.position);
            currentLine.AddPoint(dot.transform);
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject dot = Instantiate(dotPrefab, mouse, Quaternion.identity);
            Debug.Log(currentLine);
            currentLine.AddPoint(dot.transform);
        }
    }

    private Vector3 getMousePosition()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0;
        return mouse;
    }
}
