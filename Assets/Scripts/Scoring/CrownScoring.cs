using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CrownScoring : NetworkBehaviour
{
    public GlobalTime _globalTime;
    public float sceneChangeTimer;
    private float _time;
    private List<NetworkPlayer> players;
    private GameNetworkManager _gameNetMan;
    private bool gameIsOver;

    void Start()
    {
        if (!isServer) return;
        _gameNetMan = ((GameNetworkManager) NetworkManager.singleton);
        players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        _globalTime = FindObjectOfType<GlobalTime>();
        
        InvokeRepeating("crown", 0f, 0.25f);
        InvokeRepeating("updateTime", 0f, 0.1f);
    }

    private void updateTime()
    {
        _time = _globalTime.matchTime;
        if (_time <= sceneChangeTimer)
        {
            CancelInvoke();
            _gameNetMan.AfterLevelEnd();
        }
    }

    private void crown()
    {
        foreach (var player in players)
        {
            if (player.playerCharacter.gameObject.GetComponent<PlayerCollision>().hasCrown)
            {
                player.ChangeScore(1);
            }
        }
    }
}