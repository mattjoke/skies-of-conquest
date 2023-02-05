using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantBuilder : MonoBehaviour
{
    [SerializeField] private LayerMask RockMask;
    [SerializeField] private LayerMask PlantMask;
    [SerializeField] private LayerMask NutrientMask;
    ContactFilter2D RockFilter;
    ContactFilter2D PlantFilter;
    ContactFilter2D NutrientFilter;
    Collider2D[] RockCollision;

    public GameObject RootPrefab;

    Camera MainCamera;
    Vector3 MouseDragStart;
    Vector3 MousePosition;
    bool MouseDragging;
    bool RootVerified = true;
    GameObject CurrentRoot;

    void Start()
    {
        RockMask |= (1 << LayerMask.NameToLayer("Rock"));
        PlantMask |= (1 << LayerMask.NameToLayer("Plant"));
        NutrientMask |= (1 << LayerMask.NameToLayer("Nutrient"));
        RockFilter = new ContactFilter2D();
        RockFilter.SetLayerMask(RockMask);
        PlantFilter = new ContactFilter2D();
        PlantFilter.SetLayerMask(PlantMask);
        NutrientFilter = new ContactFilter2D();
        NutrientFilter.SetLayerMask(NutrientMask);
        RockCollision = new Collider2D[10];

        MainCamera = Camera.main;

        CreateRoot(Vector3.zero);
        PlaceRoot(new Vector3(0, -0.3f, 0), new Vector3(0, 1.3f, 0));
    }

    void Update()
    {
        MousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        MousePosition.z = 20;
        if (Input.GetMouseButtonDown(0))
        {
            MouseDragStart = MousePosition;
            CreateRoot(MouseDragStart);
            MouseDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseDragging = false;
            VerifyRoot();
            if (RootVerified)
            {
                int nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
                     NutrientFilter,
                     RockCollision
                 );
                if (nContacts != 0)
                {
                    for (int i = 0; i < nContacts; i++)
                    {
                        RockCollision[i].GetComponent<Nutrient>().PlayerTaps++;
                        RockCollision[i].GetComponent<Nutrient>().IsTapped = true;
                    }
                }
            }
            else
            {
                Destroy(CurrentRoot);
                return;
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
        RootVerified = true;
        if (CurrentRoot.transform.localScale.y < 2) RootVerified = false;
        //int nContacts = Physics2D.OverlapPointNonAlloc(new Vector2(MousePosition.x, MousePosition.y),RockCollision,PlantMask);
        int nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
            PlantFilter,
            RockCollision
        );
        if (nContacts != 1)
        {
            RootVerified = false;
        }
        nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
             RockFilter,
             RockCollision
         );
        if (nContacts != 0)
        {
            RootVerified = false;
            /*for (int i = 0; i < nContacts; i++)
            {
                if (Random.Range(0, 2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
            }*/
        }
        ShowVerified();
    }

    void ShowVerified()
    {
        if (RootVerified)
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

    void PlaceRoot(Vector3 StartPosition, Vector3 EndPosition)
    {
        CurrentRoot.transform.position = (EndPosition + StartPosition) / 2f;
        CurrentRoot.transform.rotation = Quaternion.Euler(0, 0,
            90f + Mathf.Rad2Deg * Mathf.Atan2(EndPosition.y - StartPosition.y, EndPosition.x - StartPosition.x)
        );
        CurrentRoot.transform.localScale = new Vector3(1, (EndPosition - StartPosition).magnitude * 0.8f + 0.25f, 1);
    }
}
