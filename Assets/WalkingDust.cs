using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
public class WalkingDust : MonoBehaviour
{
    public ThirdPersonController control;
    public float MinEmissionSpeed = 2f;

    public float delay = 2f;

    private float lastFired = Time.time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
//        if (control.speed > MinEmissionSpeed && Time.deltaTime > lastFired + delay ){
            gameObject.GetComponent<ParticleSystem>().Play();
            lastFired = Time.deltaTime;
  //      }
    }
}
