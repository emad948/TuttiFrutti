using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRotation : MonoBehaviour
{
    public Transform Front;
    public Transform Rear;
    public Transform Left;
    public Transform Right;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public Quaternion calculateRotation(){
        //print("casting ray");
        int layerMask = 1 << 8;
        Vector3 posFront, posRear, posLeft, posRight;
        posFront = posRear = posLeft = posRight = Vector3.zero;
        layerMask = ~layerMask;
        Quaternion rotation = Quaternion.identity;
        RaycastHit hit;
        if (Physics.Raycast(Front.position, Front.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask)){
            Debug.DrawRay(Front.position, Front.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                print(hit.collider.transform.rotation);
            posFront = hit.point;
        }
        if (Physics.Raycast(Rear.position, Rear.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask)){
            Debug.DrawRay(Rear.position, Rear.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            
            posRear = hit.point;
        }
        if (Physics.Raycast(Left.position, Left.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask)){
            Debug.DrawRay(Left.position, Left.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            
            posLeft = hit.point;
        }
        if (Physics.Raycast(Right.position, Right.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask)){
            Debug.DrawRay(Right.position, Right.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            
            posRight = hit.point;
        }

        //print(hit.collider.transform.rotation);
        
        var rotationX = Quaternion.LookRotation(posRear - posFront, Vector3.up);
        var rotationY = Quaternion.LookRotation(posRight - posLeft, Vector3.up);
        print(rotationX.eulerAngles);
        print(rotationY.eulerAngles);
        return Quaternion.Euler(rotationX.x,0,0);
    }

}
