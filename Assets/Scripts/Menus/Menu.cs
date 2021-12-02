using System;
using Mirror;
using Steamworks;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;

    [SerializeField] private bool useSteam = false;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;


    private void Start()
    {
        //This is only for development purposes
        if(!useSteam){return;}

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

    }

    public void HostLobby()
    {
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

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        //Steam Failed to create a Lobby
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            landingPagePanel.SetActive(true);
            return;
        }
        //if lobby creation succeeded
        
        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData
        (new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress",
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
            "HostAddress"
        );
        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();
        
        landingPagePanel.SetActive(false);
    }
    
    
}