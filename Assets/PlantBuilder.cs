using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlantBuilder : MonoBehaviour
{
    [SerializeField] private LayerMask RockMask;
    [SerializeField] private LayerMask PlantMask;
    [SerializeField] private LayerMask LeafMask;
    [SerializeField] private LayerMask NutrientMask;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private LayerMask SkyMask;
    ContactFilter2D RockFilter;
    ContactFilter2D PlantFilter;
    ContactFilter2D LeafFilter;
    ContactFilter2D NutrientFilter;
    ContactFilter2D GroundFilter;
    ContactFilter2D SkyFilter;
    Collider2D[] RockCollision;

    public Resources ResourceTracker;
    public GameObject RootPrefab;
    public GameObject LeafPrefab;
    public GameObject ShadePrefab;
    public GameObject NumberDisplay;
    public GameObject Ground;
    public GameObject Sky;

    public AudioSource RootSource;
    public AudioSource StemSource;
    public AudioClip RootClip;
    public AudioClip StemClip;

    GameObject CostDisplay;

    Camera MainCamera;
    Vector3 MouseDragStart;
    Vector3 MousePosition;

    bool RootVerified = true;
    GameObject CurrentRoot;
    bool MouseDragging;

    GameObject CurrentLeaf;
    bool LeafVerified = true;
    bool MouseDraggingLeaf;
    GameObject CurrentShade;

    bool RootIsUnderground = false;

    void Start()
    {
        RockMask |= (1 << LayerMask.NameToLayer("Rock"));
        PlantMask |= (1 << LayerMask.NameToLayer("Plant"));
        LeafMask |= (1 << LayerMask.NameToLayer("Leaf"));
        NutrientMask |= (1 << LayerMask.NameToLayer("Nutrient"));
        GroundMask |= (1 << LayerMask.NameToLayer("Ground"));
        SkyMask |= (1 << LayerMask.NameToLayer("Sky"));
        GroundFilter = new ContactFilter2D();
        GroundFilter.SetLayerMask(GroundMask);
        RockFilter = new ContactFilter2D();
        RockFilter.SetLayerMask(RockMask);
        PlantFilter = new ContactFilter2D();
        PlantFilter.SetLayerMask(PlantMask);
        LeafFilter = new ContactFilter2D();
        LeafFilter.SetLayerMask(LeafMask);
        NutrientFilter = new ContactFilter2D();
        NutrientFilter.SetLayerMask(NutrientMask);
        SkyFilter = new ContactFilter2D();
        SkyFilter.SetLayerMask(SkyMask);

        RockCollision = new Collider2D[10];

        MainCamera = Camera.main;

        CostDisplay = Instantiate(NumberDisplay, Vector3.zero, Quaternion.identity);
        HideBuildCost();

        CreateRoot(Vector3.zero);
        PlaceRoot(new Vector3(0, -0.3f, 0), new Vector3(0, 1.3f, 0));
        CreateLeaf(Vector3.zero);
        PlaceLeaf(new Vector3(0, 1f, 0));
    }

    void Update()
    {
        MousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        MousePosition.z = 20;
        if (Input.GetMouseButtonDown(0))
        {
            MouseDragStart = MousePosition;
            if (MousePosition.y < 0)
            {
                RootIsUnderground = true;
            }
            else
            {
                RootIsUnderground = false;
            }
            
            CreateRoot(MouseDragStart);
            MouseDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseDragging = false;
            VerifyRoot(true);
            if (RootVerified)
            {
                ResourceTracker.Nutrients -= (int)Mathf.Round(CurrentRoot.transform.localScale.y);
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
                RootSource.PlayOneShot(RootClip);
            }
            else
            {
                Destroy(CurrentRoot);
            }
            HideBuildCost();
            RootIsUnderground = false;
        }
        else if (MouseDragging)
        {
            PlaceRoot(MouseDragStart, MousePosition);
            
            DisplayBuildCost((int)Mathf.Round(CurrentRoot.transform.localScale.y), MousePosition);
            VerifyRoot(false);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            CreateLeaf(MousePosition);
            MouseDraggingLeaf = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            MouseDraggingLeaf = false;
            VerifyLeaf();
            if (!LeafVerified)
            {
                Destroy(CurrentLeaf);
                Destroy(CurrentShade);
                return;
            }
            ResourceTracker.Nutrients -= (int)Mathf.Round(CurrentLeaf.transform.localScale.y * 50);
            int nContacts = CurrentLeaf.GetComponent<BoxCollider2D>().OverlapCollider(
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
            HideBuildCost();
        }
        else if (MouseDraggingLeaf)
        {
            PlaceLeaf(MousePosition);
            DisplayBuildCost((int)Mathf.Round(CurrentLeaf.transform.localScale.y * 50), MousePosition);
            VerifyLeaf();
        }
    }

    void VerifyRoot(bool ReportReason)
    {
        RootVerified = true;
        if (ReportReason) Debug.Log("Verifying");
        if (CurrentRoot.transform.localScale.y < 2) RootVerified = false;
        if (CurrentLeaf.transform.position.y < 0) LeafVerified = false;
        if (Mathf.Round(CurrentRoot.transform.localScale.y) > ResourceTracker.Nutrients) RootVerified = false;

        int nContacts;
        // Check if root is in ground
        if (RootIsUnderground)
        {
            nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
              SkyFilter,
               RockCollision
            );
            if (nContacts != 0)
            {
                RootVerified = false;
                if (ReportReason) Debug.Log("Root in Sky");
            }
        }
        else
        {
            nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
              GroundFilter,
              RockCollision
            );
            if (nContacts != 0)
            {
                RootVerified = false;
                if (ReportReason) Debug.Log("Root in gound");
            }
        }

        //int nContacts = Physics2D.OverlapPointNonAlloc(new Vector2(MousePosition.x, MousePosition.y),RockCollision,PlantMask);
        nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
            PlantFilter,
            RockCollision
        );
        if (nContacts != 1)
        {
            RootVerified = false;
            if (ReportReason)
            {
                Debug.Log("Too many or few plants");
                for (int i = 0; i < nContacts; i++)
                {
                    Debug.Log(RockCollision[i].GetComponent<SpriteRenderer>().gameObject.name);
                }
            }
        }
        nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
             RockFilter,
             RockCollision
         );
        if (nContacts != 0)
        {
            RootVerified = false;
            if (ReportReason) Debug.Log("Rock");
            /*for (int i = 0; i < nContacts; i++)
            {
                if (Random.Range(0, 2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
            }*/
        }

        nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
            LeafFilter,
            RockCollision
        );
        if (nContacts != 0)
        {
            RootVerified = false;
            if (ReportReason)
            {
                Debug.Log("Leaf");
                for (int i = 0; i < nContacts; i++)
                {
                    if (Random.Range(0, 2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                    else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
        ShowVerified();
    }

    void VerifyLeaf()
    {
        LeafVerified = true;
        if (ResourceTracker.Nutrients < CurrentLeaf.transform.localScale.y * 50) LeafVerified = false;
        if (CurrentLeaf.transform.position.y < 0) LeafVerified = false;
        var nContacts = CurrentLeaf.GetComponent<BoxCollider2D>().OverlapCollider(
           RockFilter,
           RockCollision);
        if (nContacts != 0)
        {
            LeafVerified = false;
        }
        nContacts = CurrentLeaf.GetComponent<BoxCollider2D>().OverlapCollider(
           PlantFilter,
           RockCollision
        );
        if (nContacts != 1)
        {
            LeafVerified = false;
        }
        nContacts = CurrentLeaf.GetComponent<BoxCollider2D>().OverlapCollider(
          LeafFilter,
          RockCollision
        );
        if (nContacts != 0)
        {
            LeafVerified = false;
        }
        ShowVerified();
    }

    void CreateRoot(Vector3 Position)
    {
        CurrentRoot = Instantiate(RootPrefab, Position, Quaternion.identity);
    }
    void CreateLeaf(Vector3 Position)
    {
        CurrentLeaf = Instantiate(LeafPrefab, Position, Quaternion.identity);
        CurrentShade = Instantiate(ShadePrefab, Position, Quaternion.identity);
    }

    void PlaceLeaf(Vector3 position)
    {
        CurrentLeaf.transform.position = new Vector3(position.x + CurrentLeaf.transform.localScale.x * 1.8f, position.y);
        CurrentShade.transform.position = new Vector3(CurrentLeaf.transform.position.x, CurrentLeaf.transform.position.y / 2, position.y);
        CurrentShade.transform.localScale = new Vector3(12, CurrentLeaf.transform.position.y * 6.637f, 1);
    }

    void PlaceRoot(Vector3 StartPosition, Vector3 EndPosition)
    {
        CurrentRoot.transform.position = (EndPosition + StartPosition) / 2f;
        CurrentRoot.transform.rotation = Quaternion.Euler(0, 0,
            90f + Mathf.Rad2Deg * Mathf.Atan2(EndPosition.y - StartPosition.y, EndPosition.x - StartPosition.x)
        );
        CurrentRoot.transform.localScale = new Vector3(1, (EndPosition - StartPosition).magnitude * 0.8f + 0.25f, 1);
    }

    void ShowVerified()
    {
        if (CurrentRoot == null) return;
        if (RootVerified)
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
        if (CurrentLeaf == null) return;
        if (LeafVerified)
        {
            CurrentLeaf.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            CurrentLeaf.GetComponent<SpriteRenderer>().color = Color.red;
        }
        
        if (RootVerified && RootIsUnderground)
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = new Color(0.650f, 0.313f, 0f);
        }
    }

    void DisplayBuildCost(int amount, Vector3 position)
    {
        CostDisplay.transform.position = position;
        CostDisplay.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = "-" + amount.ToString();
    }
    void HideBuildCost()
    {
        CostDisplay.transform.position = new Vector3(-100, -100, 0);
    }
}
