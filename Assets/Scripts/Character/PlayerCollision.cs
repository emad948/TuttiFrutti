using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
public class PlayerCollision : NetworkBehaviour{
  
    void OnControllerColliderHit(ControllerColliderHit hit){
        if (!hasAuthority) return; // not optimal for many players: use disabled gameObject instead with AuthorityManager
        var target = hit.gameObject;
        //print(other.tag);
        if (target.tag == "PlayerCharacter"){
            ExternalForces.singleton.CmdPushCharacter(target, gameObject);
         }
    }
}