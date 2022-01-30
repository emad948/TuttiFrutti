using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUi;
    [SerializeField] private GameObject mainUi;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject lobbyFriendsPanel;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text[] playersNameTexts;
    [SerializeField] private Menu _menu;
    private GameNetworkManager _gameNatMan;
    private bool lobbyUiActive = false;
    public TMP_Text lobbyNameText;


    private void Start()
    {
        _gameNatMan = ((GameNetworkManager) NetworkManager.singleton);
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
    }

    public void ClientHandleInfoUpdated()
    {
        List<NetworkPlayer> players = _gameNatMan.PlayersList;
        for (int i = 0; i < players.Count; i++)
        {
            playersNameTexts[i].text = players[i].GetDisplayName();
            if (players[i]._isGameHost) lobbyNameText.text = players[i].lobbyName;
        }

        for (int i = players.Count; i < playersNameTexts.Length; i++)
        {
            playersNameTexts[i].text = "Waiting for player to join.";
        }
    }

    private void HandleClientConnected()
    {
        lobbyUiActive = !lobbyUiActive;
        lobbyUi.SetActive(true);
        mainUi.SetActive(false);
        lobbyPanel.SetActive(false);
        lobbyFriendsPanel.SetActive(false);
        ClientHandleInfoUpdated();
    }

    public void voteForLevel(int levelIndex)
    {
        List<NetworkPlayer> players = _gameNatMan.PlayersList;
        foreach (NetworkPlayer player in players)
        {
            if (player.hasAuthority)
            {
                player.cmdForVoteLevel(levelIndex);
            }
        }
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
        if (_gameNatMan.usingSteam)
        {
            SteamMatchmaking.LeaveLobby(_gameNatMan.currentLobbyID);
        }

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            _gameNatMan.StopHost();
            NetworkServer.Shutdown();
        }

        _gameNatMan.StopClient();
        SceneManager.LoadScene(0);
    }
}