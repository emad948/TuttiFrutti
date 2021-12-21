using System;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class menuController : MonoBehaviour
{
    private GameObject landingPagePanel;
    [SerializeField] public bool testMode = false;
    private GameNetworkManager _gameNetworkManager;
    private bool useSteam = true;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    private const string HOST_ADDRESS = "HOST_ADDRESS";
    public CSteamID LobbyId { get; private set; }

    private void Start()
    {
        //landingPagePanel = GameObject.FindWithTag("LandingPage");
        _gameNetworkManager = GetComponent<GameNetworkManager>();
        //This is only for development purposes
        if(!useSteam){
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

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby(GameObject panel)
    {
        landingPagePanel = panel;
        landingPagePanel.SetActive(false);
        if (useSteam)
        {
            //TODO @Emad change lobby to public ?
            //TODO @Emad change max players to be more than 4 
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }
        
        NetworkManager.singleton.StartHost();
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
        
        Debug.Log("here1 " + this.GetInstanceID());
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

    public void toggleUseSteam(Button toggleButton){  
        var toggleSteamButton = toggleButton;
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
