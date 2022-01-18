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

    [SyncVar] public float forceDuration = 0.035f;  
    public void addForce(Vector3 force, float duration){
        this.force += force;
        this.forceDuration = duration;
        if (!isServer) CmdSyncForce(force, duration);
    }
    [Command (requiresAuthority=false)] private void CmdSyncForce(Vector3 force, float duration){this.force = force; this.forceDuration = duration;}
    
    void Update(){
        if (!hasAuthority) return;
        fadeForce();
        if (!isServer) CmdSyncForce(force, forceDuration);   
    }

    void fadeForce(){
        if (force.magnitude < 0.01) {
            force = Vector3.zero;
            return;
        }
        force = Vector3.Slerp(force, Vector3.zero, Time.deltaTime / forceDuration);
    }
}

