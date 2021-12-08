using UnityEngine;
using Mirror;

public class PhysicsLink : NetworkBehaviour
{
    public Rigidbody rb;

    [SyncVar]//all the essential variables of a rigidbody
    public Vector3 Velocity;
    [SyncVar]
    public Quaternion Rotation;
    [SyncVar]
    public Vector3 Position;
    [SyncVar]
    public Vector3 AngularVelocity;

    void Update()
    {
      
        if (GetComponent<NetworkIdentity>().isServer)//if we are the server update the variables with our cubes rigidbody info
        {
            Position = rb.position;
            Rotation = rb.rotation;
            Velocity = rb.velocity;
            AngularVelocity = rb.angularVelocity;
            rb.position = Position;
            rb.rotation = Rotation;
            rb.velocity = Velocity;
            rb.angularVelocity = AngularVelocity;
        }
        if (GetComponent<NetworkIdentity>().isClient)//if we are a client update our rigidbody with the servers rigidbody info
        {
            rb.position = Position+Velocity*(float)NetworkTime.rtt;//account for the lag and update our variables
            rb.rotation = Rotation*Quaternion.Euler(AngularVelocity * (float)NetworkTime.rtt);
            rb.velocity = Velocity;
            rb.angularVelocity = AngularVelocity;
        }
    }
  
    public void ApplyForce(Vector3 force, ForceMode FMode)//apply force on the client-side to reduce the appearance of lag and then apply it on the server-side
    {
        rb.AddForce(force, FMode);
        CmdApplyForce(force, FMode);

    }
    [Command]//function that runs on server when called by a client
    public void CmdApplyForce(Vector3 force,ForceMode FMode)
    {
        rb.AddForce(force, FMode);//apply the force on the server side
    }
    
}