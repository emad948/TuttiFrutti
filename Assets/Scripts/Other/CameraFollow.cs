using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float offsetLen;
    [SerializeField] private Transform target;
    [SerializeField] private float verticalRotation;
    [SerializeField] private float translateSpeed;

    [SerializeField] private float rotationSpeed;

    //bool mouseCatched = true;
    // Start is called before the first frame update
    private Quaternion rotation;

    private void Awake()
    {
        rotation = Quaternion.identity;
    }

    private void Update()
    {
        CalculateOffset();
        HandleRotation();
        HandleTranslation();
    }

    private void CalculateOffset()
    {
    }

    private void HandleTranslation()
    {
        transform.position = (offset + target.position);
    }

    private void HandleRotation()
    {
        var direction = new Vector3(0, verticalRotation, 0);
        rotation = Quaternion.Lerp(transform.rotation, target.rotation * Quaternion.Euler(verticalRotation, 0, 0),
            rotationSpeed);


        transform.rotation = rotation;
    }

    private void SetTransformToFollow(Transform trans)
    {
        this.target = trans;
    }
}