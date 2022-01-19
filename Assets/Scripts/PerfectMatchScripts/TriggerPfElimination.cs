using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class TriggerPfElimination : NetworkBehaviour
{

    public GameObject globalObserverCamera;

    public PerfectMatchScoring scoring;
    // Start is called before the first frame update

    void OnTriggerEnter(Collider other){
        // Message player
        if(other.gameObject.GetComponent<PlayerCharacter>().playerHasAuthority) globalObserverCamera.SetActive(true);
            
        // 
        if(!isServer) return;
        scoring.playerFellOut(other.gameObject);
            // veraendere meine eigenen punkte
    }
}