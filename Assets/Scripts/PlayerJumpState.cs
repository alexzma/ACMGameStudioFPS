using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    PlayerControllerFSM player;
    public override void EnterState(PlayerControllerFSM player)
    {
        this.player = player;
    }

    public override void Update(PlayerControllerFSM player)
    { 
        if (Input.GetKeyDown(KeyCode.LeftShift) && player.menuCheck.menuShowing == false)
        {
            if (player.jetPackFuel > 0)
            {
                //jetpack initial force
                player.playerRb.AddForce(Vector3.up * 3f, ForceMode.Impulse);
                player.charAnimator.SetBool("IdleJump", false);
                player.charAnimator.SetBool("JumpForward", false);
                player.charAnimator.SetBool("JumpBack", false);
                player.charAnimator.SetBool("JumpRight", false);
                player.charAnimator.SetBool("JumpLeft", false);
                player.TransitionToState(player.FlyState);
            }
        }
    }

    public override void OnCollisionEnter(PlayerControllerFSM player, Collision other)
    {
        int layer = other.gameObject.layer;
        if (player.whatIsGround != (player.whatIsGround | (1 << layer)))
        {
            return;
        }
        else
        {
            // Play Collision Sound
            player.transform.GetComponent<AudioManager>().Play("Impact", false);

            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;
                //transition to walk after landing
                if (player.IsFloor(normal))
                {
                    player.charAnimator.SetBool("IdleJump", false);
                    player.charAnimator.SetBool("JumpForward", false);
                    player.charAnimator.SetBool("JumpBack", false);
                    player.charAnimator.SetBool("JumpRight", false);
                    player.charAnimator.SetBool("JumpLeft", false);                    
                    player.TransitionToState(player.WalkState);
                }
            }
        }
    }
    public override void FixedUpdate()
    {
        //slower movement in air
        player.Movement(7f);
    }

    public override void OnTriggerStay(Collider other)
    {
        return;
    }
}
