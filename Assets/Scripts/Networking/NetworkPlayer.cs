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

    [SyncVar] [SerializeField] private Color color_T;
    [SyncVar] [SerializeField] private Color color_M;
    [SyncVar] [SerializeField] private Color color_B;

    [SyncVar(hook = nameof(AuthorityHandleUpdateGameHostState))]
    public bool _isGameHost = false;

    [SyncVar] public string lobbyName = "lobbyNameNotSet";

    [SyncVar(hook = nameof(HandleColorNameChange))] [SerializeField]
    private string updateName_Color = "nothing";

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

    [Command]
    public void cmdForVoteLevel(int levelIndex)
    {
        ((GameNetworkManager) NetworkManager.singleton).levelVote(levelIndex);
    }

    public bool GetIsGameHost()
    {
        return _isGameHost;
    }

    public string GetDisplayName()
    {
        return _displayName;
    }

    [Command]
    public void cmdToUpdateColorName(Color t, Color m, Color b, string playerName)
    {
        color_T = t;
        color_M = m;
        color_B = b;
        _displayName = playerName;
    }

    private void HandleColorNameChange(string oldName_Color, string newName_Color)
    {
        if (hasAuthority)
        {
            var c_r = PlayerPrefs.GetFloat("color_T_r", 0.5f);
            var c_g = PlayerPrefs.GetFloat("color_T_g", 0.5f);
            var c_b = PlayerPrefs.GetFloat("color_T_b", 0.5f);
            var c_a = PlayerPrefs.GetFloat("color_T_a", 0.7f);
            color_T = new Color(c_r, c_g, c_b, c_a);

            c_r = PlayerPrefs.GetFloat("color_M_r", 0.5f);
            c_g = PlayerPrefs.GetFloat("color_M_g", 0.5f);
            c_b = PlayerPrefs.GetFloat("color_M_b", 0.5f);
            c_a = PlayerPrefs.GetFloat("color_M_a", 0.7f);
            color_M = new Color(c_r, c_g, c_b, c_a);

            c_r = PlayerPrefs.GetFloat("color_B_r", 0.5f);
            c_g = PlayerPrefs.GetFloat("color_B_g", 0.5f);
            c_b = PlayerPrefs.GetFloat("color_B_b", 0.5f);
            c_a = PlayerPrefs.GetFloat("color_B_a", 0.7f);
            color_B = new Color(c_r, c_g, c_b, c_a);
            var oldName = _displayName;
            _displayName = PlayerPrefs.GetString("playerName", randomPlayerName());
            HandleDisplayNameUpdated(oldName, _displayName);
            cmdToUpdateColorName(color_T, color_M, color_B, _displayName);
            //updateName_Color = "nothing";
        }
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

    public Color GetColor_T()
    {
        return color_T;
    }

    public Color GetColor_M()
    {
        return color_M;
    }

    public Color GetColor_B()
    {
        return color_B;
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
    public void ChangeColorName(string newString) => updateName_Color = newString;

    [Server]
    public void SetGameHost(bool state) => _isGameHost = state;

    [Server]
    public void SetDisplayName(string newDisplayName) => _displayName = newDisplayName;

    [Server]
    public void ChangeScore(int scorePoints) => _currentScore += scorePoints;

    [Server]
    public void UpdateTotalScore(int scorePoints) => _totalScore += scorePoints;

    [Server]
    public void ResetCurrentScore() => _currentScore = 0;

    [Server]
    public void DuplicateScores() => _currentScore = _totalScore; // for the compareTo method (sorting)

    public void ResetTotalScore() => _totalScore = 0;

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