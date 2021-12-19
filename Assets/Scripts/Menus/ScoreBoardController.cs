
using System.Collections.Generic;

using Mirror;
using TMPro;

using UnityEngine;

public class ScoreBoardController : NetworkBehaviour
{
  
    [SerializeField] private TMP_Text[] playersTexts = new TMP_Text[4];

    private void Start()
    {
        List<NetworkPlayer> players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;

        //TODO @Emad sort list according to scores
        for (int i = 0; i < players.Count; i++)
        {
            playersTexts[i].text = $"{players[i].GetDisplayName()} : {players[i].GetScore()}";
        }
    }
}