using UnityEngine;

public class PlantBuilder : MonoBehaviour
{
    [SerializeField] private LayerMask RockMask;
    [SerializeField] private LayerMask PlantMask;
    [SerializeField] private LayerMask LeafMask;
    ContactFilter2D RockFilter;
    ContactFilter2D PlantFilter;
    ContactFilter2D LeafFilter;
    Collider2D[] RockCollision;

    public GameObject RootPrefab;
    public GameObject LeafPrefab;

    Camera MainCamera;
    Vector3 MouseDragStart;
    Vector3 MousePosition;

    bool RootVerified = true;
    GameObject CurrentRoot;
    bool MouseDragging;

    GameObject CurrentLeaf;
    bool LeafVerified = true;
    bool MouseDraggingLeaf;

    void Start()
    {
        RockMask |= (1 << LayerMask.NameToLayer("Rock"));
        PlantMask |= (1 << LayerMask.NameToLayer("Plant"));
        LeafMask |= (1 << LayerMask.NameToLayer("Leaf"));
        RockFilter = new ContactFilter2D();
        RockFilter.SetLayerMask(RockMask);
        PlantFilter = new ContactFilter2D();
        PlantFilter.SetLayerMask(PlantMask);
        LeafFilter = new ContactFilter2D();
        LeafFilter.SetLayerMask(LeafMask);

        RockCollision = new Collider2D[10];
        MainCamera = Camera.main;

        CreateRoot(Vector3.zero);
        PlaceRoot(new Vector3(0, -0.3f, 0), new Vector3(0, 1, 0));
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
            CreateRoot(MouseDragStart);
            MouseDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseDragging = false;
            VerifyRoot();
            if (!RootVerified)
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
                return;
            }
        }
        else if (MouseDraggingLeaf)
        {
            PlaceLeaf(MousePosition);
            VerifyLeaf();
        }
    }

    void VerifyRoot()
    {
        RootVerified = true;
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
            for (int i = 0; i < nContacts; i++)
            {
                if (Random.Range(0, 2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

        nContacts = CurrentRoot.GetComponent<BoxCollider2D>().OverlapCollider(
            LeafFilter,
            RockCollision
        );
        if (nContacts != 0)
        {
            RootVerified = false;
            for (int i = 0; i < nContacts; i++)
            {
                if (Random.Range(0, 2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        ShowVerified();
    }

    void VerifyLeaf()
    {
        LeafVerified = true;
        var nContacts = CurrentLeaf.GetComponent<BoxCollider2D>().OverlapCollider(
           RockFilter,
           RockCollision);
        if (nContacts != 0)
        {
            LeafVerified = false;
            for (int i = 0; i < nContacts; i++)
            {
                if (Random.Range(0, 2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        nContacts = CurrentLeaf.GetComponent<BoxCollider2D>().OverlapCollider(
           PlantFilter,
           RockCollision
       );
        if (nContacts != 0)
        {
            for (int i = 0; i < nContacts; i++)
            {
                if (Random.Range(0, 2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
        nContacts = CurrentLeaf.GetComponent<BoxCollider2D>().OverlapCollider(
          LeafFilter,
          RockCollision
      );
        if (nContacts != 0)
        {
            LeafVerified = false;
            for (int i = 0; i < nContacts; i++)
            {
                if (Random.Range(0, 2) == 0) RockCollision[i].GetComponent<SpriteRenderer>().color = Color.white;
                else RockCollision[i].GetComponent<SpriteRenderer>().color = Color.red;
            }
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
    }

    void PlaceLeaf(Vector3 position)
    {
        CurrentLeaf.transform.position = new Vector3(position.x + CurrentLeaf.transform.localScale.x * 1.8f, position.y);

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
        if (RootVerified )
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            CurrentRoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
        if (LeafVerified)
        {
            CurrentLeaf.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            CurrentLeaf.GetComponent<SpriteRenderer>().color = Color.red;

        }
    }

}
