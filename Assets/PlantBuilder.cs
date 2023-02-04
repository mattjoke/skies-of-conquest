using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBuilder : MonoBehaviour
{
    public GameObject RootPrefab;

    Camera MainCamera;
    Vector3 MouseDragStart;
    Vector3 MousePosition;
    bool MouseDragging;
    GameObject CurrentRoot;

    void Start()
    {
        MainCamera = Camera.main;
        CreateRoot(Vector3.zero);
        PlaceRoot(new Vector3(0,-0.3f,0),new Vector3(0,1,0));
    }

    void Update()
    {
        MousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        MousePosition.z = 5;
        if (Input.GetMouseButtonDown(0))
        {
            MouseDragStart = MousePosition;
            CreateRoot(MouseDragStart);
            //CurrentRoot.GetComponent<SpriteRenderer>().color = Color.red;
            MouseDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseDragging = false;
        }
        else if (MouseDragging)
        {
            PlaceRoot(MouseDragStart, MousePosition);
        }
    }

    void CreateRoot(Vector3 Position)
    {
        CurrentRoot = Instantiate(RootPrefab, Position, Quaternion.identity);
    }

    void PlaceRoot(Vector3 StartPosition,Vector3 EndPosition)
    {
        CurrentRoot.transform.position = (EndPosition + StartPosition) / 2f;
        CurrentRoot.transform.rotation = Quaternion.Euler(0, 0, 
            90f + Mathf.Rad2Deg * Mathf.Atan2(EndPosition.y - StartPosition.y, EndPosition.x - StartPosition.x)
        );
        CurrentRoot.transform.localScale = new Vector3(1, (EndPosition - StartPosition).magnitude * 0.8f + 0.25f, 1);
    }
}
