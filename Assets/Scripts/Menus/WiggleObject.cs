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
    public bool isTitleText = false;
    public float fontSizeFactor = 3.5f;

    private TMP_Text text;
    private Vector3 orgPos;
    private float orgFontSize;

    private void Start()
    {
        orgPos = transform.localPosition;
        text = this.GetComponentInChildren<TMP_Text>();
        orgFontSize = text.fontSize;
    }

    void Update()
    {
        if (!isTitleText)
        {
            float xPosition = Mathf.Sin(Time.time * wiggleSpeed + offset) * wiggleDistance;
            float yPosition = Mathf.Cos(Time.time * wiggleSpeed + offset) * wiggleDistance;
            transform.localPosition = orgPos + new Vector3(xPosition, yPosition, 0);
        }
        if (!isTitleText)
        {
            text.fontSize = orgFontSize + Mathf.Sin(Time.time * wiggleSpeed + offset) * fontSizeFactor;
        }
        else
        {
            text.fontSize = orgFontSize + Mathf.Abs(Mathf.Sin(Time.time * wiggleSpeed + offset) * fontSizeFactor);
        }
    }
}