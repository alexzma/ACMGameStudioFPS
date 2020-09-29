using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private float mouseX;
    private float mouseY;
    public float mouseSensitivity;
    public bool look;

    public Transform child;
    public Transform playerBody;
    public bool prone;
    public bool resetAngle;

    float xRotation = 0f;
    static float t = 0.0f;
    static float t2 = 0.0f;
    float targetAngle = 80f;
    float targetAngle2 = 270f;
    float clamp1 = -60f;
    float clamp2 = 60f;
    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        look = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (look)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }
        else
        {
            mouseX = 0;
            mouseY = 0;
        }

        xRotation -= mouseY;
        //Debug.Log(xRotation);
        xRotation = Mathf.Clamp(xRotation, clamp1, clamp2);
        if (resetAngle)
        {          
            if (t2 < 1)
            {
                t = 0;
                t2 += 0.6f * Time.deltaTime;
                transform.localRotation = Quaternion.Euler(Mathf.LerpAngle(transform.localEulerAngles.x, 0, t2), 0f, 0f);
                child.localRotation = Quaternion.Euler(Mathf.LerpAngle(child.localEulerAngles.x, 0, t2 * 0.25f), 0f, 0f);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                child.localRotation = Quaternion.Euler(0f, 0f, 0f);
                xRotation = transform.localEulerAngles.x;
                clamp1 = -60f;
                clamp2 = 60f;
                resetAngle = false;              
            }
        }
        else if (prone)
        {
            clamp1 = 260f;
            clamp2 = 320f;
            if (t < 1)
            {
                t2 = 0;
                t += 0.6f * Time.deltaTime;                
                transform.localRotation = Quaternion.Euler(Mathf.LerpAngle(transform.localEulerAngles.x, targetAngle, t), 0f, 0f);
                child.localRotation = Quaternion.Euler(Mathf.LerpAngle(child.localEulerAngles.x, targetAngle2, 0.07f*t), 0f, 0f);
                
                xRotation = child.localEulerAngles.x;
            }
            else
            {                
 /*               clamp1 = -110f;
                clamp2 = -90f;*/
                transform.localRotation = Quaternion.Euler(80f, 0f, 0f);
                child.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            }
            //Debug.Log(xRotation);
        }
        else
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Debug.Log(child.transform.localEulerAngles.x);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
