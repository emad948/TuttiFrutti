using System;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [HideInInspector] public GameNetworkManager _gameNetworkManager;

    //public GameObject steamController;
    [SerializeField] private GameObject landingPagePanel;
    private bool useSteam = false;
    [SerializeField] public bool testMode = false;
    public Button toggleSteamButton;
    public TMP_Text steamErrorText;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HOST_ADDRESS = "HOST_ADDRESS";

    //public static CSteamID LobbyId { get; private set; }            
    public CSteamID LobbyId { get; private set; } // here!!


    private void Start()
    {
        _gameNetworkManager = (GameNetworkManager) NetworkManager.singleton;
        steamErrorText.enabled = false;
        //This is only for development purposes
        if (!useSteam)
        {
            lobbyCreated = null;
            gameLobbyJoinRequested = null;
            lobbyEntered = null;
            return;
        }

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not Initialized");
            return;
        }

        if (_gameNetworkManager.steamInitOnlyOnce)
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            _gameNetworkManager.steamInitOnlyOnce = false;
        }
    }

    public void HostLobby()
    {
        if (useSteam)
        {
            if (SteamManager.Initialized)
            {
                //TODO @Emad change lobby to public ?
                landingPagePanel.SetActive(false);
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 18);
                return;
            }
            else
            {
                steamErrorText.enabled = true;
            }
        }
        else
        {
            landingPagePanel.SetActive(false);
            NetworkManager.singleton.StartHost();
        }
    }

    public bool getUseSteam()
    {
        return useSteam;
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //Steam Failed to create a Lobby
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to Create A Steam Lobby");
            landingPagePanel.SetActive(true);
            return;
        }

        //if lobby creation succeeded
        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        if (!NetworkServer.active || !NetworkClient.active)
        {
            NetworkManager.singleton.StartHost();
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
        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        landingPagePanel.SetActive(false);
    }

    public void quitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void toggleUseSteam()
    {
        useSteam = !useSteam;
        //steamController.SetActive(useSteam);
        _gameNetworkManager.setUseSteam(useSteam);
        if (useSteam)
        {
            toggleSteamButton.GetComponentInChildren<TMP_Text>().color = Color.green;
        }
        else
        {
            toggleSteamButton.GetComponentInChildren<TMP_Text>().color = Color.red;
        }

        this.Start();
    }
}