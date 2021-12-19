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
        if (isLanding())
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

        //print("controlSpeed" +  control.speed + " controlGrounded " + control.grounded);
        if (control.speed < MinEmissionSpeed || !control.grounded)
        {
            _particles.Stop();
            return;
        }

        if (Time.time > _lastFired + delay)
        {
            #pragma warning disable 618
            _particles.emissionRate = Mathf.Sqrt(control.speed);
            _particles.startLifetime = _stdLifeTime * Mathf.Sqrt(control.speed) / Mathf.Sqrt(control.SprintSpeed);
            _particles.startColor = new Color(0, 0, 0, Mathf.Sqrt(control.speed) / control.SprintSpeed);
            #pragma warning restore 618
            _particles.Play();
            _lastFired = Time.time;
            //print("PARTICLES!");
        }
    }

    private bool isLanding()
    {
        if (!control.grounded) return _wasGrounded = false;
        if (!_wasGrounded && control.grounded) return _wasGrounded = true;
        return false;
    }
}