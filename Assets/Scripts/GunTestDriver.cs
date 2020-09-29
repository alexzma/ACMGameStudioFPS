using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTestDriver : MonoBehaviour
{
    BulletEffects be;
    MuzzleEffects me;
    void Start()
    {
        be = this.GetComponent<BulletEffects>();
        me = this.GetComponent<MuzzleEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            be.CreateBulletEffect();
            me.CreateMuzzleEffect();
        }
    }
}
