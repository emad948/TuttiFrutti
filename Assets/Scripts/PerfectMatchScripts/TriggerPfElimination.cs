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

    public AudioSource playSound;
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
    }

    public IEnumerator hideOverlayWithDelay()
    {
        yield return new WaitForSeconds(2);
        gameStateUI.SetActive(false);
        FellOutGameText.gameObject.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        // Message player
        // 
        if (other.gameObject.GetComponent<PlayerCharacter>().playerHasAuthority)
        {
            playSound.Play();
            observerCamera.SetActive(true);
            SetGameFelloutState(false);
        }

        if (!isServer) return;
        if (scoring != null) scoring.playerFellOut(other.gameObject);


        // veraendere meine eigenen punkte
    }
}