using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerControllerFSM player);
    public abstract void Update(PlayerControllerFSM player);
    public abstract void OnCollisionEnter(PlayerControllerFSM player, Collision other);
    public abstract void FixedUpdate();
    public abstract void OnTriggerStay(Collider other);
}
