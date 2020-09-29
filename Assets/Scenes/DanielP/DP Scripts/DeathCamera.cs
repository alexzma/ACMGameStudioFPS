using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamera : MonoBehaviour
{
    public int shift = 5;
    public float rotationSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject child = transform.GetChild(0).gameObject;
        child.transform.position += new Vector3(shift, shift, 0);
        //child.transform.Rotate(30, -90, 0);
        child.transform.LookAt(this.transform.position + new Vector3(0,3,0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0, Time.deltaTime * rotationSpeed, 0, Space.Self);
    }
}
