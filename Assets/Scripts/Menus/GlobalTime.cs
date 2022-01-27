using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using TMPro;

public class GlobalTime : NetworkBehaviour
{
    public GameObject startingCamera;
    public TMP_Text matchTimeText;
    public TMP_Text timerText;
    [SyncVar] public float _time;
    [SyncVar] public float matchTime;

    public float levelEntryDelayTime = 1f; 
    //[SyncVar] public bool levelIsRunning = true;
    public GameObject loadingScenePanel;

    private void Awake()
    {
        loadingScenePanel.SetActive(true);
        _time = -levelEntryDelayTime;
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
        if (_time >= -1f)
        {
            startingCamera.SetActive(false);
        }

        if (_time > -4) // will be implemented... 
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