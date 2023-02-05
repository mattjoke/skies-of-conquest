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
    [SerializeField] private LayerMask BranchMask;
    ContactFilter2D RockFilter;
    ContactFilter2D PlantFilter;
    ContactFilter2D LeafFilter;
    ContactFilter2D NutrientFilter;
    ContactFilter2D GroundFilter;
    ContactFilter2D SkyFilter;
    ContactFilter2D BranchFilter;
    Collider2D[] RockCollision;

    public Resources ResourceTracker;
    public GameObject RootPrefab;
    public GameObject RootItemPrefab;
    public GameObject RootItemThickPrefab;
    public GameObject LeafPrefab;
    public GameObject ShadePrefab;
    public GameObject NumberDisplay;
    public GameObject Ground;
    public GameObject Sky;

    public AudioSource RootSource;
    public AudioSource StemSource;
    public AudioClip RootClip;
    public AudioClip StemClip;

    public List<GameObject> AllLeaves;

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

    GameObject endpoint;

    void Start()
    {
        RockMask |= (1 << LayerMask.NameToLayer("Rock"));
        PlantMask |= (1 << LayerMask.NameToLayer("Plant"));
        LeafMask |= (1 << LayerMask.NameToLayer("Leaf"));
        NutrientMask |= (1 << LayerMask.NameToLayer("Nutrient"));
        GroundMask |= (1 << LayerMask.NameToLayer("Ground"));
        SkyMask |= (1 << LayerMask.NameToLayer("Sky"));
        BranchMask |= (1 << LayerMask.NameToLayer("Branch"));
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
        BranchFilter = new ContactFilter2D();
        BranchFilter.SetLayerMask(BranchMask);

        RockCollision = new Collider2D[10];

        MainCamera = Camera.main;

        CostDisplay = Instantiate(NumberDisplay, Vector3.zero, Quaternion.identity);
        HideBuildCost();

        CurrentRoot = Instantiate(RootPrefab, Vector3.zero, Quaternion.identity);
        PlaceRoot(new Vector3(0, -0.3f, 0), new Vector3(0, 1.3f, 0));

        // Place endpoint to top of root
        endpoint = Instantiate(RootItemPrefab, new Vector3(0, -0.3f, 0), Quaternion.identity);


        CreateLeaf(Vector3.zero);
        PlaceLeaf(new Vector3(0, 1f, 0));
        FinishLeaf();
        CheckLeaves();
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
                if (RootIsUnderground)
                {
                    RootSource.PlayOneShot(RootClip);
                }
                else
                {
                    RootSource.PlayOneShot(StemClip);
                }

                // Main branch
                nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
                    BranchFilter,
                    RockCollision
                );
                if (nContacts == 1)
                {
                    if (Vector3.Magnitude(endpoint.transform.position - MouseDragStart) < 0.5)
                    {
                        endpoint.transform.position = MousePosition;
                    }
                    else
                    {
                        endpoint.transform.position = MouseDragStart;
                    }
                    endpoint.transform.rotation = CurrentRoot.transform.rotation;
                    //CurrentRoot = Instantiate(RootItemThickPrefab, MousePosition, Quaternion.identity);
                }
                else
                {
                    CurrentRoot.transform.localScale = new Vector3(CurrentRoot.transform.localScale.x * 0.5f, CurrentRoot.transform.localScale.y, CurrentRoot.transform.localScale.z);
                }
            }
            else
            {
                Destroy(CurrentRoot);
            }
            HideBuildCost();
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
            FinishLeaf();

            HideBuildCost();
        }
        else if (MouseDraggingLeaf)
        {
            PlaceLeaf(MousePosition);
            DisplayBuildCost((int)Mathf.Round(CurrentLeaf.transform.localScale.y * 50), MousePosition);
            VerifyLeaf();
        }
        CheckLeaves();
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
                    Debug.Log(RockCollision[i].GetComponent<SpriteRenderer>());
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
        GameObject newObject = Instantiate(RootPrefab, Position, Quaternion.identity);

        if (CurrentRoot != null && CurrentRoot.transform.position.y < 0)
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = new Color(0.650f, 0.313f, 0f);
        }
        CurrentRoot = newObject;
    }
    void CreateLeaf(Vector3 Position)
    {
        CurrentLeaf = Instantiate(LeafPrefab, Position, Quaternion.identity);
        CurrentShade = Instantiate(ShadePrefab, Position, Quaternion.identity);
        CurrentLeaf.GetComponent<Leaf>().Shade = CurrentShade;
        CurrentLeaf.GetComponent<Leaf>().PlayerOwned = true;
        if (CurrentRoot != null && CurrentRoot.transform.position.y < 0)
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = new Color(0.650f, 0.313f, 0f);
        }
    }

    void PlaceLeaf(Vector3 position)
    {
        CurrentLeaf.transform.position = new Vector3(position.x + CurrentLeaf.transform.localScale.x * 1.8f, position.y);
        CurrentShade.transform.position = new Vector3(CurrentLeaf.transform.position.x, CurrentLeaf.transform.position.y / 2, position.y);
        CurrentShade.transform.localScale = new Vector3(12, CurrentLeaf.transform.position.y * 6.637f, 1);
        CurrentLeaf.GetComponent<Leaf>().LeftEdgeStart = CurrentLeaf.transform.position.x - 1.65f;
        CurrentLeaf.GetComponent<Leaf>().RightEdgeStart = CurrentLeaf.transform.position.x + 1.65f;
    }

    void PlaceRoot(Vector3 StartPosition, Vector3 EndPosition)
    {
        CurrentRoot.transform.position = (EndPosition + StartPosition) / 2f;
        CurrentRoot.transform.rotation = Quaternion.Euler(0, 0,
            90f + Mathf.Rad2Deg * Mathf.Atan2(EndPosition.y - StartPosition.y, EndPosition.x - StartPosition.x)
        );
        CurrentRoot.transform.localScale = new Vector3(1, (EndPosition - StartPosition).magnitude * 0.8f + 0.25f, 1);
    }

    void FinishLeaf()
    {
        CurrentLeaf.GetComponent<Leaf>().LeftEdge = CurrentLeaf.GetComponent<Leaf>().LeftEdgeStart;
        CurrentLeaf.GetComponent<Leaf>().RightEdge = CurrentLeaf.GetComponent<Leaf>().RightEdgeStart;
        ResizeLeafShade(CurrentLeaf);
        AllLeaves.Add(CurrentLeaf);
        CheckLeaves();
    }

    void ResizeLeafShade(GameObject Leaf)
    {
        /*
        Leaf LeafLeaf = Leaf.GetComponent<Leaf>();
        Leaf.GetComponent<Leaf>().Shade.transform.localScale = new Vector3(
            (LeafLeaf.RightEdge - LeafLeaf.LeftEdge) * 3.6363636363636363636363636363636f,
            LeafLeaf.Shade.transform.localScale.y,
            LeafLeaf.Shade.transform.localScale.z
        );
        LeafLeaf.Shade.transform.position = new Vector3(
            (LeafLeaf.RightEdge + LeafLeaf.LeftEdge) / 2f + 0.05f,
            LeafLeaf.Shade.transform.position.y,
            LeafLeaf.Shade.transform.position.z
        );
        */
    }

    void CheckIfShrink(GameObject TopLeaf, GameObject BottomLeaf)
    {
        bool Happened = false;
        Leaf TopLeafLeaf = TopLeaf.GetComponent<Leaf>();
        Leaf BottomLeafLeaf = BottomLeaf.GetComponent<Leaf>();
        if (
            TopLeafLeaf.LeftEdge < BottomLeafLeaf.RightEdge &&
            TopLeafLeaf.LeftEdge > BottomLeafLeaf.LeftEdge
        )
        {
            Happened = true;
            BottomLeafLeaf.RightEdge = TopLeafLeaf.LeftEdge;
        }
        else if (
            TopLeafLeaf.RightEdge < BottomLeafLeaf.RightEdge &&
            TopLeafLeaf.RightEdge > BottomLeafLeaf.LeftEdge
        )
        {
            Happened = true;
            BottomLeafLeaf.LeftEdge = TopLeafLeaf.RightEdge;
        }
        if (Happened)
        {
            ResizeLeafShade(BottomLeaf);
        }
    }

    void CompareLeaves(GameObject LeafA, GameObject LeafB)
    {
        if (LeafA.transform.position.y > LeafB.transform.position.y)
        {
            CheckIfShrink(LeafA, LeafB);
        }
        else
        {
            CheckIfShrink(LeafB, LeafA);
        }
    }

    float GetSunPercentage(GameObject Leaf)
    {
        Leaf LeafLeaf = Leaf.GetComponent<Leaf>();
        return Mathf.Max(0, (LeafLeaf.RightEdge - LeafLeaf.LeftEdge) / (LeafLeaf.RightEdgeStart - LeafLeaf.LeftEdgeStart));
    }

    void CheckLeaves()
    {
        float SunlightPercentage = 0;
        for (int i = 0; i < AllLeaves.Count; i++)
        {
            for (int j = i + 1; j < AllLeaves.Count; j++)
            {
                CompareLeaves(AllLeaves[i], AllLeaves[j]);
            }
            if (AllLeaves[i].GetComponent<Leaf>().PlayerOwned) SunlightPercentage += GetSunPercentage(AllLeaves[i]) / 4f;

        }
        ResourceTracker.Sunlight = SunlightPercentage;
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
