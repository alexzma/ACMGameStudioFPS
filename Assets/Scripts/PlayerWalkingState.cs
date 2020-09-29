using UnityEngine;
using System;
using System.Collections;

public class PlayerWalkingState : PlayerBaseState
{
    PlayerControllerFSM player;
    public float maxSpeed = 10;
    public float outTripRadius = 5;
    private float sqrInTripRadius;
    public float minDiveSpeed = 10f;
    public float minCrouchSpeed = 0.25f;
    public float diveForce = 2f;
    private Vector3 DiveScale = new Vector3(0, 0.427f, 0.08167f);
    public override void EnterState(PlayerControllerFSM player)
    {
        this.player = player;
        player.PressedJump = false;
        sqrInTripRadius = (outTripRadius - 1) * (outTripRadius - 1);
        player.charAnimator.SetBool("isGrounded", true);
    }
    public override void OnCollisionEnter(PlayerControllerFSM player, Collision other)
    {
        return;
    }
    public override void Update(PlayerControllerFSM player)
    {       
        //get input
        player.PressedJump = Input.GetButton("Jump");
        if (player.isGrounded && player.PressedJump && player.menuCheck.menuShowing == false)
        {
            // Add Jump sound
            player.transform.GetComponent<AudioManager>().Play("Jump");

            player.playerRb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);
            Vector3 velocity = player.transform.InverseTransformDirection(player.playerRb.velocity);
                if (velocity.z > 1)
                {
                    player.charAnimator.SetBool("JumpForward", true);
                }
                else if (velocity.z < -1)
                {
                    player.charAnimator.SetBool("JumpBack", true);
                }
                if (velocity.x > 1)
                {
                    player.charAnimator.SetBool("JumpRight", true);
                }
                else if (velocity.x < -1)
                {
                    player.charAnimator.SetBool("JumpLeft", true);
                }
                else
                {
                    player.charAnimator.SetBool("IdleJump", true);
                }
                player.charAnimator.SetBool("isGrounded", false);

            player.TransitionToState(player.JumpState);
        }
        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl) && player.isGrounded && player.menuCheck.menuShowing == false)
        {
            //only dive if grounded and velocity is greater than a certain speed
            if (player.playerRb.velocity.magnitude > minDiveSpeed && player.isGrounded)
            {
                //player.transform.localScale = player.DiveScale;
                Vector3 forceApplied = new Vector3(player.orientation.transform.forward.x, 3, player.orientation.transform.forward.z);
                
                player.PlayerCapCollider.height = player.PlayerCapCollider.height / 2;
                player.PlayerCapCollider.center = DiveScale;
                player.playerRb.AddForce(forceApplied * diveForce, ForceMode.Impulse);
                player.charAnimator.SetBool("Dive", true);
                player.TransitionToState(player.DiveState);
            }
            else //prone
            {
                player.PlayerCapCollider.height = player.PlayerCapCollider.height / 2;
                player.PlayerCapCollider.center = DiveScale;
                //player.PlayerCapCollider.height = player.PlayerCapCollider.height / 2;
                player.charAnimator.SetBool("RegularCrouch", true);
                player.charAnimator.SetBool("isGrounded", true);
                player.TransitionToState(player.ProneState);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && player.menuCheck.menuShowing == false)
        {
            if (player.jetPackFuel > 0)
            {
                //jetpack force is 1
                player.playerRb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
                player.charAnimator.SetBool("isGrounded", false);
                player.TransitionToState(player.FlyState);
            }
        }

        player.Movement(maxSpeed);
       
        int doorLayer = 1 << 9;
        Collider[] hitColliders = Physics.OverlapSphere(player.transform.position, outTripRadius, doorLayer);
        for (int i = 0; i != hitColliders.Length; i++)
        {
            Animator animator = hitColliders[i].gameObject.GetComponent<Animator>();
            bool previous = animator.GetBool("character_nearby");
            DoorSound sound = hitColliders[i].gameObject.GetComponent<DoorSound>();
            if ((player.transform.position - hitColliders[i].gameObject.transform.position).sqrMagnitude < sqrInTripRadius)
            {
                animator.SetBool("character_nearby", true);
                if (sound != null && previous != true)
                    sound.PlaySound();
            }
            else
            {
                animator.SetBool("character_nearby", false);
                if (sound != null && previous != false)
                    sound.PlaySound();
            }
        }
    }

    public override void FixedUpdate()
    {
        return;
    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("StairGoingUp"))
        {
            float angle = Vector3.Angle(player.playerRb.velocity, other.transform.forward);
            //Debug.Log(angle);
            if (!Input.GetButton("Jump") && angle < 50 && angle > 0)
            {

                if (angle > -90)
                {
                    if (player.playerRb.velocity.y > 0)
                        player.playerRb.velocity = new Vector3(player.playerRb.velocity.x, 0, player.playerRb.velocity.z);
                }
            }
        }
        if (other.CompareTag("StairGoingDown"))
        {
            if (!Input.GetButton("Jump") && player.isGrounded) //&& Vector3.Angle(playerRb.velocity, other.transform.forward) < 90)
            {
                player.playerRb.AddForce(other.transform.up * -2, ForceMode.Impulse);

                Debug.Log("Down");
            }
        }
    }
}
