using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelUI : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text levelIndex;

    // Start is called before the first frame update
    void Start()
    {
        int curlevelIndex = ((GameNetworkManager) NetworkManager.singleton).currentLevelIndex + 1;
        levelIndex.text = curlevelIndex.ToString() + "/4"; 
    }

    // Update is called once per frame
    void Update()
    {
        score.text = NetworkClient.localPlayer.GetComponent<NetworkPlayer>().GetScore(false);
    }
}