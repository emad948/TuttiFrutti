using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    
    [SyncVar] public Vector3 Control;
    Rigidbody _body;

    #region Server

    public override void OnStartServer()
    {
       DontDestroyOnLoad(gameObject);
    }

    #endregion



    #region Client

    void Awake(){
        _body = gameObject.GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (hasAuthority)
        {
            
            float vInput = Input.GetAxis("Vertical");
            float hInput = Input.GetAxis("Horizontal");
            PhysicsLink link = GetComponent<PhysicsLink>();
            Control = (_body.transform.forward * vInput); //update our controll varible
            link.ApplyForce(Control, ForceMode.Force); //Use our custom force function*/
            link.Rotation = Quaternion.Euler(_body.rotation.eulerAngles + Vector3.up * hInput);
        }
         
    }

    #endregion
    
   
}
