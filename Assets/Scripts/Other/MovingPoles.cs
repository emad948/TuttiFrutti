using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPoles : MonoBehaviour
{
    private List<GameObject> poles;
    private List<Vector3> orgPos;
    private List<float> wiggleSpeeds;
    private float wiggleDistance = 2.1f;

    private List<float> offsets = new List<float>() // because of syncronization
    {
        71, 25, 6, 136, 77, 80, 42, 102, 116, 75, 142, 31, 108, 152, 151, 73, 22, 52, 134, 137, 33, 68, 154, 145, 62,
        96, 90, 24, 37, 20, 78, 40, 46, 89, 59, 94, 15, 115, 93, 5, 106, 132, 85, 112, 38, 30, 149, 49, 140, 3, 12, 133,
        127, 51, 21, 88, 143, 147, 86, 34, 39, 29, 11, 109, 146, 63, 13, 70, 66, 138, 124, 36, 7, 50, 139, 107, 144,
        131, 45, 16, 61, 76, 53, 95, 23, 141, 111, 153, 98, 69, 26, 41, 4, 117, 118, 87, 129, 125, 54, 10, 35, 43, 84,
        18, 97, 110, 67, 8, 65, 114, 104, 101, 79, 105, 0, 119, 128, 148, 81, 60, 120, 55, 19, 82, 2, 28, 121, 32, 92,
        100, 150, 9, 27, 135, 58, 99, 44, 1, 48, 72, 103, 74, 47, 57, 113, 126, 130, 64, 122, 56, 123, 83, 17, 14
    };

    void Start()
    {
        var Go = this;
        poles = new List<GameObject>();
        orgPos = new List<Vector3>();
        wiggleSpeeds = new List<float>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            var g = Go.transform.GetChild(i).gameObject;
            poles.Add(g);
            orgPos.Add(g.transform.localPosition);
            wiggleSpeeds.Add(Random.Range(1f, 1.8f));
        }
    }

    void Update()
    {
        var i = 0;
        foreach (GameObject go in poles)
        {
            float yPosition = Mathf.Abs(Mathf.Cos(Time.time * wiggleSpeeds[i] + offsets[i]) * wiggleDistance);
            go.transform.localPosition = orgPos[i] + new Vector3(0, yPosition, 0);
            i++;
        }
    }
}