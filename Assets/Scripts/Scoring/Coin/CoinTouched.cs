using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTouched : MonoBehaviour
{
    GameObject other;
    private void OnTriggerEnter(Collider col){
        other = col.gameObject;
        transform.parent.BroadcastMessage("touched", col.gameObject);
    }

    public void Update(){

    }

    
}
