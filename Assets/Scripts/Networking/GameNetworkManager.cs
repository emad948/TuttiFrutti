using System;
using System.Collections.Generic;
using kcp2k;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameNetworkManager : NetworkManager
{
    [SerializeField] private Menu _menu;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transport steamTransport;


    private bool _gameStarted;

    public List<NetworkPlayer> PlayersList { get; } = new List<NetworkPlayer>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public void setUseSteam(bool useSteam)
    {
        Transport kcpTransport = GetComponent<KcpTransport>();
        var steamManager = GetComponent<SteamManager>();
        steamManager.enabled = useSteam;
        steamTransport.enabled = useSteam;
        kcpTransport.enabled = !useSteam;

        if (useSteam) transport = steamTransport;
        else transport = kcpTransport;

        Transport.activeTransport = transport;

        print("GameNetworkManager: useSteam=" + useSteam);
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

        GameObject.FindObjectOfType<GameLevelsManager>().startLevel();
    }


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var playerName = "";

        if (_menu.useSteam)
        {
            var steamId = SteamMatchmaking.GetLobbyMemberByIndex(Menu.LobbyId, numPlayers - 1);
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
    }

    public override void OnStopClient()
    {
        PlayersList.Clear();
    }

    #endregion
}