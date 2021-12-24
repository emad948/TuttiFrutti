using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ExternalForces : NetworkBehaviour
{
    private static ExternalForces identity;
    public static ExternalForces singleton{get{
        if (identity == null) identity = new ExternalForces();
        
        return identity;
    }
    }
    private ExternalForces(){
        Start();
    }


    private readonly SyncList<Force> globalForces = new SyncList<Force>();
    public float forceMult = 0.01f;
    public float timeMult = 100;

    public float minThreshold = 0.0001f;
    public Vector3 force(GameObject g){
        Vector3 result = Vector3.zero;
        foreach (Force f in globalForces){
             if (f.target == g)
             result += f.current;
        }
        return result;
    }

    void Awake(){
        DontDestroyOnLoad(gameObject);
        Start();
    }
    void Start(){
        identity = this;
    }
    [Command (requiresAuthority=false)] public void CmdPushCharacter(GameObject target, GameObject source){
        Vector3 sourceSpeed = source.transform.rotation * Vector3.forward * forceMult;
        Force newForce = new Force(sourceSpeed, target);
        globalForces.Add(newForce);
    } 
    
    void Update(){
        if (!isServer) return;
        foreach (Force force in globalForces){
            if (force.final.magnitude - force.current.magnitude < minThreshold){
                 globalForces.Remove(force);
                 Update();
                 return;
            }
            force.current = Vector3.Lerp(force.current, force.final, Time.deltaTime * timeMult);
            if (globalForces.Count > 0) print(globalForces.ToString());
        }
        
    }
    
    private class Force {
        public Force(Vector3 force, GameObject target){
            final = force;
            current = Vector3.zero;
        }

        public Force(){
            final = current = Vector3.zero;
            target = null;
        }
        public Vector3 final;
        public Vector3 current;
        public NetworkTransform target;
        public string toString(){
            return current + " " + final + " " + target.tag;
        }
    }
}

