using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using Mirror;

// Plays particles on characters feet while landing, running and walking
public class WalkingDust : MonoBehaviour
{
    public ThirdPersonController controller;
    public float MinEmissionSpeed = 4f;

    public float repeatDelay = 1f;

    private float _lastFired;
    private ParticleSystem _particles;
    private float _stdLifeTime;

    private bool _wasGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        _lastFired = Time.time;
        _particles = gameObject.GetComponentInChildren<ParticleSystem>();
        #pragma warning disable 618
        _stdLifeTime = _particles.startLifetime;
        #pragma warning restore 618
    }

    // Update is called once per frame
    void Update()
    {
        if (isLanding()) emitMoreOnLanding();
        //print("controlSpeed" +  control.speed + " controlGrounded " + control.grounded);
        if (controller.speed < MinEmissionSpeed || !controller.grounded)
        {
            _particles.Stop();
            return;
        }

        if (Time.time > _lastFired + repeatDelay)
        {
            #pragma warning disable 618
            _particles.emissionRate = Mathf.Sqrt(controller.speed);
            _particles.startLifetime = _stdLifeTime * Mathf.Sqrt(controller.speed) / Mathf.Sqrt(controller.SprintSpeed);
            _particles.startColor = new Color(0, 0, 0, Mathf.Sqrt(controller.speed) / controller.SprintSpeed);
            #pragma warning restore 618
            _particles.Play();
            _lastFired = Time.time;
            //print("PARTICLES!");
        }
    }

    private void emitMoreOnLanding()
    {
            #pragma warning disable 618
            _particles.startColor = new Color(0, 0, 0, 200);
            _particles.startLifetime = _stdLifeTime * 2;
            _particles.emissionRate = 5;
            #pragma warning restore 618
            _particles.Play();
            //_particles.startSpeed = originalSpeed;
            _lastFired = Time.time;
            return;        
    }
    private bool isLanding()
    {
        if (!controller.grounded) return _wasGrounded = false;
        if (!_wasGrounded && controller.grounded) return _wasGrounded = true;
        return false;
    }
}