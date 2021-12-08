using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform target;
    [SerializeField] private float verticalRotation;
    [SerializeField] private float translateSpeed;

    [SerializeField] private float rotationSpeed;
    // Start is called before the first frame update

    private void Awake(){
        
    }
    private void Update()
    {
        // todo @alex: make object with authority call this once.
        if (GameObject.FindGameObjectsWithTag("PlayerCharacter").Length >= 1) 
            target = GameObject.FindGameObjectsWithTag("PlayerCharacter")[0].transform;

        HandleTranslation();
        HandleRotation();
    }

    private void HandleTranslation(){
        var targetPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
    }
    private void HandleRotation(){
        var direction = target.position - transform.position - new Vector3(0, verticalRotation, 0);
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    } 
}




