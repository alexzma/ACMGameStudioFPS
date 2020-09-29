using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProneState : PlayerBaseState
{
    PlayerControllerFSM player;
    bool tempFreeze;
    public float maxSpeed = 2;
    private Vector3 PlayerScale = new Vector3(0, 0.8851604f, 0.08167458f);
    public override void EnterState(PlayerControllerFSM player)
    {
        this.player = player;
        tempFreeze = true;
        player.charAnimator.SetBool("UnCrouch", false);
        player.playerCam.prone = true;
    }
    IEnumerator DelayedUpdate(PlayerControllerFSM player)
    {
        //wait a little bit after transition to allow animation to finish
        if (tempFreeze == true)
        {            
            yield return new WaitForSeconds(1f);
            //player.playerRb.velocity = new Vector3(0, 0, 0);
            tempFreeze = false;
           
        }
        player.charAnimator.SetBool("RegularCrouch", false);
        //slower crawling movement
        if (player.isGrounded)
        {
            
            //friction
            
            if (Input.GetKeyDown(KeyCode.LeftControl) && player.isGrounded && !tempFreeze && player.menuCheck.menuShowing == false)
            {
                player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.25f, player.transform.position.z);                
                player.PlayerCapCollider.height = 1.848392f;
                player.PlayerCapCollider.center = PlayerScale;
                player.charAnimator.SetBool("UnCrouch", true);
                player.playerCam.prone = false;
                player.playerCam.resetAngle = true;
                player.TransitionToState(player.WalkState);
            }
        }
        player.Movement(maxSpeed);
    }
    public override void Update(PlayerControllerFSM player)
    {
        player.StartChildRoutine(DelayedUpdate(player));
    }
    public override void OnCollisionEnter(PlayerControllerFSM player, Collision other)
    {
        return;
    }
    public override void FixedUpdate()
    {
        return;
    }
    public override void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("StairGoingDown"))
        {
            if (player.isGrounded) //&& Vector3.Angle(playerRb.velocity, other.transform.forward) < 90)
            {
                player.playerRb.AddForce(other.transform.up * -2, ForceMode.Impulse);

                Debug.Log("Down");
            }
        }
        return;
    }
}
