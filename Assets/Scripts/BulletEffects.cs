using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffects : MonoBehaviour
{
    public Camera cam;
    public GameObject bullet_hole;
    private float bulletScale = 1f;
    private float range;
    // Start is called before the first frame update
    public void Start()
    {
        range = GetComponent<Shooting>().MAX_RANGE;
    }

    // Update is called once per frame
    public void CreateBulletEffect()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, range))
        {
            if(hit.transform.tag == "Obstacle")
            {
                GameObject bullet = Instantiate(bullet_hole, hit.point + Random.value/100*hit.normal*-1, Quaternion.LookRotation(hit.normal * -1));
                bullet.transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
            }
            // Do something with the object that was hit by the raycast.
        }
    }
}
