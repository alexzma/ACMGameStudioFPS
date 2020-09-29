using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiveState : PlayerBaseState
{
    PlayerControllerFSM player;
    public override void EnterState(PlayerControllerFSM player)
    {
        this.player = player;
        player.charAnimator.SetBool("isGrounded", false);
        player.charAnimator.SetBool("UnCrouch", false);
        return;
    }
    public override void Update(PlayerControllerFSM player)
    {
        return;
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
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;
                //dive and transition to crawling state
                if (IsFloor(normal))
                {
                    player.playerRb.velocity = new Vector3(0, 0, 0);
                    player.charAnimator.SetBool("Dive", false);
                    player.charAnimator.SetBool("isGrounded", true);
                    player.TransitionToState(player.ProneState);
                }
            }
        }
    }
    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < player.maxSlopeAngle;
    }

    public override void FixedUpdate()
    {
        return;
    }
    public override void OnTriggerStay(Collider other)
    {
        return;
    }
}
