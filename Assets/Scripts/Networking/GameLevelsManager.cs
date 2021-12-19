using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO when backToMenu - implement first backToMenu after all levels were played

public class GameLevelsManager : NetworkBehaviour
{
    private string[] _gameLevels = {"Level_HillKing"}; 
    private List<NetworkPlayer> players;
    private bool gameIsRunning = false;     // TODO figure out when gameIsRunning and change accordingly
    
    private void Start()
    {
        //DontDestroyOnLoad(this);    // still necessary as this is attached in MainMenu
        if (!isServer) return;
        players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        //Shuffle Game Levels
        _gameLevels = RandomStringArrayTool.RandomizeStrings(_gameLevels);
    }

    private void Update()
    {
        if (!isServer) return;

    }

    public void AfterLevelEnd()
    {
        players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        players.Sort();
        var counter = players.Count;
        foreach (NetworkPlayer player in players)
        {
            player.UpdateTotalScore(counter);
            counter--;
        }

        ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene("ScoringBoard");
        Invoke("startLevel", 3f);
    }

    public void startLevel()
    {
        if (gameIsRunning)
        {
            foreach (NetworkPlayer player in players)
            {
                player.ResetCurrentScore();
            }
        }

        gameIsRunning = true;
        string level = GETNextGameLevel();
        switch (level)
        {
            case "Level_HillKing":
                ChangeScene("Level_HillKing");
                break;
            default:
                SceneManager.LoadScene(0);
                //ChangeScene("MainMenu");
                Debug.Log("Unknown scene name");
                break;
        }
    }


    public void EndLevel()
    {
        //TODO @Emad add end game scene if all levels are played
        if (_gameLevels.Length == 0)
        {
            foreach (NetworkPlayer player in players)
            {
                player.DuplicateScores(); // for the compareTo method (sorting)
            }
            ChangeScene("WinnerScene");
        }

        // ChangeScene("Level_HillKing");
    }



    #region HelperFunctions

    public void ChangeScene(string scene)
    {
        ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene(scene);

        Debug.Log("after game scene changed");
    }

    public string GETNextGameLevel()
    {
        //TODO @Emad change to End Game Scene
        if (_gameLevels.Length == 0) return "WinnerScene";
        var nextGameLevel = _gameLevels[0];
        _gameLevels = _gameLevels.Skip(1).ToArray();
        return nextGameLevel;
    }

    #endregion
}