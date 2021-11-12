using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerDisplayName;
    [SerializeField] private Renderer displayColorRenderer;
    
    //SyncVar are only Updated on the server
    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField] private string displayName = "Missing Name";
    [SyncVar(hook = nameof(HandleDisplayColorUpdated))] [SerializeField] private Color color;
    [SyncVar] public Vector3 Control;
    
    #region Server
    [Server] public void SetDisplayName(string newDisplayName) => displayName = newDisplayName;
    [Server] public void SetColor(Color newColor) => color = newColor;
    
    #endregion

    #region Client

    private void HandleDisplayColorUpdated(Color oldColor, Color newColor)=>displayColorRenderer.material.SetColor("_BaseColor",newColor);
    
    private void HandleDisplayNameUpdated(string oldName, string newName) => playerDisplayName.text = newName;


    [ClientRpc]
    private void Update()
    {
        Control = new Vector3(Input.GetAxis("Horizontal") * .2f, 0,
            Input.GetAxis("Vertical") * .2f); //update our controll varible
        GetComponent<PhysicsLink>().ApplyForce(Control, ForceMode.VelocityChange); //Use our custom force function
    }

    #endregion

}
