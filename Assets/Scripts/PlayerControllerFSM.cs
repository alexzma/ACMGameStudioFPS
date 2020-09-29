using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;
//FSM for all different states of character
public class PlayerControllerFSM :  NetworkBehaviour
{    
    public CameraLook playerCam;
    public Transform orientation;
    public Animator charAnimator;
    //public Animator localAnimator;
    public Rigidbody playerRb;
    private CapsuleCollider playerCapCollider;
    public JetpackEffect jetpackfire;
    //public NetworkIdentity networkIdentity;
    //public SkinnedMeshRenderer remoteSkin;
    //public GameObject outerModel;
    public KeyListener menuCheck;

    private float xRotation;
    private float sensitivity = 50f;

    public float moveSpeed = 2000f;
    public float jetPackFuel = 2f;
    public bool isGrounded;
    public LayerMask whatIsGround;
    //public Transform groundCheck;
    public float groundDistance = 0.8f;
    private bool shouldRefuel;

    private float counterMovement = 0.375f;
    private float threshold = 0.01f;
    public float floorAngle;
    public float maxSlopeAngle = 55f;

    private Vector3 diveScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;    

    private float jumpCooldown = 0.25f;
    public float jumpForce = 10f;

    float x;
    float y;
    bool pressedJump;
    bool sprinting;
    bool diving;


    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    private float mouseX;
    private float mouseY;
    //public float mouseSensitivity;

    private Vector3 rayVector;
    private bool hitStairs;
    private Vector3 forward;
    private Vector3 right;
    RaycastHit hitInfo;
    private Vector3 offset = new Vector3(0, 1.75f, 0);
    private bool hitSomething;
    private bool reload;

    private PlayerBaseState currentState;

    public readonly PlayerWalkingState WalkState = new PlayerWalkingState();
    public readonly PlayerJumpState JumpState = new PlayerJumpState();
    public readonly PlayerProneState ProneState = new PlayerProneState();
    public readonly PlayerDiveState DiveState = new PlayerDiveState();
    public readonly PlayerFlyState FlyState = new PlayerFlyState();

    #region Accessors
    //accessors
    public float XRotation
    {
        get { return xRotation; }
        set { xRotation = value; }
    }
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    public float Threshold
    {
        get { return threshold; }
        set { threshold = value; }
    }
    public Vector3 DiveScale
    {
        get { return diveScale; }
        set { diveScale = value; }
    }
    public Vector3 PlayerScale
    {
        get { return playerScale; }
        set { playerScale = value; }
    }
    public float JumpCoolDown
    {
        get { return jumpCooldown; }
        set { jumpCooldown = value; }
    }
    public float X
    {
        get { return x; }
        set { x = value; }
    }
    public float Y
    {
        get { return y; }
        set { y = value; }
    }
    public bool PressedJump
    {
        get { return pressedJump; }
        set { pressedJump = value; }
    }
    public bool Sprinting
    {
        get { return sprinting; }
        set { sprinting = value; }
    }
    public bool Diving
    {
        get { return diving; }
        set { diving = value; }
    }
    public bool ShouldRefuel
    {
        get { return shouldRefuel; }
        set { shouldRefuel = value; }
    }
    public PlayerBaseState CurrentState
    {
        get { return currentState; }
    }
    public CapsuleCollider PlayerCapCollider
    {
        get { return playerCapCollider; }
        set { playerCapCollider = value; }
    }
    public RaycastHit HitInfo
    {
        get { return hitInfo;}
    }
    #endregion 

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCapCollider = GetComponent<CapsuleCollider>();
        //networkIdentity = GetComponent<NetworkIdentity>();
        //NetworkAnimator animator = new NetworkAnimator();
        //animator.animator = localAnimator;
    }
    private void Start()
    {
        jetPackFuel = 2;
        TransitionToState(WalkState);
        playerScale = transform.localScale;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;        
        //if (networkIdentity.isLocalPlayer)
        //   remoteSkin.enabled = false;
        menuCheck = GameObject.Find("Canvas(Clone)").GetComponent<KeyListener>();
    }
    // Update is called once per frame
    void Update()
    {
        if (menuCheck.menuShowing == true)
        {
            playerCam.look = false;
            x = 0;
            y = 0;
            reload = false;
        }
        else
        {
            playerCam.look = true;
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
            reload = Input.GetKeyDown(KeyCode.R);
        }
            //Look();
            if (reload)
            {
                charAnimator.SetBool("Reload", true);
                Debug.Log("Reload");
            }
            else
            {
                charAnimator.SetBool("Reload", false);
            }
            if (isGrounded)
            {
                CheckGround();
            }
            else
            {//always use gravity when not grounded
                playerRb.useGravity = true;
            }
            if (y > 0)
            {
                charAnimator.SetBool("WalkingForward", true);
            }
            else if (y < 0)
            {
                charAnimator.SetBool("WalkingBackward", true);
            }
            else
            {
                charAnimator.SetBool("WalkingForward", false);
                charAnimator.SetBool("WalkingBackward", false);
            }
            if (x > 0)
            {
                charAnimator.SetBool("StrafeRight", true);
            }
            else if (x < 0)
            {
                charAnimator.SetBool("StrafeLeft", true);
            }
            else
            {
                charAnimator.SetBool("StrafeRight", false);
                charAnimator.SetBool("StrafeLeft", false);
            }
            currentState.Update(this);
        
        //refuel jetpack in any state slowly
        if (shouldRefuel)
        {
            if (jetPackFuel < 2f)
                jetPackFuel += Time.deltaTime / 6.0f;
            else
                jetPackFuel = 2f;
        }
    }    
    private void FixedUpdate()
    {
        if (menuCheck.menuShowing == true)
        {
            playerCam.look = false;
        }
        else
        {
            playerCam.look = true;
            if (isGrounded)
            {
                CalculateForward();
                drawDebugLines();
            }
            else
            {
                forward = orientation.transform.forward;
                right = orientation.transform.right;
            }
            currentState.FixedUpdate();
        }
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, whatIsGround);
    }    
    private void OnCollisionEnter(Collision other)
    {
        currentState.OnCollisionEnter(this, other);
    }
    private void OnTriggerStay(Collider other)
    {
        currentState.OnTriggerStay(other);
        if (other.CompareTag("Wind"))
        {
            playerRb.AddForce(0, 4, 0, ForceMode.Impulse);
        }
    }
    public void TransitionToState(PlayerBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
    public void StartChildRoutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
    public void StopGrounded()
    {
        isGrounded = false;
    }
    public void Movement(float maxSpeed)
    {
        //velocity relative to direction
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //friction
        addFriction(x, y, mag);

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Apply forces to move player
        playerRb.AddForce(forward * y * moveSpeed * Time.deltaTime);
        playerRb.AddForce(right * x * moveSpeed * Time.deltaTime);
    }
    public void addFriction(float x, float y, Vector2 mag)
    {
        if (!isGrounded || PressedJump)
        {
            return;
        }
        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            playerRb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            playerRb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

    }
    // Find the velocity direction
    // used for vectors calculations on movement and friction
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(playerRb.velocity.x, playerRb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = playerRb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }
    //check if collided with a floor and the angle is walkable
    public bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        floorAngle = angle;
        return angle < maxSlopeAngle;
    }
    void CalculateForward()
    {
        if (!isGrounded || !hitSomething)
        {
            forward = orientation.transform.forward;
            right = orientation.transform.right;
            return;
        }
        if (IsFloor(hitInfo.normal))
        {
            //Vector3 newDirection = new Vector3(playerRb.velocity.z, playerRb.velocity.y, -playerRb.velocity.x);
            forward = Vector3.Cross(transform.right, hitInfo.normal);
            right = Vector3.Cross(hitInfo.normal, transform.forward);
        }
        
    }
    void CheckGround()
    {        
        rayVector = new Vector3(0, -2f, 0f);
        if (y > 0)
            rayVector += Vector3.forward;
        if (y < 0)
            rayVector += Vector3.back;
        if (x > 0)
            rayVector += Vector3.right;
        if (x < 0)
            rayVector += Vector3.left;
        rayVector = transform.TransformVector(rayVector);
        hitSomething = Physics.Raycast(transform.position + offset, rayVector, out hitInfo, 2.25f, whatIsGround);
    }
    public void drawDebugLines()
    {
        Debug.DrawLine(transform.position + offset, transform.position + offset + forward , Color.blue);
        Debug.DrawLine(transform.position + offset, transform.position + offset + rayVector, Color.green);
    }
    public void Respawn()
    {
        
    }
}
