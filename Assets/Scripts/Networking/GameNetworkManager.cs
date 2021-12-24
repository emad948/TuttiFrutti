using System;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameNetworkManager : NetworkManager
{
    [SerializeField] private GameObject characterPrefab;
    //public GameObject steamController;
    private Transport steamTransport;
    private menuController _menuController;
    private GameLevelsManager _gameLevelsManager;
    private bool _gameStarted;
    public List<NetworkPlayer> PlayersList { get; } = new List<NetworkPlayer>();
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public override void Awake()
    {
        base.Awake();
        _menuController = GetComponent<menuController>();
    }
    
    public void setUseSteam(bool useSteam)
    {
        //steamTransport = steamController.GetComponent<FizzySteamworks>();
        steamTransport = GetComponent<FizzySteamworks>();
        Transport kcpTransport = GetComponent<KcpTransport>();
        //var steamManager = steamController.GetComponent<SteamManager>();
        var steamManager = GetComponent<SteamManager>();
        steamManager.enabled = useSteam;
        steamTransport.enabled = useSteam;
        kcpTransport.enabled = !useSteam;

        if (useSteam) transport = steamTransport;
        else transport = kcpTransport;

        Transport.activeTransport = transport;

        Debug.Log("UsingSteam: " + useSteam);
    }

    #region Server

    
  
    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!_gameStarted) return;
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        var player = conn.identity.GetComponent<NetworkPlayer>();
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
        if (!_menuController.testMode && PlayersList.Count < 2) return;

        _gameStarted = true;

        //Game levels manager will be created we the game starts
        _gameLevelsManager = GameObject.FindGameObjectWithTag("GameLevelsManager").GetComponent<GameLevelsManager>();
        
        _gameLevelsManager.startLevel();
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var playerName = "";

        if (_menuController.getUseSteam())
        {
            var steamId = SteamMatchmaking.GetLobbyMemberByIndex(_menuController.LobbyId, numPlayers - 1);
            playerName = SteamFriends.GetFriendPersonaName(steamId);
        }
        else
        {
            playerName = $"Player {numPlayers}";
        }


        var player = conn.identity.GetComponent<NetworkPlayer>();

        PlayersList.Add(player);


        player.SetDisplayName(playerName);


        var randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        player.SetColor(randomColor);

        //if the playersList contain only 1 player this player is the host
        player.SetGameHost(PlayersList.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Level"))
            foreach (var player in PlayersList)
            {
                var characterInstance = Instantiate(
                    characterPrefab,
                    GetStartPosition().position,
                    GetStartPosition().rotation
                );
                characterInstance.GetComponent<PlayerCharacter>().SetDisplayName(player.GetDisplayName());
                characterInstance.GetComponent<PlayerCharacter>().SetColor(player.GetColor());
                player.playerCharacter = characterInstance;

                NetworkServer.Spawn(characterInstance, player.connectionToClient);
            }
    }


    // public void LeaveGame()
    // {
    //     // SceneManager.LoadScene(0);   // or up here?
    //     if (NetworkServer.active && NetworkClient.isConnected)
    //     {
    //         NetworkManager.singleton.StopHost();
    //     }
    //     else
    //     {
    //         NetworkManager.singleton.StopClient();
    //     }
    //     SceneManager.LoadScene(0);
    // }
    

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
        
        /*  If disconnected from server for any reason, go back to main menu.
        *** I don't know, how to reset the client tho, therefore, another connection is currently not possible */        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }

    public override void OnStopClient()
    {
        PlayersList.Clear();
    }


    #endregion

    #region HelperFunctions
    
    #endregion
}