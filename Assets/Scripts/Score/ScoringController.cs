using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoringController : NetworkBehaviour
{
    private string currentLevel;
    private int currentLevelScore; // ??
    private List<NetworkPlayer> players;
    public int currenZoneIndex = 1;

    private void Start()
    {
        players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        
        currentLevel = SceneManager.GetActiveScene().name;

        switch (currentLevel)
        {
            case "Level_HillKing":
                InvokeRepeating("HillKing", 0f, 0.25f);
                break;
            case "Level_??":
                break;
            default:
                Debug.Log("Unknown scene name");
                break;
        }
    }

    private void HillKing()
    {
        // TODO update currentZoneIndex
        foreach (NetworkPlayer player in players)
        {
            var pos = new Vector3(19, 13, -20);
            pos = player.playerCharacter.transform.position;
            //pos = NetworkClient.connection.identity.GetComponent<PlayerCharacter>().transform.position;
            Debug.Log(pos.ToString());
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

    #region Client

    private void handleClientScoreUpdated(int score)
    {
        currentLevelScore = score;
    }

    #endregion
}