using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetCollider : MonoBehaviour
{
    public Transform cube;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnCollisionStay(Collision collision){
        OnCollisionEnter(collision);
    }
    void OnCollisionEnter(Collision collision){
           //print(collision.transform.rotation);
           //Quaternion groundRotation = collision.contacts[0].otherCollider.rotation;
           //print(groundRotation);
    }
}
