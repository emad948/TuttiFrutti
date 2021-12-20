using System;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameNetworkManager : NetworkManager
{
    public static GameNetworkManager _instance;
    
    public override void Awake()
    {
        if (_instance != null && SceneManager.GetActiveScene().name != "MainMenu")
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // uncomment?
        }
        base.Awake();

        _menu = GameObject.FindGameObjectWithTag("MainMenuDisplayTag").GetComponent<Menu>();
    }
    
    [SerializeField] private Menu _menu;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transport steamTransport;

    private GameLevelsManager _gameLevelsManager;


    private bool _gameStarted;

    public List<NetworkPlayer> PlayersList { get; } = new List<NetworkPlayer>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public void setUseSteam(bool useSteam)
    {
        Transport kcpTransport = GetComponent<KcpTransport>();
        var steamManager = GetComponent<SteamManager>();
        steamManager.enabled = useSteam;
        steamTransport.enabled = useSteam;
        kcpTransport.enabled = !useSteam;

        if (useSteam) transport = steamTransport;
        else transport = kcpTransport;

        Transport.activeTransport = transport;

        print("GameNetworkManager: useSteam=" + useSteam);
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
        if (!_menu.testMode && PlayersList.Count < 2) return;

        _gameStarted = true;

        //Game levels manager will be created we the game starts
        _gameLevelsManager = GetComponentInChildren<GameLevelsManager>();
        
        _gameLevelsManager.startLevel();
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var playerName = "";

        if (_menu.useSteam)
        {
            var steamId = SteamMatchmaking.GetLobbyMemberByIndex(Menu.LobbyId, numPlayers - 1);
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


    public void LeaveGame()
    {
        // SceneManager.LoadScene(0);   // or up here?
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
        SceneManager.LoadScene(0);
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

    #region HelperFunctions
    
    #endregion
}