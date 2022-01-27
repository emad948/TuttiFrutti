﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public GameObject timerText;
    public GameObject gameStateUI;
    public TextMeshProUGUI gameStateText;
    public TextMeshProUGUI countdownText;
    private Grid grid;
    private GameNetworkManager _gameNetMan;
    public GlobalTime globalTime;
    
    private float threeSecondTimer = 3.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<Grid>();
        _gameNetMan = ((GameNetworkManager) NetworkManager.singleton);
        globalTime = FindObjectOfType<GlobalTime>();
    }

    // Update is called once per frame
    void Update()
    {
        timerText.SetActive(grid.revealingFruit);
        gameObject.SetActive(true);
        
        countdownText.gameObject.SetActive(true);
        countdownText.text = Mathf.Round(threeSecondTimer).ToString();
        if (threeSecondTimer < 0)
        {
            grid.canStart = true;
            countdownText.gameObject.SetActive(false);
        }
        else
        {
            // Countdown.
            threeSecondTimer -= Time.deltaTime;
        }
    }

    public void SetText(string text)
    {
        timerText.GetComponent<TextMeshProUGUI>().text = text;
    }
    
    public void SetGameStateText(string text)
    {
        gameStateText.text = text;
    }

    public void SetGameState(bool isWon)
    {
        gameStateUI.SetActive(true);
        gameStateText.gameObject.SetActive(true);
    if (isWon)
    {
        SetGameStateText("Du weiter!");
      
    }
    else
    {
             SetGameStateText("Du bist ausgeschieden!");
    }
            _gameNetMan.AfterLevelEnd();
            print("gameEnds4");
    }

}
