using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class CharacterController : NetworkBehaviour
{

    public float Speed = 5f;
    public float rotSpeed = 10f;

    private float vInput = 0;
    private float hInput = 0;

    private Quaternion rotation = Quaternion.Euler(Vector3.forward);
    private Rigidbody _body;
    private bool isGrounded = true;

    [SyncVar] Vector3 globalPosition;
    [SyncVar] Quaternion globalRotation;
    private NetworkIdentity identity;

    private void Awake()
    {
        identity = GetComponent<NetworkIdentity>();
        _body = gameObject.GetComponent<Rigidbody>();
        globalPosition = _body.position;
        globalRotation = _body.rotation;

    }


    private void Update(){
        if (hasAuthority)
        {
            vInput = Input.GetAxis("Vertical");
            hInput = Input.GetAxis("Horizontal");
        }
    }

    private void FixedUpdate()
    {

        Vector3 position = globalPosition;
        Quaternion rotation = globalRotation;
        if (hasAuthority)
        {
            position = Vector3.Lerp(_body.position, _body.position + _body.transform.forward * vInput * Speed, Time.fixedDeltaTime);
            rotation = Quaternion.Euler(_body.rotation.eulerAngles + Vector3.up * hInput * rotSpeed);
            updateLocally(position, rotation);
            updateOnServer(position, rotation);
        }

        _body.MovePosition(globalPosition);
        _body.rotation = (globalRotation);

    }

    void updateLocally(Vector3 pos, Quaternion rot)
    {
        globalPosition = pos;
        globalRotation = rot;
        print("updating on client");
    }
    [Command]
    void updateOnServer(Vector3 pos, Quaternion rot) => updateLocally(pos, rot);


}
