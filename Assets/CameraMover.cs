using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    public Camera MainCamera;
    public Camera MapCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            MoveCamera(1f);
        }
        else if (Input.GetKey("s"))
        {
            MoveCamera(-1f);
        }
    }

    void MoveCamera(float direction)
    {
        MainCamera.transform.position += new Vector3(0, direction, 0);
        MapCamera.transform.position += new Vector3(0, direction, 0);
    }
}
