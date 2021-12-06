using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkPlayer :NetworkBehaviour
{
    
    //SyncVar are only Updated on the server
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] private string _displayName;
    [SyncVar] [SerializeField] private Color color;
    [SyncVar(hook=nameof(AuthorityHandleUpdateGameHostState))] private bool _isGameHost = false;
    
    
    //Lobby Events
    public static event Action<bool> AuthorityOnGameHostStateUpdated;
    public static event Action ClientOnInfoUpdated;

    public static event Action<string> ClientOnDisplayNameChanged; 

    public bool GetIsGameHost()
    {
        return _isGameHost;
    }

    public string GetDisplayName()
    {
        return _displayName;
    }
    
    
    
     #region Server

    public override void OnStartServer()
    {
        //prevent unity from destroying gameObject when loading Scene
        DontDestroyOnLoad(gameObject);
    }
    
    [Server] public void SetGameHost(bool state) => _isGameHost = state;
    [Server] public void SetDisplayName(string newDisplayName) => _displayName = newDisplayName;
    [Server] public void SetColor(Color newColor) => color = newColor;
    
    [Command]
    public void CmdStartGame()
    {
        //Only the host is allowed to start the game
        if (!_isGameHost) return;
        ((GameNetworkManager)NetworkManager.singleton).StartGame();
      
    }

    
    #endregion

    #region Client

    public override void OnStartClient()
    {
        //if this is a server don't add to player list
        if (NetworkServer.active) return;
        
        DontDestroyOnLoad(gameObject);
        
        ((GameNetworkManager)NetworkManager.singleton).PlayersList.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        
        //if not server
        if (!isClientOnly) return;
        ((GameNetworkManager)NetworkManager.singleton).PlayersList.Remove(this);
    }

    
    

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnDisplayNameChanged?.Invoke(newName);
        ClientOnInfoUpdated?.Invoke();
        
        
       
    }
    
    #endregion

    #region AuthotityHandlers

    private void AuthorityHandleUpdateGameHostState(bool oldState, bool newState)
    {
        if (!hasAuthority) return;
        AuthorityOnGameHostStateUpdated?.Invoke(newState);
    }

    #endregion
}
