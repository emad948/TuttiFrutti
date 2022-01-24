using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class LobbyListItem : MonoBehaviour
{
    public CSteamID lobbySteamId;
    public string lobbyName;
    public int numberOfPlayers;
    public int maxNumberOfPlayers;

    [SerializeField] private Text LobbyNameText;
    [SerializeField] private Text NumerOfPlayersText;
    private Menu _menu;

    private void Start()
    {
        _menu = _menu = GameObject.FindGameObjectWithTag("MainMenuDisplayTag").GetComponent<Menu>();
    }

    public void SetLobbyItemValues()
    {
        LobbyNameText.text = lobbyName;
        NumerOfPlayersText.text =
            "Number of Players: " + numberOfPlayers.ToString() + "/" + maxNumberOfPlayers.ToString();
    }

    public void JoinLobby()
    {
        //Debug.Log("JoinLobby: Player selected to join lobby with steam id of: " + lobbySteamId.ToString());
        _menu.JoinLobby(lobbySteamId);
    }
}