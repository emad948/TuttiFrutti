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
    private List<NetworkPlayer> players;


    private void Awake()
    {
        _gameNetworkManager = ((GameNetworkManager) NetworkManager.singleton);
        players = _gameNetworkManager.PlayersList;

        if (isWinnerScene)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (isServer)
            {
                foreach (NetworkPlayer player in players)
                {
                    player.DuplicateScores(); // for the compareTo method (sorting)
                }
            }
        }
    }

    private void Start()
    {
        players.Sort();

        var counter = players.Count;
        foreach (NetworkPlayer player in players)
        {
            player.UpdateTotalScore(counter);
            counter--;
            // TODO @Colin: even points?? 
        }

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
                // NetworkServer.DisconnectAll();
                // _gameNetworkManager.StopServer();
                _gameNetworkManager.StopHost();
            }
            else
            {
                NetworkServer.Shutdown();
                _gameNetworkManager.StopHost();
            }
        }
        else
        {
            _gameNetworkManager.StopClient();
        }

        SceneManager.LoadScene(0);
    }
}