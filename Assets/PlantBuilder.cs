using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBuilder : MonoBehaviour
{
    [SerializeField] private LayerMask RockMask;

    public GameObject RootPrefab;

    Camera MainCamera;
    Vector3 MouseDragStart;
    Vector3 MousePosition;
    bool MouseDragging;
    bool RootVerified;
    GameObject CurrentRoot;

    void Start()
    {
        RockMask |= (1 << LayerMask.NameToLayer("Rock"));

        MainCamera = Camera.main;

        CreateRoot(Vector3.zero);
        PlaceRoot(new Vector3(0,-0.3f,0),new Vector3(0,1,0));
    }

    void Update()
    {
        MousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        MousePosition.z = 20;
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
            if(!RootVerified)
            {
                Destroy(CurrentRoot);
            }
        }
        else if (MouseDragging)
        {
            PlaceRoot(MouseDragStart, MousePosition);
            VerifyRoot();
        }
    }

    void VerifyRoot()
    {
        Collider2D[] RockCollision = Physics2D.OverlapBoxAll(
            new Vector2(CurrentRoot.transform.position.x,CurrentRoot.transform.position.y),
            new Vector2(CurrentRoot.transform.localScale.x,CurrentRoot.transform.localScale.y),
            CurrentRoot.transform.rotation.z,
            RockMask
        );
        if(RockCollision.Length == 0)
        {
            RootVerified = true;
        }
        else
        {
            RootVerified = false;
            for(int i = 0;i < RockCollision.Length;i++)
            {
                if(Random.Range(0,2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        if(RootVerified)
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = Color.red;
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
