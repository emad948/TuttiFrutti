using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
// detects if you hit another player
public class PlayerCollision : NetworkBehaviour{
    void OnControllerColliderHit(ControllerColliderHit hit){
        if (!hasAuthority) return; // not optimal for many players: use disabled gameObject instead with AuthorityManager
        var target = hit.gameObject;
        //print(other.tag);
        if (target.tag == "PlayerCharacter"){
            Vector3 pushDirection = (hit.point - transform.position);
            pushDirection.y = 0;
            pushDirection = pushDirection.normalized;
            target.GetComponent<ExternalForces>().addForce(pushDirection);
            print("add force");
         }
    }
}