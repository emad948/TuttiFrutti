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
    [HideInInspector] public Menu _menu;
    private bool _gameStarted;
    private Transport steamTransport;
    public bool steamInitOnlyOnce { get; set; } = true;
    public bool usingSteam { get; set; } = false;
    public List<NetworkPlayer> PlayersList { get; } = new List<NetworkPlayer>();
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    private const string HOST_ADDRESS = "HOST_ADDRESS";
    public CSteamID LobbyId { get; private set; }
    public CSteamID currentLobbyID { get; private set; }
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;

    private List<GameObject> listOfLobbyListItems = new List<GameObject>();

    public List<CSteamID> lobbyIDS = new List<CSteamID>();
    public bool publicLobbies = true;

    public Levels startingLevel;
    private string[] _allGameLevels;
    private string[] _gameLevels;
    public int currentLevelIndex = 0;

    struct LobbyMetaData
    {
        public string m_Key;
        public string m_Value;
    }

    struct LobbyMembers
    {
        public CSteamID m_SteamID;
        public LobbyMetaData[] m_Data;
    }

    struct Lobby
    {
        public CSteamID m_SteamID;
        public CSteamID m_Owner;
        public LobbyMembers[] m_Members;
        public int m_MemberLimit;
        public LobbyMetaData[] m_Data;
    }

    public enum Levels
    {
        Level_HillKing = 1,
        Level_PerfectMatch = 2,
        Level_Crown = 3,
        Level_RunTheLine = 4
    }

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

    public void updateMenuReference()
    {
        _menu = GameObject.FindGameObjectWithTag("MainMenuDisplayTag").GetComponent<Menu>();
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
        _gameStarted = true;
        startLevel();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var playerName = "";
        var player = conn.identity.GetComponent<NetworkPlayer>();
        PlayersList.Add(player);

        if (usingSteam)
        {
            var steamId = SteamMatchmaking.GetLobbyMemberByIndex(LobbyId, numPlayers - 1);
            playerName = SteamFriends.GetFriendPersonaName(steamId);
        }
        else
        {
            playerName = player.GetDisplayName();
        }

        player.SetDisplayName(playerName);
        
        if (PlayersList.Count == 1)
        {
            player.SetGameHost(true);
            player.lobbyName = _menu.lobbyName;
        }
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

    private string decodeLevel(Levels l)
    {
        if (l is Levels.Level_HillKing) return "Level_HillKing";
        if (l is Levels.Level_PerfectMatch) return "Level_PerfectMatch";
        if (l is Levels.Level_Crown) return "Level_Crown";
        if (l is Levels.Level_RunTheLine) return "Level_RunTheLine";
        return "MainMenu";
    }


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
    }

    public override void Start()
    {
        base.Start();
        _allGameLevels = new string[] {decodeLevel(startingLevel)};
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

    public void GetListOfLobbies(bool _public)
    {
        publicLobbies = _public;
        if (lobbyIDS.Count > 0)
            lobbyIDS.Clear();
        SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(1);
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
        SteamMatchmaking.AddRequestLobbyListStringFilter("gameName", "TuttiFrutti",
            ELobbyComparison.k_ELobbyComparisonEqual);
        SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();
    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {
        if (_menu.listOfLobbyListItems.Count > 0)
            _menu.DestroyOldLobbyListItems();
        if (publicLobbies)
        {
            for (int i = 0; i < result.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                lobbyIDS.Add(lobbyID);
                SteamMatchmaking.RequestLobbyData(lobbyID);
            }
        }
        else
        {
            List<CSteamID> lobbyIDS = new List<CSteamID>();
            EFriendFlags friendFlags = EFriendFlags.k_EFriendFlagImmediate;
            int cFriends = SteamFriends.GetFriendCount(friendFlags);
            for (int i = 0; i < cFriends; i++)
            {
                FriendGameInfo_t friendGameInfo;
                CSteamID steamIDFriend = SteamFriends.GetFriendByIndex(i, friendFlags);
                if (SteamFriends.GetFriendGamePlayed(steamIDFriend, out friendGameInfo) &&
                    friendGameInfo.m_steamIDLobby.IsValid())
                {
                    lobbyIDS.Add(friendGameInfo.m_steamIDLobby);
                    SteamMatchmaking.RequestLobbyData(friendGameInfo.m_steamIDLobby);
                }
            }

            _menu.DisplayFriendsLobbies(lobbyIDS);
        }
    }

    void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {
        _menu.DisplayLobbies(lobbyIDS, result);
    }

    // SteamLobby END


    public void menuStart()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not Initialized");
            return;
        }

        if (steamInitOnlyOnce)
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
            Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
            steamInitOnlyOnce = false;
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to Create A Steam Lobby");
            FindObjectOfType<Menu>().landingPagePanel.SetActive(true);
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        currentLobbyID = LobbyId;
        if (!NetworkServer.active || !NetworkClient.active)
        {
            this.StartHost();
        }

        SteamMatchmaking.SetLobbyData(
            LobbyId,
            HOST_ADDRESS,
            SteamUser.GetSteamID().ToString()
        );
        if (_menu.didPlayerNameTheLobby)
        {
            SteamMatchmaking.SetLobbyData(
                LobbyId,
                "name",
                _menu.lobbyName
            );
        }
        else
        {
            SteamMatchmaking.SetLobbyData(
                LobbyId,
                "name",
                SteamFriends.GetPersonaName().ToString() + "'s lobby"
            );
        }

        // for filtering results when searching lobbies
        SteamMatchmaking.SetLobbyData(
            LobbyId,
            "gameName",
            "TuttiFrutti"
        );
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return; //if Host
        currentLobbyID = (CSteamID) callback.m_ulSteamIDLobby;
        string hostAddress = SteamMatchmaking.GetLobbyData(
            currentLobbyID,
            HOST_ADDRESS
        );
        networkAddress = hostAddress;
        StartClient();
        FindObjectOfType<Menu>().landingPagePanel.SetActive(false);
    }

    #endregion
}