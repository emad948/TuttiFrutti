using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPfElimination : MonoBehaviour
{

    public PerfectMatchScoring scoring;
    // Start is called before the first frame update

    void OnTriggerEnter(Collider other){
        if(!other.gameObject.GetComponent<PlayerCharacter>().playerHasAuthority) return;
        print("hasAuthority and fell out!");
        scoring.playerFellOut(other.gameObject);
            // veraendere meine eigenen punkte
    }
}
