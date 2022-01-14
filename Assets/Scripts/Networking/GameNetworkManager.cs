using System;
using System.Collections;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    private Menu _menu;
    private bool _gameStarted;
    private Transport steamTransport;
    public List<NetworkPlayer> PlayersList { get; } = new List<NetworkPlayer>();
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public override void Awake()
    {
        base.Awake();
        _menu = GameObject.FindGameObjectWithTag("MainMenuDisplayTag").GetComponent<Menu>();
    }

    public void setUseSteam(bool useSteam)
    {
        steamTransport = GetComponent<FizzySteamworks>();
        Transport kcpTransport = GetComponent<KcpTransport>();
        var steamManager = GetComponent<SteamManager>();
        steamManager.enabled = useSteam;
        steamTransport.enabled = useSteam;
        kcpTransport.enabled = !useSteam;

        if (useSteam) transport = steamTransport;
        else transport = kcpTransport;

        Transport.activeTransport = transport;
    }

    public void Update()
    {
        Debug.Log(PlayersList);
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

        startLevel();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var playerName = "";

        if (_menu.getUseSteam())
        {
            var steamId = SteamMatchmaking.GetLobbyMemberByIndex(_menu.LobbyId, numPlayers - 1);
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
        resettingLevelsManager();
        SceneManager.LoadScene(0);
    }

    public override void OnStopClient()
    {
        resettingLevelsManager();
    }

    public override void OnStopHost()
    {
        //resettingLevelsManager(); needed?
    }

    #endregion

    #region (Previously) GameLevelsManager

    private string[] _gameLevels = {"Level_HillKing"};

    //private string[] _gameLevels = {"Level_HillKing", "Level_Crown", "Level_RunTheLine"}; 
    private bool gameIsRunning = false;

    private void resettingLevelsManager()
    {
        gameIsRunning = false;
        PlayersList.Clear(); // also for OnClientDisconnect
        _gameLevels = new string[] {"Level_HillKing"}; // TODO change to all levels
    }

    public override void Start()
    {
        //if (!isServer) return;
        //if (NetworkServer.active && NetworkClient.isConnected)
        //if(mode == NetworkManagerMode.Host)
        // -----
        base.Start();
        //Shuffle Game Levels
        _gameLevels = RandomStringArrayTool.RandomizeStrings(_gameLevels);
    }

    public void AfterLevelEnd()
    {
        ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene("ScoreBoard");
        Invoke("startLevel", 5f);
    }

    public void startLevel()
    {
        if (gameIsRunning)
        {
            foreach (NetworkPlayer player in PlayersList)
            {
                player.ResetCurrentScore();
            }
        }

        gameIsRunning = true;
        string level = GETNextGameLevel();
        switch (level)
        {
            case "Level_HillKing":
                ChangeScene("Level_HillKing");
                break;
            case "Level_RunTheLine":
                ChangeScene("Level_RunTheLine");
                break;
            case "Level_Crown":
                ChangeScene("Level_Crown");
                break;
            case "WinnerBoard":
                ChangeScene("WinnerBoard");
                break;
            default:
                Debug.Log("Unknown scene name");
                break;
        }
    }

    public void ChangeScene(string scene)
    {
        ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene(scene);
    }

    public string GETNextGameLevel()
    {
        if (_gameLevels.Length == 0) return "WinnerBoard";
        var nextGameLevel = _gameLevels[0];
        _gameLevels = _gameLevels.Skip(1).ToArray();
        return nextGameLevel;
    }

    #endregion
}