using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPoles : MonoBehaviour
{
    private List<GameObject> poles;
    private List<float> offsets;
    private List<Vector3> orgPos;
    private List<float> wiggleSpeeds;
    private float wiggleDistance = 2f;
    //private float wiggleSpeed = 1.2f;

    void Start()
    {
        var Go = this;
        poles = new List<GameObject>();
        offsets = new List<float>();
        orgPos = new List<Vector3>();
        wiggleSpeeds = new List<float>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            var g = Go.transform.GetChild(i).gameObject;
            poles.Add(g);
            offsets.Add(Random.Range(0f, 100f));
            orgPos.Add(g.transform.localPosition);
            wiggleSpeeds.Add(Random.Range(1f,1.8f));
        }
    }

    void Update()
    {
        var i = 0;
        foreach (GameObject go in poles)
        {
            float yPosition = Mathf.Cos(Time.time * wiggleSpeeds[i] + offsets[i]) * wiggleDistance;
            go.transform.localPosition = orgPos[i] + new Vector3(0, yPosition, 0);
            i++;
        }
    }
}