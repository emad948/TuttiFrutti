using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movePlayerFromStart : MonoBehaviour
{
    public GameObject wall1;
    public GameObject wall2;
    public GameObject wall3;
    public GameObject wall4;
    public GlobalTime _globalTime;
    private int counter = 0;
    private bool onlyOnce = false;

    private void Update()
    {
        if (!onlyOnce && _globalTime.matchTime < 100f)  
        {
            InvokeRepeating("movingForward", 0f, 0.2f);
            onlyOnce = true;
        }
    }

    void movingForward()
    {
        wall1.transform.localPosition += new Vector3(0, 0, 0.04f);
        wall2.transform.localPosition -= new Vector3(0, 0, 0.04f);
        wall3.transform.localPosition += new Vector3(0.04f, 0, 0);
        wall4.transform.localPosition -= new Vector3(0.04f, 0, 0);
       counter++;
       if (counter == 25)
       {
           CancelInvoke();
       }
    }
}