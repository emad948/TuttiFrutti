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
    

    
    // TODO @Emad
    //1- show players score as your score 
    //2-show  player ranking
    //3-show top 10 players
    
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

        if (!isWinnerScene && isServer)
        {
            var counter = players.Count;
            foreach (NetworkPlayer player in players)
            {
                player.UpdateTotalScore(counter);
                counter--;
                // TODO @Colin: even points?? 
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            int rank = i + 1;
            string rankString;
            switch (rank)
            {
                default:
                    rankString = rank + "TH"; break;
                case 1:
                    rankString = "1ST"; break;
                case 2:
                    rankString = "2ND"; break;
                case 3:
                    rankString = "3RD"; break;
                
            }
            playersTexts[i].text = $"{rankString} {players[i].GetDisplayName()} : {players[i].GetScore(isWinnerScene)}";
        }
    }

    public void backToMenu()
    {
        if (isServer)
        {
            NetworkServer.Shutdown();
        }

        _gameNetworkManager.StopClient();

        SceneManager.LoadScene(0);
    }
}