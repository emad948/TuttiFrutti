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

public class GameLevelsManager : NetworkBehaviour
{
    public static GameLevelsManager _instance;
    
    void Awake()
    {
        if (_instance != null)
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                Destroy(_instance.gameObject);
                _instance = this;
                DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }   
    //private string[] _gameLevels = {"Level_Crown"}; 
    //private string[] _gameLevels = {"Level_HillKing", "Level_Crown", "Level_RunTheLine"}; 
    private string[] _gameLevels = {"Level_RunTheLine"};
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
            // TODO @Colin: does not care about even points 
        }
        
        ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene("ScoreBoard");
        Invoke("startLevel", 5f);
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
            //case "Level_HillKing":
             //    ChangeScene("Level_HillKing");
             //    break;
            case "Level_RunTheLine":
                ChangeScene("Level_RunTheLine");
                break;
             //case "Level_Crown":
              //   Debug.Log("Here1");
              //   ChangeScene("Level_Crown");
               //  break;
            //case "WinnerBoard":
            //    ChangeScene("WinnerBoard");
             //   break;
            default:
                Debug.Log("Unknown scene name");
                break;
        }
    }
    
    #region HelperFunctions

    public void ChangeScene(string scene)
    {
        ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene(scene);

        Debug.Log("after game scene changed");
    }

    public string GETNextGameLevel()
    {
        if (_gameLevels.Length == 0) return "WinnerBoard";
        var nextGameLevel = _gameLevels[0];
        _gameLevels = _gameLevels.Skip(1).ToArray();
        return nextGameLevel;
    }

    #endregion
}