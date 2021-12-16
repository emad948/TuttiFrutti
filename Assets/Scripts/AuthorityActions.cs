using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class CameraActions : NetworkBehaviour
{

    public GameObject CameraFollowPoint;


    override public void OnStartAuthority(){
        CameraFollowPoint.SetActive(true);
        gameObject.GetComponent<PlayerInput>().enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
