using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoardController : NetworkBehaviour
{
    [SerializeField] private TMP_Text[] playersTexts = new TMP_Text[4];
    public bool isWinnerScene;
    private GameNetworkManager _gameNetworkManager;


    private void Awake()
    {
        _gameNetworkManager = ((GameNetworkManager) NetworkManager.singleton);
        if (isWinnerScene)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (isServer)
            {
                List<NetworkPlayer> players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
                foreach (NetworkPlayer player in players)
                {
                    player.DuplicateScores(); // for the compareTo method (sorting)
                }
            }
        }
    }

    private void Start()
    {
        List<NetworkPlayer> players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        players.Sort();

        // var counter = PlayersList.Count;
        // foreach (NetworkPlayer player in PlayersList)
        // {
        //     player.UpdateTotalScore(counter);
        //     counter--;
        //     // TODO @Colin: even points?? 
        // }

        for (int i = 0; i < players.Count; i++)
        {
            playersTexts[i].text = $"{players[i].GetDisplayName()} : {players[i].GetScore(isWinnerScene)}";
        }
    }

    public void backToMenu()
    {
        if (isServer)
        {
            if (_gameNetworkManager.usingSteam)
            {
                NetworkServer.DisconnectAll();
            }
            else
            {
                NetworkServer.Shutdown();
            }

            _gameNetworkManager.StopHost();
        }
        else
        {
            _gameNetworkManager.StopClient();
        }

        SceneManager.LoadScene(0);
    }
}