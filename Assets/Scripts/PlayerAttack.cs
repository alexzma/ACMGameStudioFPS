using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Gun curGun;

    public Camera viewCamera;
    public LayerMask damageable;

    public GameObject test;

    void Awake()
    {
        if (curGun) Equip(curGun);
    }
    
    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            curGun.Fire();
        }
        if (Input.GetButton("Fire2"))
        {
            curGun.aiming = true;
        }
        else
        {
            curGun.aiming = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            curGun.Reload();
        }
    }

    private void Equip(Gun gun)
    {
        curGun = gun;
        gun.Setup(viewCamera);
    }
    
}