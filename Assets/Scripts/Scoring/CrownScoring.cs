using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using System;

public class CrownScoring : NetworkBehaviour
{
    public GlobalTime _globalTime;
    public float sceneChangeTimer;
    private float _time;
    private List<NetworkPlayer> players;
    private GameNetworkManager _gameNetMan;
    private bool gameIsOver;

    public AudioSource audioCrown;

    void Start()
    {
        if (!isServer) return;
        _gameNetMan = ((GameNetworkManager) NetworkManager.singleton);
        players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;

        var count = players.Count;
        var rnd = new System.Random();

        audioCrown = GetComponent<AudioSource>();

        List<int> zoneIndices = new List<int>();
        zoneIndices = zoneIndices.OrderBy(item => rnd.Next()).ToList();
        var _crownCount = crownCount(players.Count);
        for (var i = 0; i < _crownCount; i++) // TODO Colin: set relative to num players 
        {
            var curCollision = players[i].playerCharacter.gameObject.GetComponent<PlayerCollision>();
            curCollision.hasCrown = true;
        }

        InvokeRepeating("crown", 3f, 0.25f);
        InvokeRepeating("updateTime", 3f, 0.1f);
    }

    private int crownCount(int playerCount)
    {
        if (playerCount > 1 && playerCount < 4)
        {
            return 1;
        }
        else if (playerCount >= 4 && playerCount < 7)
        {
            return 2;
        }
        else if  (playerCount >= 7)
        {
            return (playerCount / 3);
        }
        Debug.Log("Error in playerCount");
        return 0;
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
                audioCrown.Play();
                player.ChangeScore(1);
            }
        }
    }
}