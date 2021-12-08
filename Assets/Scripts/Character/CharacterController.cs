using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class CharacterController : NetworkBehaviour
{

    [SerializeField] private float accFactor = 1f;
    [SerializeField] private float rotFactor = 1f;
    public Vector3 currentVelocity = Vector3.zero;
    public Quaternion currentRotation = Quaternion.identity;

    public float Speed = 5f;
    public float rotSpeed = 1f;
    
    private float vInput = 0;
    private float hInput = 0;

    private Quaternion rotation = Quaternion.Euler(Vector3.forward);
    private Rigidbody _body;
    private bool isGrounded = true;

private void Awake(){
    _body = gameObject.GetComponent<Rigidbody>();
}

private void Update(){
    vInput = Input.GetAxis("Vertical");
    hInput = Input.GetAxis("Horizontal");
}
private void FixedUpdate(){
    if(hasAuthority){
    _body.MovePosition(Vector3.Lerp(_body.position, _body.position + _body.transform.forward * vInput * Speed, Time.fixedDeltaTime));
    _body.MoveRotation(Quaternion.Euler(_body.rotation.eulerAngles + Vector3.up * hInput));
    }
}
}