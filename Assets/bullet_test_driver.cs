using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_test_driver : MonoBehaviour
{
    BulletEffects be;
    MuzzleEffects me;
    void Start()
    {
        be = this.GetComponent<BulletEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            be.CreateBulletEffect();
        }
    }
}
