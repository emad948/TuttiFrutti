using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour, IComparable<NetworkPlayer>
{
    //SyncVar are only Updated on the server
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField]
    private string _displayName;

    [SyncVar] [SerializeField] private Color color;

    [SyncVar(hook = nameof(AuthorityHandleUpdateGameHostState))]
    private bool _isGameHost = false;

    //Scores
    [SyncVar(hook = nameof(HandleScoreUpdated))]
    private int _currentScore = 0;

    [SyncVar] private int _totalScore = 0;
    public static event Action<int> ClientOnScoreUpdated;
    public GameObject playerCharacter;

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

    public string GetScore(bool isWinnerScene)
    {
        if (isWinnerScene)
        {
            return _totalScore.ToString();
        }
        else
        {
            return _currentScore.ToString();
        }
    }


    public Color GetColor()
    {
        return color;
    }

    public int CompareTo(NetworkPlayer other)
    {
        if (this._currentScore < other._currentScore)
        {
            return 1;
        }
        else if (this._currentScore > other._currentScore)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }


    #region Server

    public override void OnStartServer()
    {
        //prevent unity from destroying gameObject when loading Scene
        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public void SetGameHost(bool state) => _isGameHost = state;

    [Server]
    public void SetDisplayName(string newDisplayName) => _displayName = newDisplayName;

    [Server]
    public void SetColor(Color newColor) => color = newColor;

    [Server]
    public void ChangeScore(int scorePoints) => _currentScore += scorePoints;

    [Server]
    public void UpdateTotalScore(int scorePoints) => _totalScore += scorePoints;

    [Server]
    public void ResetCurrentScore() => _currentScore = 0;

    [Server]
    public void DuplicateScores() => _currentScore = _totalScore; // for the compareTo method (sorting)

    [Command]
    public void CmdStartGame()
    {
        //Only the host is allowed to start the game
        if (!_isGameHost) return;
        ((GameNetworkManager) NetworkManager.singleton).StartGame();
    }

    #endregion

    #region Client

    public override void OnStartClient()
    {
        //if this is a server don't add to player list
        if (NetworkServer.active) return;

        DontDestroyOnLoad(gameObject);

        ((GameNetworkManager) NetworkManager.singleton).PlayersList.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        //if not server
        if (!isClientOnly) return;
        ((GameNetworkManager) NetworkManager.singleton).PlayersList.Remove(this);
    }


    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnDisplayNameChanged?.Invoke(newName);
        ClientOnInfoUpdated?.Invoke();
    }

    private void HandleScoreUpdated(int oldScore, int newScore)
    {
        ClientOnScoreUpdated?.Invoke(newScore);
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