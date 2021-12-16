using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using TMPro;

public class GlobalTime : NetworkBehaviour
{
    public TMP_Text matchTimeText;
    public TMP_Text timerText;
    [SyncVar] public float _time = -1f;
    [SyncVar] public float matchTime = 90f;
    
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
    
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            _time += Time.deltaTime;
            if (_time > 0)
            {
                matchTime -= Time.deltaTime;
            }
        }
        if (_time < 0)
        {
            timerText.text = Math.Round(_time).ToString();
        }
        else
        {
            timerText.text = "";
            matchTimeText.text = Math.Round(matchTime).ToString();
        }
    }
}