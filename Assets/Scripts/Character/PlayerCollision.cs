using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
// detects if you hit another player
public class PlayerCollision : NetworkBehaviour{
    public float _pushStrength = 1f; 
    public float runMultiplier = 1000f;
    private StarterAssets.ThirdPersonController controller;

    void Start(){
        controller = GetComponent<StarterAssets.ThirdPersonController>();
    }
    void OnControllerColliderHit(ControllerColliderHit hit){
        if (!hasAuthority) return; // not optimal for many players: use disabled gameObject instead with AuthorityManager
        var target = hit.gameObject;
        if (target.tag == "PlayerCharacter"){
            var multiplier = _pushStrength;
            if (controller.inputRunning) {
                multiplier *= runMultiplier;
                target.GetComponent<ExternalForces>().forceDuration = 0.5f;
            }
            else target.GetComponent<ExternalForces>().forceDuration = 0.1f;

            Vector3 pushDirection = (hit.point - transform.position);
            pushDirection.y = 0;
            pushDirection = pushDirection.normalized;
            target.GetComponent<ExternalForces>().addForce(pushDirection * multiplier);
         }
    }
}