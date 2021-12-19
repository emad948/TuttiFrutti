
using System.Collections.Generic;

using Mirror;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardController : NetworkBehaviour
{
    [SerializeField] private TMP_Text[] playersTexts = new TMP_Text[4];
    public bool isWinnerScene = false;

    private void Start()
    {
        List<NetworkPlayer> players = ((GameNetworkManager) NetworkManager.singleton).PlayersList;
        if (isWinnerScene)
        {
            foreach (NetworkPlayer player in players)
            {
                player.DuplicateScores(); // for the compareTo method (sorting)
            }
        }
        players.Sort();
        for (int i = 0; i < players.Count; i++)
        {
            playersTexts[i].text = $"{players[i].GetDisplayName()} : {players[i].GetScore(isWinnerScene)}";
        }
    }

    public void backToMenu()
    {
        ((GameNetworkManager) NetworkManager.singleton).LeaveGame();
    }
}