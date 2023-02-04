using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root1 : MonoBehaviour
{
    public GameObject rootPrefab;
    private Transform[] points;
    [SerializeField] private lr_LineController line;

    void Start(){

    }

    void Update(){

        
        DrawLine();
    }

    void DrawLine(){
        if(points.Length >= 2){
            line.SetUpLine(points);
        }
    }

}
