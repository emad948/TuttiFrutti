using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class TriggerPfElimination : NetworkBehaviour
{

    public TextMeshProUGUI gameStateText;
    public GameObject gameStateUI;
    public PerfectMatchScoring scoring;
    // Start is called before the first frame update

    public void SetGameStateText(string text)
    {
        gameStateText.text = text;
    }

    void OnTriggerEnter(Collider other){
        // Message player
        gameStateUI.SetActive(true);
        gameStateText.gameObject.SetActive(true);
        SetGameStateText("Du bist ausgeschieden2");
        // 
        if(!isServer) return;
        scoring.playerFellOut(other.gameObject);
        gameStateUI.SetActive(true);
        gameStateText.gameObject.SetActive(true);
        
         SetGameStateText("Du bist ausgeschieden!");
            // veraendere meine eigenen punkte
    }
}