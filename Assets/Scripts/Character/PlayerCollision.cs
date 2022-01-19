using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// detects if you hit another player
public class PlayerCollision : NetworkBehaviour
{
    public float _pushStrength = 0.1f;
    private StarterAssets.ThirdPersonController controller;
    [SyncVar] public bool hasCrown = false;
    private bool hadCrown = false;
    private bool isCrownLevel = false;

    public GameObject crown;

    void Start()
    {
        controller = GetComponent<StarterAssets.ThirdPersonController>();
        if (SceneManager.GetActiveScene().name == "Level_Crown") isCrownLevel = true;
    }
    void Update()
    {
        if (hadCrown != hasCrown){
            crown.SetActive(hasCrown);
            hadCrown = hasCrown;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hasAuthority)
            return; // not optimal for many players: use disabled gameObject instead with AuthorityManager
        var target = hit.gameObject;
        if (target.tag == "PlayerCharacter")
        {
            if (controller.inputRunning){
                var multiplier = _pushStrength;
                var duration = 0.15f;

                Vector3 pushDirection = (hit.point - transform.position);
                pushDirection.y = 0;
                pushDirection = pushDirection.normalized;

                target.GetComponent<ExternalForces>().addForce(pushDirection * multiplier, duration);
            }

            // --- the following is for crowning
            if (isCrownLevel)
            {
                var cr = target.GetComponent<PlayerCollision>().hasCrown;
                if (cr)
                {
                    target.GetComponent<PlayerCollision>().hasCrown = hasCrown;                
                    hasCrown = cr;
                    if (!isServer){
                        syncCrown(hasCrown);
                        target.GetComponent<PlayerCollision>().syncCrown(target.GetComponent<PlayerCollision>().hasCrown);
                    }
                }
                
            }
        }
    }
    [Command (requiresAuthority=false)] public void syncCrown(bool hasCrown){
        this.hasCrown = hasCrown;
    }
    
}