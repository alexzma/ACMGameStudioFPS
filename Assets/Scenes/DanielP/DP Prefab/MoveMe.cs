using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class MoveMe : NetworkBehaviour
{
    
    private CharacterController _controller;
    public float speed = 20;
    public float power = 1.05f;
    public float gravity = -20;
    public float groundDistance = 1.1f;
    public float max_up = 10;
    public float jump = 20;
    public float outTripRadius = 5;
    private GameObject _groundChecker;
    private GameObject _camera;
    private float sqrInTripRadius;
    private float rotationSpeed = 50f;
    private float RotationMultiplier = 0.2f;
    private Vector3 _velocity;
    private bool _isGrounded;
    private int ground;
    private Vector3 drag;
    private float _cameraVertAngle;

    
    // Start is called before the first frame update
    void Start()
    {
        _controller = this.GetComponent<CharacterController>();
        _velocity = Vector3.zero;
        ground = 1 << 8;
        drag = new Vector3(0.1f, 0.5f, 0.1f);
        Cursor.visible = false;
        //_cameraVertAngle = 45;
        sqrInTripRadius = (outTripRadius - 1) * (outTripRadius - 1);
        //transform.position = new Vector3(PlayerPrefs.GetFloat("Player_x"), PlayerPrefs.GetFloat("Player_y"), PlayerPrefs.GetFloat("Player_z"));
        //transform.Rotate(PlayerPrefs.GetFloat("Player_rx"), PlayerPrefs.GetFloat("Player_ry"), PlayerPrefs.GetFloat("Player_rz"));
        _groundChecker = this.transform.GetChild(0).gameObject;
        _camera = this.transform.GetChild(1).gameObject;

        if (isLocalPlayer)
        {
            _camera.GetComponent<Camera>().enabled = true;
            _camera.GetComponent<AudioListener>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);
        move = transform.TransformVector(move);
        //_controller.Move(move * Time.deltaTime * speed);

        _velocity.y += gravity * Time.deltaTime;

        _isGrounded = Physics.CheckSphere(_groundChecker.transform.position, groundDistance, ground, QueryTriggerInteraction.Ignore);
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        if (Input.GetKey(KeyCode.Space) && _isGrounded)
            _velocity.y = jump;
        else if (Input.GetKey(KeyCode.Space) && _velocity.y < max_up)
            _velocity.y += power;

        _velocity.x /= 1 + drag.x * Time.deltaTime;
        _velocity.y /= 1 + drag.x * Time.deltaTime;
        _velocity.z /= 1 + drag.x * Time.deltaTime;

        if (isLocalPlayer)
        {
            _controller.Move((_velocity + move * speed) * Time.deltaTime);
        }
        //_controller.Move((_velocity + move * speed) * Time.deltaTime);
        //Debug.Log("Jump Press Status: " + Input.GetKey(KeyCode.Space));
        //Debug.Log("Grounded Status: " + _isGrounded);
        //Debug.Log("Ground Checker Position is " + _groundChecker.transform.position);
    }

    private void LateUpdate()
    {
        // This code is from the Unity FPS tutorial
        //Debug.Log(Input.mousePosition);
        //float lookInputsHoriz = GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameHorizontal, GameConstants.k_AxisNameJoystickLookHorizontal);
        float lookInputsHoriz = Input.GetAxisRaw("Mouse X");
        //float lookInputsVert = GetMouseOrStickLookAxis(GameConstants.k_MouseAxisNameVertical, GameConstants.k_AxisNameJoystickLookVertical);
        float lookInputsVert = Input.GetAxisRaw("Mouse Y");

        // Transform CameraMan
        float rotate_y = lookInputsHoriz * rotationSpeed * RotationMultiplier;
        transform.Rotate(new Vector3(0f, rotate_y, 0f), Space.Self);
        //PlayerPrefs.SetFloat("Player_rx", 0f);
        //PlayerPrefs.SetFloat("Player_ry", rotate_y);
        //PlayerPrefs.SetFloat("Player_rz", 0f);

        PlayerPrefs.SetFloat("Player_x", transform.position.x);
        PlayerPrefs.SetFloat("Player_y", transform.position.y);
        PlayerPrefs.SetFloat("Player_z", transform.position.z);

        //float temp = lookInputsHoriz * rotationSpeed * RotationMultiplier;
        //transform.forward = new Vector3(Mathf.Cos(temp), Mathf.Sin(temp),0f).normalized;

        // Add vertical angle, but set limits 
        _cameraVertAngle += lookInputsVert * rotationSpeed * RotationMultiplier;
        _cameraVertAngle = Mathf.Clamp(_cameraVertAngle, -89f, 89f);
        _camera.transform.localEulerAngles = new Vector3(-_cameraVertAngle, 0, 0);

        //////////////////////////////////////////////////////////////////////////
        /// Check for Nearby Doors
        int doorLayer = 1 << 9;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, outTripRadius, doorLayer);
        for (int i = 0; i != hitColliders.Length; i++)
        {
            if ((transform.position - hitColliders[i].gameObject.transform.position).sqrMagnitude < sqrInTripRadius)
                hitColliders[i].gameObject.GetComponent<Animator>().SetBool("character_nearby", true);
            else
                hitColliders[i].gameObject.GetComponent<Animator>().SetBool("character_nearby", false);
            //Debug.Log((transform.position - hitColliders[i].gameObject.transform.position).sqrMagnitude < sqrInTripRadius);
        }
    }
}
