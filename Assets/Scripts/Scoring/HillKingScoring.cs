using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HillKingScoring : NetworkBehaviour
{
    private string currentLevel;
    private List<NetworkPlayer> players;
    public int currenZoneIndex = 1;
    public GlobalTime _globalTime;
    private float _time;
    private bool onlyOnce = true;

    public float sceneChangeTimer = 85f;    // TODO change to correct value
    public bool testingMode = true;         
    
    private void Start()
    {
        if (!isServer) return;
        players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;

        InvokeRepeating("HillKing", 0f, 0.25f);
    }

    private void Update()
    {
        if (!isServer) return;
        _time = _globalTime.matchTime;
        if (_time <= sceneChangeTimer && onlyOnce && !testingMode) // TODO @Colin change to actual matchTimer and also <= 0
        {
            //After 90 seconds end game and go to ScoringBoard
            CancelInvoke();
            ((GameNetworkManager) NetworkManager.singleton).GETGameLevelsManager().AfterLevelEnd();
            onlyOnce = false;
        }
    }

    private void HillKing()
    {
        // TODO update currentZoneIndex
        foreach (NetworkPlayer player in players)
        {
            var pos = player.playerCharacter.transform.position;
            switch (currenZoneIndex)
            {
                case 1:
                    if (pos.x > 18 && pos.x < 23)
                    {
                        if (pos.z > -23 && pos.z < -18)
                        {
                            if (pos.y > 12)
                            {
                                player.ChangeScore(1);
                            }
                        }
                    }

                    break;
                case 2:
                    // TODO @Colin update coordinate for other zones
                    if (pos.x > 18 && pos.x < 23)
                    {
                        if (pos.z > -23 && pos.z < -18)
                        {
                            if (pos.y > 12)
                            {
                                player.ChangeScore(1);
                            }
                        }
                    }

                    break;
                case 3:
                    // TODO @Colin update coordinate for other zones
                    if (pos.x > 18 && pos.x < 23)
                    {
                        if (pos.z > -23 && pos.z < -18)
                        {
                            if (pos.y > 12)
                            {
                                player.ChangeScore(1);
                            }
                        }
                    }

                    break;
                default:
                    Debug.Log("Error");
                    break;
            }
        }
    }
    public void addPointToPlayer(GameObject player){
        NetworkPlayer current = null;
        foreach (var np in players){
            if (np.playerCharacter.gameObject == player) current = np; 
        } 
        if (current is null) return;
        current.ChangeScore(1);
        print("add point to player " + current.GetDisplayName()); // keep me!
    }
    #region Client
    

    #endregion
}