using System.Collections.Generic;
using UnityEngine;

public class PläntBase : MonoBehaviour
{

    public GameObject stemPrefab;
    public GameObject leafPrefab;

    public GameObject clicker;

    private int highestLevel = 0;

    private void OnMouseDrag()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().SetPosition(0, GetComponent<List<Vector2>>()[GetComponent<List<Vector2>>().Count - 1]);
        GetComponent<LineRenderer>().SetPosition(1, mouse);
        GetComponent<LineRenderer>().startColor = Color.red;
        GetComponent<LineRenderer>().endColor = Color.blue;
    }
    private void OnMouseUp()
    {
        GetComponent<List<Vector2>>().Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Instantiate(clicker, GetComponent<List<Vector2>>()[GetComponent<List<Vector2>>().Count - 1], Quaternion.identity);
    }
}