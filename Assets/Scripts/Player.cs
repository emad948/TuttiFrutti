using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerDisplayName;
    [SerializeField] private Renderer displayColorRenderer;
    
    //SyncVar are only Updated on the server
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] private string _displayName;
    [SyncVar(hook = nameof(HandleDisplayColorUpdated))] [SerializeField] private Color color;
    [SyncVar(hook=nameof(AuthorityHandleUpdateGameHostState))] private bool isGameHost = false;

    [SyncVar] public Vector3 Control;
    
    //Lobby Events
    public static event Action<bool> AuthorityOnGameHostStateUpdated;
    public static event Action ClientOnInfoUpdated;

    public bool GetIsGameHost => isGameHost;

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
    
    [Server] public void SetGameHost(bool state) => isGameHost = state;
    [Server] public void SetDisplayName(string newDisplayName) => _displayName = newDisplayName;
    [Server] public void SetColor(Color newColor) => color = newColor;
    
    [Command]
    public void CmdStartGame()
    {
        //Only the host is allowed to start the game
        if (!isGameHost) return;
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

    
    private void HandleDisplayColorUpdated(Color oldColor, Color newColor) => displayColorRenderer.material.color=newColor;

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
        playerDisplayName.text = newName;
        
    }
    
    [ClientRpc]
    private void Update()
    {
        if (isClientOnly)
        {
            Control = new Vector3(Input.GetAxis("Horizontal") * .2f, 0,
                Input.GetAxis("Vertical") * .2f); //update our controll varible
            GetComponent<PhysicsLink>().ApplyForce(Control, ForceMode.VelocityChange); //Use our custom force function
        }
        
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
