using System;
using Mirror;
using UnityEngine;


public class _CharacterController : NetworkBehaviour
{
    // --- Fields ---

    public Animator Anim;
    public Collider FeetCollider;
    public float Acceleration = 50f;
    public float RotSpeed = 5f;
    public GameObject freeLook;

    public GroundRotation gr;


    public float WalkSpeed = 2f;
    public float RunSpeed = 4f;
    public float MaxAngularMagnitude = 5f;
    public float RotationChangeDelay = 10;
    public float DampeningFactor = 0.25f;

    public Vector3 RotationVelocity;

    public float RotationSmoothTime;
    
    public float RotationSmoothAngle = 0.12f;
    
    // --- This is all we sync between server and clients ---
    [SyncVar] Vector3 globalPosition;
    [SyncVar] Quaternion globalRotation;

    // --- Private ---
    private GlobalTime _globalTime;
    private NetworkIdentity _identity; // is server or client?
    private Rigidbody _body; // we operate!

    private Camera _mainCamera; // rotations are relative to mainCamera

    //private bool _isGrounded = true; // for later implmentations @todo alex!
    private bool _isJumping = false, _isRunning = false; // yes yes, but we want dynamic jump range later
    private Vector2 _input = Vector2.zero; // hmm... for what would the controller need this?
    private Vector2 _inputState = new Vector2(0, 0); // we abstract a state from the last couple inputs
    private Quaternion _fallBackCameraRotation = Quaternion.identity; // if the camera ever gets too close, it won't give good reference rotation. trust me ;)

    
    private Vector2 _currentSpeed = Vector2.zero;


    override public void OnStartAuthority()
    {
        base.OnStartAuthority();
        _mainCamera = GameObject.FindObjectOfType<Camera>();
        freeLook.SetActive(true); 
    }

    private void Awake()
    {
        _identity = GetComponent<NetworkIdentity>();
        _body = gameObject.GetComponent<Rigidbody>();
        globalPosition = _body.position;
        globalRotation = _body.rotation;
        _globalTime = GameObject.FindObjectOfType<GlobalTime>();

    }

    private void Update()
    {
        if (hasAuthority)
        {
            
            if (_globalTime._time >= 0)
            handleInputs();
        }
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
            rotation = Quaternion.Euler(Vector3.SmoothDamp(rotation.eulerAngles, targetRotation.eulerAngles, ref RotationVelocity, RotationSmoothTime));
            // todo if targetRotation == 180*current then choose rotation direction
 
            // --- Velocity ON Rotation ---
            velocity = CalculateSpeed(_inputState, _body.velocity, rotation);
            //print(velocity);
            
            //print(velocity);
            // --- Move it! ---
            _body.MoveRotation(globalRotation);
            _body.AddForce(velocity - _body.velocity, ForceMode.VelocityChange);
            _body.AddForce(Vector3.down, ForceMode.Acceleration);

            // --- Animations ---
            bool walking = _input.magnitude != 0;
            Anim.SetBool("walking", walking);
            Anim.SetFloat("velocity", velocity.magnitude);
            if (_isJumping) { Anim.SetTrigger("jump"); _isJumping = false; print("jump!"); }
            Anim.SetBool("running", _isRunning);
            
            
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

    private void handleInputs(){
            _input.x = Input.GetAxis("Horizontal");
            _input.y = Input.GetAxis("Vertical");
            if (_input.magnitude == 0) applyDrag();
            _inputState += _input / RotationChangeDelay;
            if (_inputState.magnitude > 1) _inputState = _inputState.normalized;
            // _isJumping is consumed in fixedUpdate
            _isJumping = Input.GetKeyDown(KeyCode.Space) ? true : _isJumping;
            _isRunning = Input.GetKey(KeyCode.LeftShift);
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
        var lookDir = _body.transform.position - _mainCamera.transform.position;
        lookDir.y = 0;
        return  Vector3.forward * inputs.magnitude * Time.deltaTime * Acceleration;
    }
    private Vector3 CalculateSpeed(Vector2 inputs, Vector3 currentSpeed, Quaternion rotation)
    {
        var maxSpeed = _isRunning ? RunSpeed : WalkSpeed;
        //print(gr.calculateRotation());
        Vector3 unclampedSpeed = rotation* Quaternion.Euler(-gr.calculateRotation(), 0,0) *  CalculateAcceleration(inputs, rotation) + _body.velocity ;
        Vector3 horizontalSpeed = Vector3.ClampMagnitude(new Vector3(unclampedSpeed.x, 0, unclampedSpeed.z), maxSpeed);
        Vector3 verticalSpeed = Vector3.ClampMagnitude(Vector3.Scale(unclampedSpeed, Vector3.up), maxSpeed);
        return verticalSpeed + horizontalSpeed;
    }

    void updateLocally(Vector3 pos, Quaternion rot)
    {
        globalPosition = pos;
        globalRotation = rot;
    }
    // Updates globales on server instance:
    [Command] void updateOnServer(Vector3 pos, Quaternion rot) => updateLocally(pos, rot);
}