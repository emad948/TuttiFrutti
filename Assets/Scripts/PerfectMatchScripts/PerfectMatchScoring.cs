using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PerfectMatchScoring : MonoBehaviour
{
    public GlobalTime globalTime;
    //List<NetworkPlayer> players = ((GameNetworkManager)Mirror.NetworkManager.singleton).PlayersList;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void playerFellOut(GameObject player){
        // hier score von player zeitabhaengig erhoehen.
        if (globalTime.matchTime > 100){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
            //blaa
        }
    }
}
