using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiggleObject : MonoBehaviour
{
    public float wiggleDistance = 2;
    public float wiggleSpeed = 1;
    public float offset;

    private Vector3 orgPos;

    // set x and y pos in start() to wiggle around
    private void Start()
    {
        orgPos = transform.localPosition;
    }

    void Update()
    {
        float xPosition = Mathf.Sin(Time.time * wiggleSpeed + offset) * wiggleDistance;
        float yPosition = Mathf.Cos(Time.time * wiggleSpeed + offset) * wiggleDistance;
        transform.localPosition = orgPos + new Vector3(xPosition, yPosition, 0);
    }
}