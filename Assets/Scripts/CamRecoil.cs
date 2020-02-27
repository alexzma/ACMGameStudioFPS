using UnityEngine;

public class CamRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float rotationSpeed = 6;
    public float returnSpeed = 25;

    [Header("Hipfire")]
    public Vector3 RecoilRot = new Vector3(2f, 2f, 2f);

    [Header("Aiming")]
    public Vector3 RecoilRotAiming = new Vector3(0.5f, 0.5f, 1.5f);

    [Header("State")]
    public bool aiming;

    Vector3 currentRot;
    Vector3 rot;

    Vector3 savedRot;

    private void FixedUpdate()
    {
        currentRot = Vector3.Lerp(currentRot, Vector3.zero, returnSpeed * Time.deltaTime);        // Change to fixedDelta?
        rot = Vector3.Slerp(rot, currentRot, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        if (aiming)
        {
            currentRot += new Vector3(-RecoilRotAiming.x, Random.Range(-RecoilRotAiming.y, RecoilRotAiming.y), Random.Range(-RecoilRotAiming.z, RecoilRotAiming.z));
        }
        else
        {
            currentRot += new Vector3(-RecoilRot.x, Random.Range(-RecoilRot.y, RecoilRot.y), Random.Range(-RecoilRot.z, RecoilRot.z));
        }
    }


}