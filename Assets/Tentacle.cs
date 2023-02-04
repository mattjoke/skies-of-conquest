using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public int length = 10;
    public LineRenderer lineRenderer;
    public Vector3[] segments;
    public Vector3[] segmentsVelocity;

    public Transform target;
    public float targetDistance = 1f;
    public float smoothTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = length;
        segments = new Vector3[length];
        segmentsVelocity = new Vector3[length];
    }

    // Update is called once per frame
    void Update()
    {
        segments[0] = target.position;
        for (int i = 1; i < segments.Length; i++)
        {
            segments[i] = Vector3.SmoothDamp(segments[i], segments[i - 1] + target.right * targetDistance, ref segmentsVelocity[i], smoothTime);
        }
        lineRenderer.SetPositions(segments);
    }
}
