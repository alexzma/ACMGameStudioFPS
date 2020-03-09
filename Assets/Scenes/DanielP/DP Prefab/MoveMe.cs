using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMe : MonoBehaviour
{
    private CharacterController _controller;
    [SerializeField] private float speed;
    [SerializeField] private GameObject _groundChecker;
    [SerializeField] private Camera _camera;
    [SerializeField] private float outTripRadius;

    private Vector3 _velocity;
    private bool _isGrounded;
    private float sqrInTripRadius;
    private float rotationSpeed = 50f;
    private float rotationMultiplier = 0.2f;
    private int ground;
    private float _cameraVertAngle;

    
    // Start is called before the first frame update
    void Start()
    {
        _controller = this.GetComponent<CharacterController>();
        _velocity = Vector3.zero;
        ground = 1 << 8;
        Cursor.visible = false;
        sqrInTripRadius = (outTripRadius - 1) * (outTripRadius - 1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        move = Vector3.ClampMagnitude(move, 1);
        move = transform.TransformVector(move);
        _controller.Move(move * Time.deltaTime * speed);

        _velocity.y += -10 * Time.deltaTime;

        _isGrounded = Physics.CheckSphere(_groundChecker.transform.position, 2.5f, ground, QueryTriggerInteraction.Ignore);
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = 0f;

        _controller.Move(_velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        // This code is from the Unity FPS tutorial
        float lookInputsHoriz = Input.GetAxisRaw("Mouse X");
        float lookInputsVert = Input.GetAxisRaw("Mouse Y");

        // Transform CameraMan
        transform.Rotate(new Vector3(0f, (lookInputsHoriz * rotationSpeed * rotationMultiplier), 0f), Space.Self);

        // Add vertical angle, but set limits 
        _cameraVertAngle += lookInputsVert * rotationSpeed * rotationMultiplier;
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
        }
    }
}
