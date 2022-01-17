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
    [SyncVar] public float _time;
    [SyncVar] public float matchTime;
    //[SyncVar] public bool levelIsRunning = true;
    public GameObject loadingScenePanel;
    private char sepFloat = '.';

    private void Awake()
    {
        loadingScenePanel.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            _time += Time.deltaTime;
            if (_time > 0)
            {
                matchTime -= Time.deltaTime;
            }
        }

        if (_time > -4)
        {
            loadingScenePanel.SetActive(false);
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