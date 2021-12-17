using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
public class AuthorityActions : NetworkBehaviour
{
    public GameObject CameraFollowPoint;
    override public void OnStartAuthority(){
        CameraFollowPoint.SetActive(true);
        gameObject.GetComponent<PlayerInput>().enabled = true;
    }
}
