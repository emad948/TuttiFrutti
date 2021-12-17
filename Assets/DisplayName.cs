using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayName : MonoBehaviour
{
    public float textRotationSpeed = 6;

    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!cam) return;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - cam.transform.position, Vector3.up), Time.deltaTime * textRotationSpeed );
    }
}
