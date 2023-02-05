using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberPopup : MonoBehaviour
{

    public TMP_Text textMesh;
    Color TextColor;

    void Start()
    {
        TextColor = textMesh.color;
    }

    void Update()
    {
        transform.position += new Vector3(0, Time.deltaTime, 0);
        TextColor.a -= Time.deltaTime;
        textMesh.color = TextColor;
        if(TextColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
