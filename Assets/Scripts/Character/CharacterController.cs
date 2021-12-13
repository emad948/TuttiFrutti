using System;
using Mirror;
using UnityEngine;


public class CharacterController : NetworkBehaviour
{
    // --- Fields ---
    public float Acceleration = 0.0001f;
    public float RotSpeed = 5f;

    public Animator Anim;
    public float MaxVelocity = 1.5f;
    public float MaxAngularMagnitude = 5f;
    public float ReverseDelay = 10;
    public float DampeningFactor = 0.25f;
    // --- This is all we sync between server and clients ---
    [SyncVar] Vector3 globalPosition;
    [SyncVar] Quaternion globalRotation;


    // --- Private ---
    private NetworkIdentity _identity; // is server or client?
    private Rigidbody _body; // we operate!

    private Camera _mainCamera; // rotations are relative to mainCamera

    //private bool _isGrounded = true; // for later implmentations @todo alex!
    private bool _isJumping = false; // yes yes, but we want dynamic jump range later
    private Vector2 _input = Vector2.zero; // hmm... for what would the controller need this?
    private Vector2 _inputState = new Vector2(0, 0); // we abstract a state from the last couple inputs
    private Quaternion _fallBackCameraRotation = Quaternion.identity; // if the camera ever gets too close, it won't give good reference rotation. trust me ;)

    private Vector2 _currentSpeed = Vector2.zero;


    override public void OnStartAuthority()
    {
        base.OnStartAuthority();
        _mainCamera = GameObject.FindObjectOfType<Camera>();
    }

    private void Awake()
    {
        _identity = GetComponent<NetworkIdentity>();
        _body = gameObject.GetComponent<Rigidbody>();
        globalPosition = _body.position;
        globalRotation = _body.rotation;

    }


    private void Update()
    {
        if (hasAuthority)
        {
            _input.y = Input.GetAxis("Vertical");
            _input.x = Input.GetAxis("Horizontal");
            if (_input.magnitude == 0) applyDrag();
            _inputState += _input / ReverseDelay;
            if (_inputState.magnitude > 1) _inputState = _inputState.normalized;
            
            // _isJumping is consumed in fixedUpdate
            _isJumping = Input.GetKeyDown(KeyCode.Space) ? true : _isJumping;
        }
    }

    private void applyDrag(){
            if (_inputState.magnitude < DampeningFactor) _inputState = Vector2.zero;
            else                                         _inputState = _inputState - _inputState.normalized * DampeningFactor;
            
    }
    private Vector3 yRotationFromInput(Vector2 input)
    {
        float padToDeg(Vector2 pad)
        {
            float v = pad.y * Math.Abs(pad.y);
            float h = pad.x * Math.Abs(pad.x);
            if (v > 0) v = 0;
            if (v < 0 && h > 0) v *= -1;

            return h * 90 + v * 180;
        }

        return Vector3.up * padToDeg(input);
    }

    private Vector3 CalculateAcceleration(Vector2 inputs, Quaternion rotation)
    {
        float test = Time.fixedDeltaTime;
        var lookDir = _body.transform.position - _mainCamera.transform.position;
        lookDir.y = 0;
        return rotation * Vector3.forward * inputs.magnitude * Time.deltaTime * Acceleration;
    }
    private Vector3 CalculateSpeed(Vector2 inputs, Vector3 currentSpeed, Quaternion rotation)
    {
        // input.x = horizontal, input.y = vertical
        return Vector3.Scale(Vector3.up, _body.velocity) + Vector3.ClampMagnitude(_body.velocity + CalculateAcceleration(inputs, rotation), MaxVelocity);
    }

    private void FixedUpdate()
    {
        // --- Variables ---
        Vector3 position = _body.position;
        Quaternion rotation = _body.rotation;
        Vector3 velocity = _body.velocity;
        if (hasAuthority || _identity == null) // identy unset => local
        {

            // --- Rotation ---
            var localLookDir = _body.position - _mainCamera.transform.position;
            localLookDir.y = 0; // we don't care about vertical rotation here
            var lookRotation = Quaternion.LookRotation(localLookDir);
            if (localLookDir.magnitude < 0.5f) lookRotation = _fallBackCameraRotation;
            else _fallBackCameraRotation = lookRotation;
            Quaternion targetRotation = rotation;
            if (_input.magnitude > 0.1f) targetRotation = lookRotation * Quaternion.Euler(yRotationFromInput(_inputState));
            if (Math.Abs(Quaternion.Angle(rotation, targetRotation)) <= 95)
                rotation = Quaternion.Lerp(rotation, targetRotation, RotSpeed * Time.fixedDeltaTime);
            else // Instant rotation if rotating more than 90 deg
                rotation = targetRotation;

            // --- Velocity ON Rotation ---
            velocity = CalculateSpeed(_inputState, _body.velocity, rotation); ;

            // --- Move it! ---
            _body.MoveRotation(globalRotation);
            _body.velocity = velocity;

            // --- Animations ---
            bool walking = _input.magnitude != 0;
            Anim.SetBool("walking", walking);
            Anim.SetFloat("walkSpeed", velocity.magnitude);
            if (_isJumping) { Anim.SetTrigger("jump"); _isJumping = false; print("jump!"); }
            
            // --- Syncing ---
            updateLocally(_body.position, rotation);
            if (!_identity.isServer && _identity != null) updateOnServer(_body.position, rotation);

        }
        else
        { // controlled by other player | todo @alex: improve smoothing
            _body.position = Vector3.Lerp(_body.position, globalPosition, Time.fixedDeltaTime * 100);
            _body.rotation = Quaternion.Lerp(_body.rotation, globalRotation, Time.fixedDeltaTime * 100);
        }

    }

    void updateLocally(Vector3 pos, Quaternion rot)
    {
        globalPosition = pos;
        globalRotation = rot;
    }
    // Updates globales on server instance:
    [Command] void updateOnServer(Vector3 pos, Quaternion rot) => updateLocally(pos, rot);
}