using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using TMPro;

public class LevelUI : NetworkBehaviour
{
    public TMP_Text score;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        score.text = NetworkClient.localPlayer.GetComponent<NetworkPlayer>().GetScore();
    }
}
