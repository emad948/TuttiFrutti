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
    [SerializeField] private Menu _menu;
    private bool lobbyUiActive = false;

    private void Start()
    {
        //When client connects 
        GameNetworkManager.ClientOnConnected += HandleClientConnected;
        NetworkPlayer.AuthorityOnGameHostStateUpdated += AuthorityHandleGameHostStateUpdated;
        NetworkPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy()
    {
        GameNetworkManager.ClientOnConnected -= HandleClientConnected;
        NetworkPlayer.AuthorityOnGameHostStateUpdated -= AuthorityHandleGameHostStateUpdated;
        NetworkPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        ((GameNetworkManager) NetworkManager.singleton).OnDestroy();
    }

    public void ClientHandleInfoUpdated()
    {
        List<NetworkPlayer> players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        for (int i = 0; i < players.Count; i++)
        {
            playersNameTexts[i].text = players[i].GetDisplayName();
        }

        for (int i = players.Count; i < playersNameTexts.Length; i++)
        {
            playersNameTexts[i].text = "Waiting for player to join.";
        }

        //StartGame button will be disabled if players are less than 2
        if (!_menu.testMode)
        {
            startGameButton.interactable = players.Count > 1;
        }
    }

    private void HandleClientConnected()
    {
        lobbyUiActive = !lobbyUiActive;
        lobbyUi.SetActive(true);
    }

    private void AuthorityHandleGameHostStateUpdated(bool state)
    {
        //Turns the start/stop game button on/off
        startGameButton.gameObject.SetActive(state);
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<NetworkPlayer>().CmdStartGame();
    }

    public void LeaveLobby()
    {
        //Host
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            //NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();
            ((GameNetworkManager) NetworkManager.singleton).StopHost();
            //NetworkManager.singleton.OnStopServer();
        }
        //Client
        else
        {
            ((GameNetworkManager) NetworkManager.singleton).StopClient();
        }

        //SceneManager.LoadScene(0);
    }
}