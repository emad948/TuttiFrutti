using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightboxAnimator : MonoBehaviour
{
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponentInChildren<Renderer>().material;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        OnTriggerStay(other);
    }
    void OnTriggerStay(Collider other){
        
        mat.SetVector("playerPosition", transform.InverseTransformPoint(other.transform.position));
    }
    void OnTriggerLeave(Collider other){
        mat.SetVector("playerPosition", Vector3.zero);
    }
}
