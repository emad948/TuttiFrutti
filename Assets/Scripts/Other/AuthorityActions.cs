using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class AuthorityActions : NetworkBehaviour
{
    public GameObject CameraFollowPoint;
    public GameObject DisplayText;

    override public void OnStartAuthority()
    {
        CameraFollowPoint.SetActive(true);
        gameObject.GetComponent<PlayerInput>().enabled = true;
        //DisplayText.SetActive(false); it has been ruled, that your name is to be shown!
    }
}