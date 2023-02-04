using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Camera MainCamera;
    public Camera MapCamera;
    public float CameraSpeed;

    void Update()
    {
        if (Input.GetKey("w") || Input.GetKey("up"))
        {
            MoveCamera(1f);
        }
        else if (Input.GetKey("s") || Input.GetKey("down"))
        {
            MoveCamera(-1f);
        }
    }

    void MoveCamera(float direction)
    {
        MainCamera.transform.position += new Vector3(0, direction * CameraSpeed * Time.deltaTime, 0);
        MapCamera.transform.position += new Vector3(0, direction * CameraSpeed * Time.deltaTime, 0);
    }
}
