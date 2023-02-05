using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public Vector3 start;
    private Vector3 mouseCurrent;
    private Vector3 end;

    private Vector3 cameraOffset = new(0, 0, 10);

    private LineRenderer lineRenderer;
    public Material lineMaterial;

    public RectTransform parentRectTransform;
    private RectTransform rt;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && checkBounds())
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            lineRenderer.enabled = true;
            lineRenderer.material = lineMaterial;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, start);
            lineRenderer.useWorldSpace = true;
        }
        if (Input.GetMouseButton(0) && lineRenderer != null)
        {
            mouseCurrent = Camera.main.ScreenToWorldPoint(Input.mousePosition) + cameraOffset;
            lineRenderer.SetPosition(1, mouseCurrent);
        }
        if (Input.GetMouseButtonUp(0) && lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

    }

    private bool checkBounds()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos.x > parentRectTransform.rect.xMin &&
        mousePos.x < parentRectTransform.rect.xMax &&
        mousePos.y > parentRectTransform.rect.yMin &&
        mousePos.y < parentRectTransform.rect.yMax;
    }
}
