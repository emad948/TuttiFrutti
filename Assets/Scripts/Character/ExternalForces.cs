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


    [SyncVar] ArrayList globalForces;
    [SyncVar] ArrayList forces;
    public float forceMult = 0.01f;
    public float timeMult = 100;

    public float minThreshold = 0.0001f;
    public Vector3 force(GameObject g){
        Vector3 result = Vector3.zero;
        foreach (Force f in forces){
            //if (f.target == g)
             result += f.current;
        }
        return result;
    }

    void Awake(){
        DontDestroyOnLoad(gameObject);
    }
    void Start(){
        forces = new ArrayList();
        identity = this;
    }
    [Command] void AddOnServer(Force force){forces.Add(force);print("force added on server by client");}

    private void AddForce(Force force){
        if (isServer) forces.Add(force);
        else AddOnServer(force);
    }
    public void pushCharacter(GameObject target, GameObject source){
        Vector3 sourceSpeed = source.transform.rotation * Vector3.forward * forceMult;
        Force newForce = new Force(sourceSpeed, target);
        AddForce(newForce);

    } 
    
    void Update(){
        foreach (Force force in forces){
            if (force.final.magnitude - force.current.magnitude < minThreshold){
                 forces.Remove(force);
                 Update();
                 return;
            }
            force.current = Vector3.Lerp(force.current, force.final, Time.deltaTime * timeMult);
            if (forces.Count > 0) print(forces);
                    globalForces = forces;
        }
    }

    
    private class Force{
        public Force(Vector3 force, GameObject target){
            final = force;
            current = Vector3.zero;
        }

        public Force(){
            final = current = Vector3.zero;
            target = null;
        }
        public Vector3 final{get; set;}
        public Vector3 current{get; set;}

        public GameObject target;
        public string toString(){
            return current + " " + final + " " + target.tag;
        }
    }
}

