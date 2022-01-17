using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WiggleObject : MonoBehaviour
{
    public float wiggleDistance = 2;
    public float wiggleSpeed = 1;
    public float offset;

    private TMP_Text text;
    private Vector3 orgPos;
    private float orgFontSize;
    private float fontSizeFactor = 3.5f;

    // set x and y pos in start() to wiggle around
    private void Start()
    {
        orgPos = transform.localPosition;
        text = this.GetComponentInChildren<TMP_Text>();
        orgFontSize = text.fontSize;
    }

    void Update()
    {
        float xPosition = Mathf.Sin(Time.time * wiggleSpeed + offset) * wiggleDistance;
        float yPosition = Mathf.Cos(Time.time * wiggleSpeed + offset) * wiggleDistance;
        transform.localPosition = orgPos + new Vector3(xPosition, yPosition, 0);
        // 
        text.fontSize = orgFontSize + Mathf.Sin(Time.time * wiggleSpeed + offset) * fontSizeFactor;
    }
}