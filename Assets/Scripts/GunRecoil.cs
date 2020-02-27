using UnityEngine;

public class GunRecoil : MonoBehaviour
{
    [Header("Reference Points")]
    public Transform recoilPos;
    public Transform rotPoint;

    [Header("Speed Settings")]
    public float posRecoilSpeed = 8f;
    public float rotRecoilSpeed = 8f;

    public float posReturnSpeed = 18f;
    public float rotReturnSpeed = 38f;

    [Header("Amount Settings")]
    public Vector3 recoilRot = new Vector3(10, 5, 7);
    public Vector3 recoilKickBack = new Vector3(0.015f, 0f, -0.2f);
    public Vector3 recoilRotAim = new Vector3(10, 4, 6);
    public Vector3 recoilKickBackAim = new Vector3(0.015f, 0f, -0.2f);

    [Header("State")]
    public bool aiming;

    Vector3 rotRecoil;
    Vector3 posRecoil;
    Vector3 rot;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Fire();
        }
        if (Input.GetButton("Fire2"))
        {
            aiming = true;
        }
        else
        {
            aiming = false;
        }
    }

    private void FixedUpdate()
    {
        rotRecoil = Vector3.Lerp(rotRecoil, Vector3.zero, rotReturnSpeed * Time.deltaTime);
        posRecoil = Vector3.Lerp(posRecoil, Vector3.zero, posReturnSpeed * Time.deltaTime);

        recoilPos.localPosition = Vector3.Slerp(recoilPos.localPosition, posRecoil, posRecoilSpeed * Time.fixedDeltaTime);
        rot = Vector3.Slerp(rot, rotRecoil, rotRecoilSpeed * Time.fixedDeltaTime);
        rotPoint.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        if (aiming)
        {
            rotRecoil += new Vector3(-recoilRotAim.x, Random.Range(-recoilRotAim.y, recoilRotAim.y), Random.Range(-recoilRotAim.z, recoilRotAim.z));
            posRecoil += new Vector3(Random.Range(-recoilKickBackAim.x, recoilKickBackAim.x), Random.Range(-recoilKickBackAim.y, recoilKickBackAim.y), recoilKickBackAim.z);
        }
        else
        {
            rotRecoil += new Vector3(-recoilRot.x, Random.Range(-recoilRot.y, recoilRot.y), Random.Range(-recoilRot.z, recoilRot.z));
            posRecoil += new Vector3(Random.Range(-recoilKickBack.x, recoilKickBack.x), Random.Range(-recoilKickBack.y, recoilKickBack.y), recoilKickBack.z);
        }
    }
}