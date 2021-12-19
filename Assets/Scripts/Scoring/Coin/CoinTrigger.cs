using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTrigger : MonoBehaviour
{
    private bool wasTouched = false;
    private void OnTriggerEnter(Collider col){
        if (wasTouched) return;
        wasTouched = true;
        transform.parent.parent.BroadcastMessage("touched", col.gameObject);
    }
   
}
