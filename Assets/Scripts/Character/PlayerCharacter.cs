using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    
    
    public GameObject michelin;
   [SerializeField] private TMP_Text playerDisplayName;

   [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] private string _displayName;
   [SyncVar(hook = nameof(HandleColorUpdated))] [SerializeField] private Color color;
 
   Rigidbody _body;
   

 
   




    #region Server

    [Server] public void SetDisplayName(string newDisplayName) => _displayName = newDisplayName;
    [Server] public void SetColor(Color newColor) => color = newColor;
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
    
    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        if(!hasAuthority) playerDisplayName.text = newName;
    }

    private void HandleColorUpdated(Color oldColor, Color newColor)
    {
        if (michelin){
            foreach(var component in  michelin.GetComponentsInChildren<SkinnedMeshRenderer>()){
                component.material.color = newColor;
            }
        }

    }

    #endregion
    
   
    
   
}
