using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RunLineScoring : NetworkBehaviour
{
    private List<NetworkPlayer> resultList = new List<NetworkPlayer>();
    private List<NetworkPlayer> players;
    private GameNetworkManager _gameNetMan;
    private bool gameIsOver;
    
    void Start()
    {
        _gameNetMan = ((GameNetworkManager) NetworkManager.singleton);
        players = ((GameNetworkManager) NetworkManager.singleton).PlayersList; 
    }

    void Update()
    {
        if (isServer)
        {
            foreach (NetworkPlayer player in players)
            {
                if (pos.x >= 5.59 && pos.x >= 81.1)      // coordinates are bigger than
                {
                    resultList.Add(player);
                    players.Remove(player);
                }
            }

            if (globalTime.matchTime <=0)     // End when globalTime <0=
            {
                gameEnded();
            }
        }
    }

    private void gameEnded()
    {
        if (players.Count >= 0)
        {
            if(players.Count == 0)
            {
                resultList.Add(players[0]);     // correct Index? 
            }
            //else   Zero point for players who doesnt reach the finishline
            //{
                // figure out order of remaining players via difference between pos and finish.pos
                // and add to resultList
            //    foreach (NetworkPlayer player in players)
            //    {

            //    }
           // }
        }
        
        var counter = resultList.Count;
        foreach (NetworkPlayer player in resultList)
        {
            player.ChangeScore(counter);
            counter--;
        }

        _gameNetMan.AfterLevelEnd();
    }
}
