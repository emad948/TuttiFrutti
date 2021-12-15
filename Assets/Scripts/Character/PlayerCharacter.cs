using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    
    public Color color;
    public GameObject michelin;
   [SerializeField] private TMP_Text playerDisplayName;

   public string displayName;
   Rigidbody _body;


private void Start()
{
    if (!hasAuthority)playerDisplayName.text = displayName;
    setColor(color);
}

public void setColor(Color color){
        if (color != null && michelin){
        foreach(var component in  michelin.GetComponentsInChildren<SkinnedMeshRenderer>()){
            component.material.color = color;
        }
    }
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
