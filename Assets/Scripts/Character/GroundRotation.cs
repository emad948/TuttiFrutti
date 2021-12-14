using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRotation : MonoBehaviour
{

    public float rayLength = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public float calculateRotation(){
        //print("casting ray");
        int layerMask = 1 << 8;
        Vector3 posFront;
        float rotation = 0;
        posFront  = Vector3.zero;
        layerMask = ~layerMask;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, rayLength, layerMask)){
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            
                //print(hit.collider.transform.rotation);
            rotation = Vector3.Angle(hit.normal, transform.up);
            if (rotation == 0){
                //rayLength = hit.distance;`
            }
        }
        
        return rotation;
    }

}
