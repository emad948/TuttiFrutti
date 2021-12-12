using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Score :NetworkBehaviour
{

    [SerializeField] private int startingScore = 0;

    [SyncVar(hook=nameof(HandleScoreUpdated))] private int _currentScore;

    public event Action<int> ClientOnScoreUpdated;
    
    #region Server

    public override void OnStartServer()
    {
        _currentScore = startingScore;
    }

    [Server]
    public void ChangeScore(int scorePoints)
    {
        _currentScore += scorePoints;
    }

    #endregion



    #region Client

    private void HandleScoreUpdated(int oldScore,int newScore){
        
    ClientOnScoreUpdated?.Invoke(newScore);
    }
    

    #endregion
}
