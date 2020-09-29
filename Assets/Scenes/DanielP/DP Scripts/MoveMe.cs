using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class MoveMe : NetworkBehaviour
{
    // Character
    private CharacterController _controller;
    public float speed = 20;
    public float power = 1.05f;
    public float gravity = -20;
    public float groundDistance = 1.1f;
    public float max_up = 10;
    public float jump = 20;
    public float outTripRadius = 5;
    private GameObject _groundChecker;
    private GameObject _runner;
    private float sqrInTripRadius;
    private Vector3 _velocity;
    private Vector3 drag;
    private bool _isGrounded;
    private int ground;

    // Constants
    LayerMask DOOR_LAYER;

    // Start is called before the first frame update
    void Start()
    {
        _controller = transform.GetComponent<CharacterController>();
        _velocity = Vector3.zero;
        ground = 1 << 10;
        drag = new Vector3(0.1f, 0.5f, 0.1f);
        Cursor.visible = false;
        sqrInTripRadius = (outTripRadius - 1) * (outTripRadius - 1);
        _groundChecker = this.transform.GetChild(0).gameObject;

        // Set Constants
        DOOR_LAYER = LayerMask.GetMask("Door");
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            move = Vector3.ClampMagnitude(move, 1);
            move = transform.TransformVector(move);

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

            _controller.Move((_velocity + move * speed) * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        //////////////////////////////////////////////////////////////////////////
        /// Check for Nearby Doors
        Collider[] hitColliders = Physics.OverlapSphere(_groundChecker.transform.position, outTripRadius, DOOR_LAYER);
        for (int i = 0; i != hitColliders.Length; i++)
        {
            if ((_groundChecker.transform.position - hitColliders[i].gameObject.transform.position).sqrMagnitude < sqrInTripRadius)
                hitColliders[i].gameObject.GetComponent<Animator>().SetBool("character_nearby", true);
            else
                hitColliders[i].gameObject.GetComponent<Animator>().SetBool("character_nearby", false);
            //Debug.Log((transform.position - hitColliders[i].gameObject.transform.position).sqrMagnitude < sqrInTripRadius);
        }
    }

    public void ResetPlayerVelocity()
    {
        // Reset the Player's velocity
        _velocity = Vector3.zero;
    }
}
