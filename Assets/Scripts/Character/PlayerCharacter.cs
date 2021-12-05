using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    
    [SyncVar] public Vector3 Control;

    
    #region Server

    public override void OnStartServer()
    {
       DontDestroyOnLoad(gameObject);
    }

    #endregion



    #region Client

    private void Update()
    {
        if (hasAuthority)
        {
            Control = new Vector3(Input.GetAxis("Horizontal") * .2f, 0,
                Input.GetAxis("Vertical") * .2f); //update our controll varible
            GetComponent<PhysicsLink>().ApplyForce(Control, ForceMode.VelocityChange); //Use our custom force function
        }
         
    }

    #endregion
    
   
}
