using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUi;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text[] playersNameTexts = new TMP_Text[4];

    private void Start()
    {
        //When client connects 
        GameNetworkManager.ClientOnConnected += HandleClientConnected;
        Player.AuthorityOnGameHostStateUpdated += AuthorityHandleGameHostStateUpdated;
        Player.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy()
    {
        GameNetworkManager.ClientOnConnected -= HandleClientConnected;
         Player.AuthorityOnGameHostStateUpdated -= AuthorityHandleGameHostStateUpdated;
         Player.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    public void ClientHandleInfoUpdated()
    {
        List<Player> players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        for(int i=0;i<players.Count;i++)
        {
            playersNameTexts[i].text = players[i].GetDisplayName();
        }

        for (int i = players.Count; i < playersNameTexts.Length; i++)
        {
            playersNameTexts[i].text = "Waiting for player to join.";
        }
        
        //StartGame button will be disabled if players are less than 2
        //TODO @Emad uncomment
        // startGameButton.interactable = players.Count > 1;
    }

    private void HandleClientConnected()
    {
        lobbyUi.SetActive(true);
    }

    private void AuthorityHandleGameHostStateUpdated(bool state)
    {
        //Turns the start/stop game button on/off
        startGameButton.gameObject.SetActive(state);
    }
   
    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
    }

    public void LeaveLobby()
    {
        //Host
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        //Client
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}