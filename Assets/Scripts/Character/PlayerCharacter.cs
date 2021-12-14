using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    
    
   [SerializeField] private TMP_Text playerDisplayName;

   public string displayName;
   Rigidbody _body;


private void Start()
{
    if (!hasAuthority)playerDisplayName.text = displayName;
       
}



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
