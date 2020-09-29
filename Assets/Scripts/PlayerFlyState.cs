using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
//using UnityEditor.Rendering;
using UnityEngine.Rendering;
using UnityEngine;

public class PlayerFlyState : PlayerBaseState
{
    PlayerControllerFSM player;
    public float jetPackForce;
    public float maxJetPackForce;
    public float timeSinceLastZero = 0;
    public override void EnterState(PlayerControllerFSM player)
    {
        jetPackForce = 1f;
        this.player = player;
        Debug.Log("Flying");
        maxJetPackForce = 10f;
        player.charAnimator.SetBool("Flying", true);
        //reset these if we started flying after jumping to reset animations
        player.charAnimator.SetBool("IdleJump", false);
        player.charAnimator.SetBool("JumpForward", false);
        player.charAnimator.SetBool("JumpBack", false);
        player.charAnimator.SetBool("JumpRight", false);
        player.charAnimator.SetBool("JumpLeft", false);
        player.jetpackfire.ActivateJetpackEffect();
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
            // Play Collision Sound
            player.transform.GetComponent<AudioManager>().Play("Impact", false);

            //check if we landed on a floor
            for (int i = 0; i < other.contactCount; i++)
            { 
                Vector3 normal = other.contacts[i].normal;
                //if the floor angle is enough to be walked on
                if (player.IsFloor(normal))
                {
                    player.charAnimator.SetBool("Flying", false);
                    player.charAnimator.SetBool("FlyingLeft", false);
                    player.charAnimator.SetBool("FlyingRight", false);                    
                    player.playerRb.velocity = new Vector3(0, 0, 0);
                    player.ShouldRefuel = true;
                    player.jetpackfire.DeactivateJetpackEffect();
                    player.TransitionToState(player.WalkState);
                }
            }
        }
    }
    public override void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift) && player.jetPackFuel > 0f && player.menuCheck.menuShowing == false)
        {
            player.GetComponent<AudioManager>().Play("Engine", false);

            if (jetPackForce > maxJetPackForce)
                jetPackForce = maxJetPackForce;
            else if (jetPackForce < 1)
                jetPackForce = 1;
            jetPackForce += Time.deltaTime * 5;
            Debug.Log(jetPackForce);
            player.playerRb.velocity = new Vector3(player.playerRb.velocity.x, 0.01f + jetPackForce, player.playerRb.velocity.z);// (Vector3.up * jetPackForce);

            player.jetPackFuel -= Time.deltaTime;
            if (player.jetPackFuel < 0) // hit zero
            {
                player.jetPackFuel = 0;
                timeSinceLastZero = Time.time;
            }
            player.ShouldRefuel = false;
        }
        else
        {
            if (Time.time - timeSinceLastZero < 4)//overheat period before refueling jet pack
            {
                player.jetPackFuel = 0;
            }
            else
            {
                //refuel, but this will be done in the player controller so that the jet pack can 
                //refuel in other states as well
                player.ShouldRefuel = true;
                //reset the jet pack force, but not completely
                jetPackForce -= Time.deltaTime * 0.25f;
            }
        }        
        Vector3 velocity = player.transform.InverseTransformDirection(player.playerRb.velocity);
        if (velocity.x > 2)
        {
            player.charAnimator.SetBool("FlyingRight", true);
        }
        else
        {
            player.charAnimator.SetBool("FlyingRight", false);
        }
        
        if (velocity.x < -2)
        {
            player.charAnimator.SetBool("FlyingLeft", true);
        }
        else
        {
            player.charAnimator.SetBool("FlyingLeft", false);            
        }
        player.Movement(15);
        /*
        player.playerRb.AddForce(player.orientation.transform.right * player.X * player.moveSpeed * Time.deltaTime * 0.25f, ForceMode.Acceleration);
        player.playerRb.AddForce(player.orientation.transform.forward * player.Y * player.moveSpeed * Time.deltaTime * 0.25f, ForceMode.Acceleration);
        */
    }
    public override void OnTriggerStay(Collider other)
    {
        return;
    }
}
