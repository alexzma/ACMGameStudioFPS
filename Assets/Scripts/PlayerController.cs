using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 1;
    public float forward;
    public float side;
    public float jumpForce;
    public Rigidbody playerRb;
    public bool isGrounded;

    // Update is called once per frame
    void Update()
    {

        forward = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * speed * Time.deltaTime * forward);        
        side = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * speed * Time.deltaTime * side);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    public void Respawn()
    {
    }
}
