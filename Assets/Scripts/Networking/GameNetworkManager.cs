using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameNetworkManager : NetworkManager
{

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public List<Player> PlayersList { get; } = new List<Player>();

    private bool _gameStarted = false;


    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!_gameStarted) return;
        conn.Disconnect();

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Player player = conn.identity.GetComponent<Player>();
        Debug.Log(player);
        PlayersList.Remove(player);
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        PlayersList.Clear();
        _gameStarted = false;
    }
    
    public void StartGame()
    {
        if (PlayersList.Count < 2) return;
        _gameStarted = true;
        
        ServerChangeScene("GameScene_1");
    }


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Player player = conn.identity.GetComponent<Player>();
        
        PlayersList.Add((player));
        
        //TODO @Emad get player name from UI or Steam 
        player.SetDisplayName($"Player: {numPlayers}");


        Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        
        player.SetColor(randomColor);
        
        //if the playersList contain only 1 player this player is the host
        player.SetGameHost(PlayersList.Count == 1);
    }

    #endregion

    #region Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
       PlayersList.Clear();
    }

    #endregion
   

   
}
