using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    private List<string> bottomComponents = new List<string>()
    {
        "footL", "footR", "kneecapL", "kneecapR", "lowerLegL", "lowerLegR", "shinL", "shinR", "thighJointL",
        "thighJointR", "thighL", "thighR", "footL", "footR", "ankleL", "ankleR", "hips"
    };

    private List<string> topComponents = new List<string>()
    {
        "elbowL.001", "elbowR", "handL", "handR", "lowerArmL", "lowerArmR", "schoulderL", "shoulderR", "upperArmL",
        "upperArmR", "wristL", "wristR", "head"
    };

    private List<string> middleComponents = new List<string>() {"lowerBreast", "stomach",};

    public GameObject michelin;
    [SerializeField] private TMP_Text playerDisplayName;

    [SyncVar(hook = nameof(HandleDisplayNameUpdated))] [SerializeField]
    private string _displayName;

    [SyncVar(hook = nameof(HandleColorUpdated_T))] [SerializeField]
    private Color color_T;

    [SyncVar(hook = nameof(HandleColorUpdated_M))] [SerializeField]
    private Color color_M;

    [SyncVar(hook = nameof(HandleColorUpdated_B))] [SerializeField]
    private Color color_B;

    Rigidbody _body;

    #region Server

    [Server]
    public void SetDisplayName(string newDisplayName) => _displayName = newDisplayName;

    [Server]
    public void SetColor(Color newColor_T, Color newColor_M, Color newColor_B)
    {
        color_T = newColor_T;
        color_M = newColor_M;
        color_B = newColor_B;
    }

    #endregion


    #region Client

    private void HandleDisplayNameUpdated(string oldName, string newName)
    {
        playerDisplayName.text = newName;
    }

    private void HandleColorUpdated_T(Color oldColor, Color newColor)
    {
        if (michelin)
        {
            foreach (var component in michelin.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var n = component.name;
                if (topComponents.Contains(n))
                {
                    component.material.color = newColor;
                }
            }
        }
    }

    private void HandleColorUpdated_M(Color oldColor, Color newColor)
    {
        if (michelin)
        {
            foreach (var component in michelin.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var n = component.name;
                if (middleComponents.Contains(n))
                {
                    component.material.color = newColor;
                }
            }
        }
    }

    private void HandleColorUpdated_B(Color oldColor, Color newColor)
    {
        if (michelin)
        {
            foreach (var component in michelin.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                var n = component.name;
                if (bottomComponents.Contains(n))
                {
                    component.material.color = newColor;
                }
            }
        }
    }

    public bool playerHasAuthority
    {
        get { return hasAuthority; }
    }

    #endregion
}