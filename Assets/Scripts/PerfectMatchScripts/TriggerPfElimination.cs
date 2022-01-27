using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class TriggerPfElimination : NetworkBehaviour
{

    public TextMeshProUGUI FellOutGameText;
    public GameObject timerText;
    public GameObject FellOutText;
    public GameObject gameStateUI;

    public GameObject observerCamera;
    public PerfectMatchScoring scoring;
    // Start is called before the first frame update

  public void SetText(string text)
    {
        FellOutText.GetComponent<TextMeshProUGUI>().text = text;
    }
    
    public void SetFelloutGameStateText(string text)
    {
        FellOutGameText.text = text;
    }
    public void SetGameFelloutState(bool isWon)
    {
        
        gameStateUI.SetActive(true);
        FellOutGameText.gameObject.SetActive(true);
        StartCoroutine(hideOverlayWithDelay());
        print("gameEnds44");
    }

    public IEnumerator hideOverlayWithDelay(){
        print("test1");
        yield return new WaitForSeconds(2);
        gameStateUI.SetActive(false);
        FellOutGameText.gameObject.SetActive(false);
        print("test");
    }
  

    void OnTriggerEnter(Collider other){
        // Message player
        // 
        if (other.gameObject.GetComponent<PlayerCharacter>().playerHasAuthority){
            observerCamera.SetActive(true);
            SetGameFelloutState(false);
        }

        if(!isServer) return;
        scoring.playerFellOut(other.gameObject);

        
            // veraendere meine eigenen punkte
    }
}