using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiggleObject : MonoBehaviour
{
    float wiggleDistance = 4;
    float wiggleSpeed = 1;

    private Vector3 orgPos;
    // set x and y pos in start() to wiggle around
    private void Start()
    {
        orgPos = transform.position;
    }
 
    void Update()
    {
        float xPosition = Mathf.Sin(Time.time * wiggleSpeed) * wiggleDistance;
        float yPosition = Mathf.Cos(Time.time * wiggleSpeed) * wiggleDistance;
        transform.position = orgPos + new Vector3(xPosition, yPosition, 0);
    }
}
