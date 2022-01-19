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
    public bool steamInitOnlyOnce { get; set; } = true;
    public bool usingSteam { get; set; } = false;
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
        usingSteam = useSteam;
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
        resettingLevelsManager();
    }

    public void StartGame()
    {
        //if (!_menu.testMode && PlayersList.Count < 2) return;

        _gameStarted = true;

        startLevel();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var playerName = "";

        if (usingSteam)
        {
            var steamId = SteamMatchmaking.GetLobbyMemberByIndex(LobbyId, numPlayers - 1);
            playerName = SteamFriends.GetFriendPersonaName(steamId);
        }
        else
        {
            playerName = randomPlayerName();
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

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
        
        /*  If disconnected from server for any reason, go back to main menu.
        *** I don't know, how to reset the client tho, therefore, another connection is currently not possible */
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        resettingLevelsManager();
        SceneManager.LoadScene(0);
        StopClient();
    }

    public override void OnStopClient()
    {
        resettingLevelsManager();
        base.OnStopClient();
    }

    public override void OnStopHost()
    {
        resettingLevelsManager();
        base.OnStopHost();
    }

    #endregion

    #region (Previously) GameLevelsManager
    public enum Levels{
        Level_HillKing = 1,
        Level_PerfectMatch = 2,
        Level_Crown = 3,
        Level_RunTheLine = 4
    }

    private string decodeLevel(Levels l){
        if (l is Levels.Level_HillKing) return "Level_HillKing";
        if (l is Levels.Level_PerfectMatch) return "Level_PerfectMatch";
        if (l is Levels.Level_Crown) return "Level_Crown";
        if (l is Levels.Level_RunTheLine) return "Level_RunTheLine";
        return "MainMenu";     
    }

    public Levels startingLevel;
    private string[] _allGameLevels;
    private string[] _gameLevels;
    public int currentLevelIndex = 0;
    

    //private string[] _gameLevels = {"Level_HillKing", "Level_Crown", "Level_RunTheLine", "PerfectMatch"}; 
    //private bool gameIsRunning = false;

    private void resettingLevelsManager()
    {
        currentLevelIndex = 0;
        _gameStarted = false;
        usingSteam = false;
        foreach (NetworkPlayer player in PlayersList)
        {
            player.ResetTotalScore();
        }
        PlayersList.Clear();
        _gameLevels = _allGameLevels;
        //  TODO _gameLevels = {"Level_HillKing", "Level_Crown", "Level_RunTheLine", "PerfectMatch"};
    }

    public override void Start()
    {
        base.Start();
        _allGameLevels = new string[]{decodeLevel(startingLevel)};
        //Shuffle Game Levels
        _gameLevels = RandomStringArrayTool.RandomizeStrings(_allGameLevels);
    }

    public void AfterLevelEnd()
    {
        ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene("ScoreBoard");
        // has to be bigger then 0f for showing correct (non-zero) scores in scene
        Invoke("startLevel", 3f); 
    }

    public void startLevel()
    {
        _gameStarted = true;
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
            case "Level_PerfectMatch":
                ChangeScene("Level_PerfectMatch");
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
        if (scene != "WinnerBoard")
        {
            if (_gameStarted)
            {
                foreach (NetworkPlayer player in PlayersList)
                {
                    player.ResetCurrentScore();
                }
            }
        }

        ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene(scene);
    }

    public string GETNextGameLevel()
    {
        if (_gameLevels.Length == 0) return "WinnerBoard";
        currentLevelIndex++;
        var nextGameLevel = _gameLevels[0];
        _gameLevels = _gameLevels.Skip(1).ToArray();
        return nextGameLevel;
    }

    private string randomPlayerName()
    {
        var names = new List<string>()
        {
            "Orange", "Guava", "Apple", "Banana", "Kiwi", "Pommegranate", "Fig", "Acerola", "Mandarine", "Avocado",
            "Watermelon", "Lemon", "Pineapple", "Mango", "Plum", "Coconut"
        };
        var random = new System.Random();
        return names[random.Next(names.Count)];
    }

    #endregion
    
    #region (Previously in) Menu.cs
    
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    private const string HOST_ADDRESS = "HOST_ADDRESS";
    public CSteamID LobbyId { get; private set; }

    public void menuStart()
    {
        if (!usingSteam)
        {
            // lobbyCreated = null;
            // gameLobbyJoinRequested = null;
            // lobbyEntered = null;
            // return;
        }

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not Initialized");
            return;
        }

        //SteamManager.
        if (steamInitOnlyOnce)
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            steamInitOnlyOnce = false;
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //Steam Failed to create a Lobby
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to Create A Steam Lobby");
            var a = FindObjectOfType<Menu>();
            a.landingPagePanel.SetActive(true);
            return;
        }

        //if lobby creation succeeded
        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        if (!NetworkServer.active || !NetworkClient.active)
        {
            this.StartHost();
        }

        SteamMatchmaking.SetLobbyData
        (LobbyId,
            HOST_ADDRESS,
            SteamUser.GetSteamID().ToString()
        );
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //if Host
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HOST_ADDRESS
        );
        networkAddress = hostAddress;
        //Debug.Log("here2");

        StartClient();
        //Debug.Log("here3");

        var a = FindObjectOfType<Menu>();
        a.landingPagePanel.SetActive(false);
    }

    #endregion
}