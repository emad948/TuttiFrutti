using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    
    Rigidbody _body;

    #region Server

    public override void OnStartServer()
    {
       DontDestroyOnLoad(gameObject);
    }

    #endregion



    #region Client

    override public void OnStartAuthority(){
        base.OnStartAuthority();
            GameObject.FindGameObjectWithTag("MainCamera").BroadcastMessage("SetTransformToFollow", gameObject.transform);
    }


    #endregion
    
   
}
