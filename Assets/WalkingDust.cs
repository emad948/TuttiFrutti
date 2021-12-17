using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Mirror;
public class WalkingDust : MonoBehaviour
{
    public ThirdPersonController control;
    public float MinEmissionSpeed = 4f;

    public float delay = 1f;

    private float _lastFired;
    private ParticleSystem _particles;
    private float _stdLifeTime;
    // Start is called before the first frame update
    void Start()
    {
        _lastFired = Time.time;
        _particles = gameObject.GetComponentInChildren<ParticleSystem>();
        _stdLifeTime = _particles.startLifetime;
    }

    // Update is called once per frame
    void Update()
    {
        //print("controlSpeed" +  control.speed + " controlGrounded " + control.grounded);
         if (control.speed < MinEmissionSpeed || !control.grounded){
            _particles.Stop();
            return;
        }
        if (Time.time > _lastFired + delay){
            _particles.emissionRate = Mathf.Sqrt(control.speed);
            _particles.startLifetime = _stdLifeTime * Mathf.Sqrt(control.speed) / Mathf.Sqrt(control.SprintSpeed);
            _particles.startColor  = new Color(0,0,0,Mathf.Sqrt(control.speed)  / control.SprintSpeed);
            _particles.Play();
            _lastFired = Time.time;
            //print("PARTICLES!");
        }

        
    }
}
