using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Camera MainCamera;
    public Camera MapCamera;
    public float CameraSpeed;


    Vector3 MouseDragStart;
    Vector3 MousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            MouseDragStart = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(2))
        {
            MousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            MoveCamera(MouseDragStart.y - MousePosition.y);
        }
    }

    void MoveCamera(float direction)
    {
        MainCamera.transform.position += new Vector3(0, direction * CameraSpeed * Time.deltaTime, 0);
        MapCamera.transform.position += new Vector3(0, direction * CameraSpeed * Time.deltaTime, 0);
    }
}
