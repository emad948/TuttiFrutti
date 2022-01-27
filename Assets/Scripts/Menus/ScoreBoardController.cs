using System;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScoreBoardController : NetworkBehaviour
{
    public bool isWinnerScene;
    private GameNetworkManager _gameNatMan;
    private List<NetworkPlayer> players;
    
    [SerializeField] private Text[] posTexts = new Text[10];
    [SerializeField] private Text[] scoresTexts = new Text[10];
    [SerializeField] private Text[] namesTexts = new Text[10];
    [SerializeField] private GameObject[] medals = new GameObject[3];
    [SerializeField] private GameObject[] entryBackground = new GameObject[5];

    
    private void Awake()
    {
        _gameNatMan = ((GameNetworkManager) NetworkManager.singleton);
        players = _gameNatMan.PlayersList;
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
            if (i  % 2 == 0)
            {
                entryBackground[i].GameObject().SetActive(true);
            }
            
            if (i < 3)
            {
                medals[i].GameObject().SetActive(true);
            }
            
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

            posTexts[i].text = rankString;
            scoresTexts[i].text = players[i].GetScore(isWinnerScene);
            namesTexts[i].text = players[i].GetDisplayName();

            if (players[i].isLocalPlayer)
            {
                posTexts[i].color=Color.green;
                posTexts[i].fontStyle = FontStyle.Bold;
                scoresTexts[i].color=Color.green;
                scoresTexts[i].fontStyle = FontStyle.Bold;
                namesTexts[i].color=Color.green;
                namesTexts[i].fontStyle = FontStyle.Bold;
            }
        
            

        }
    }

    public void backToMenu()
    {
        if (_gameNatMan.usingSteam)
        {
            SteamMatchmaking.LeaveLobby(_gameNatMan.currentLobbyID);
        }

        if (isServer)
        {
            _gameNatMan.StopHost();
            NetworkServer.Shutdown();
        }

        _gameNatMan.StopClient();

        SceneManager.LoadScene(0);
    }
}