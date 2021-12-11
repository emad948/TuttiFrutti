using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameNetworkManager : NetworkManager
{
    
    [SerializeField] private Menu _menu;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transport steamTransport;
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public List<NetworkPlayer> PlayersList { get; } = new List<NetworkPlayer>();

    private bool _gameStarted = false;


    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!_gameStarted) return;
        conn.Disconnect();

    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkPlayer player = conn.identity.GetComponent<NetworkPlayer>();
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
        if (!_menu.testMode && PlayersList.Count < 2) return;
        
        _gameStarted = true;
        
        ServerChangeScene("HillKing");
    }


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        
        var playerName="";
        
        if (_menu.useSteam)
        {
            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(Menu.LobbyId, numPlayers - 1);
            playerName = SteamFriends.GetFriendPersonaName(steamId);
        }
        else
        {
            playerName = $"Player {numPlayers}";
            Debug.Log(playerName);
        }
        
        
        NetworkPlayer player = conn.identity.GetComponent<NetworkPlayer>();
        
        PlayersList.Add((player));
        
       
        
        player.SetDisplayName(playerName);


        Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        
        player.SetColor(randomColor);
        
        //if the playersList contain only 1 player this player is the host
        player.SetGameHost(PlayersList.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.Equals("HillKing"))
        {
            foreach (NetworkPlayer player in PlayersList)
            {
                Debug.Log(characterPrefab);
                GameObject characterInstance = Instantiate(
                    characterPrefab,
                    GetStartPosition().position,
                    Quaternion.identity
                );
                
                NetworkServer.Spawn(characterInstance,player.connectionToClient);

            }
        }
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
   
    public void setUseSteam(bool useSteam){
        Transport kcpTransport = this.GetComponent<kcp2k.KcpTransport>();
        SteamManager steamManager = this.GetComponent<SteamManager>();
        steamManager.enabled = useSteam;
        steamTransport.enabled = useSteam;
        kcpTransport.enabled = !useSteam;
        
        if (useSteam) this.transport = steamTransport;
        else          this.transport = kcpTransport;
        
        Transport.activeTransport = this.transport;

        print("GameNetworkManager: useSteam=" + useSteam);
    }

   
}
