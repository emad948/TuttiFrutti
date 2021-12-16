using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GlobalTime : NetworkBehaviour
{
    private char sepFloat = '.';
    private string xAfterDot(float num, int x)
    {
        string b = num.ToString();
        int dot = b.IndexOf(sepFloat);
        string[] seperated = b.Split(sepFloat);
        x = x < seperated[1].Length ? x : seperated[1].Length - 1;
        string afterDot = "";
        for (int i = 0; i < x ; i++) afterDot += seperated[1][i]; // fix: no more out of bounds in substring
        return seperated[0] + sepFloat + afterDot;
    }

    public Text timeText;
    [SyncVar] public float _time = -3f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            _time += Time.deltaTime;
        }
        //timeText.text = xAfterDot(time, 2);
        timeText.text = Math.Round(_time).ToString();

    }
}
