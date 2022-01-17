using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HillKingScoring : NetworkBehaviour
{
    private string currentLevel;
    private List<NetworkPlayer> players;
    [HideInInspector] [SyncVar] public int currentZoneIndex;
    public GlobalTime _globalTime;

    public LightboxManager lightboxes;
    private float _time;
    private bool onlyOnce = true;
    private List<int> zoneIndices = new List<int>() {1, 2, 3};
    private int counter = -1;
    public float sceneChangeTimer; // TODO change to correct value
    public bool testingMode = true;

    private void Start()
    {
        players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        var abc = (GameNetworkManager) NetworkManager.singleton;
        if (!isServer) return; // move higher again
        _globalTime = FindObjectOfType<GlobalTime>();
        currentZoneIndex = 1;
        //shuffling zones 
        var rnd = new System.Random();
        zoneIndices = zoneIndices.OrderBy(item => rnd.Next()).ToList();
        InvokeRepeating("changeZoneIndex", Math.Abs(_globalTime._time), 50f);
        InvokeRepeating("HillKing", 0f, 0.25f);
    }

    private void Update()
    {
        
        if (!isServer) return;
        _time = _globalTime.matchTime;
        if (_time <= sceneChangeTimer && onlyOnce &&
            !testingMode) 
            // TODO @Colin change to actual matchTimer and also <= 0
        {
            Debug.Log("Hello from update");
            //After 90 seconds end game and go to ScoringBoard
            CancelInvoke();
            ((GameNetworkManager) NetworkManager.singleton).AfterLevelEnd();
            onlyOnce = false;
        }
    }

    private void changeZoneIndex()
    {
        counter++;
        counter %= 3;
        currentZoneIndex = zoneIndices[counter];
    }

    private void HillKing()
    {
        // TODO update currentZoneIndex
        foreach (NetworkPlayer player in players)
        {
            var pos = player.playerCharacter.transform.position;
            switch (currentZoneIndex)
            {
                case 1:
                    // grey tower
                    if (pos.x >= 18 && pos.x <= 23)
                    {
                        if (pos.z >= (-23) && pos.z <= (-18))
                        {
                            if (pos.y >= 11.8)
                            {
                                player.ChangeScore(1);
                            }
                        }
                    }

                    break;
                case 2:
                    // yellow tower
                    if (pos.x >= 5.75 && pos.x <= 9.75)
                    {
                        if (pos.z >= (-6.25) && pos.z <= 0.75)
                        {
                            if (pos.y >= 8.8)
                            {
                                player.ChangeScore(1);
                            }
                        }
                    }

                    break;
                case 3:
                    // green tower
                    if (pos.x >= (-7.25) && pos.x <= (-3.25))
                    {
                        if (pos.z >= (-14.75) && pos.z <= (-10.75))
                        {
                            if (pos.y >= 12.8)
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

    public void addPointToPlayer(GameObject player)
    {
        NetworkPlayer current = null;
        foreach (var np in players)
        {
            if (np.playerCharacter.gameObject == player) current = np;
        }

        if (current is null) return;
        current.ChangeScore(1);
        //print("add point to player " + current.GetDisplayName()); // keep me!
    }

    #region Client

    #endregion
}