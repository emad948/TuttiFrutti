using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ExternalForces : NetworkBehaviour
{
    [HideInInspector]
    [SyncVar] public Vector3 force = Vector3.zero;
    public float forceMult = 1f;
    public float forceDuration = 0.1f;

    public float minThreshold = 0.0001f;    
    public void addForce(Vector3 force){
        this.force += force;
        if (!isServer) CmdSyncForce(force);
    }
    [Command (requiresAuthority=false)] private void CmdSyncForce(Vector3 force){this.force = force;}
    
    void Update(){
        if (!hasAuthority) return;
        updateForce();
        if (!isServer) CmdSyncForce(force);   
    }
    void updateForce(){
        if (force.magnitude < 0.1) {
            force = Vector3.zero;
            return;
        }
        force = Vector3.Slerp(force, Vector3.zero, Time.deltaTime / forceDuration);
    }
}

