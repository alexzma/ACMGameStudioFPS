using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;   // DEBUG

// Note - all attack damage factor from gun
public class Gun : MonoBehaviour
{
    [Header("Gun Attributes")]
    public int baseDmg = 1;
    public float baseCd = 1;                        // Lower is faster
    [Range(0, 100)] public float baseAcc = 100;     // 100 = perfect accuracy
    public int baseAmmoCap = 10;
    public float baseReloadDelay = 0.5f;
    public float baseAimZoom = 1.2f;                // Mult on default view

    // Do we want finite total ammo (R6) or infinite (Overwatch)
    
    [Header("State")]
    public int curAmmo;
    public bool aiming = false;                     // aiming state, controlled thru PlayerAttack

    [Header("Relations")]
    public LayerMask hitLayers;
    public UnityEvent fired;

    Camera viewCam;
    CamRecoil cr;

    bool canFire = false;
    float defaultFOV;

    private void Awake()
    {
    }

    public void Setup(Camera viewCam)
    {
        this.viewCam = viewCam;
        cr = viewCam.GetComponentInParent<CamRecoil>();
        fired.AddListener(cr.Fire);                         // CamRecoil listens for firing

        defaultFOV = viewCam.fieldOfView;
        curAmmo = baseAmmoCap;
        // DEBUG - Ammo amount indicator
        ammoIndicator.text = curAmmo + "/" + baseAmmoCap;
    }

    float nextFire = Constants.ACTIVATE_GUN_DELAY;
    float startTime;            // DEBUG
    bool lastAiming = true;     // DEBUG
    public Text ammoIndicator;
    private void Update()
    {
        // Check has ammo, attack cooldown
        if (curAmmo > 0 && Time.time > nextFire)
        {
            nextFire = Time.time + baseCd;
            canFire = true;
        }

        // DEBUG - aiming anim
        if (lastAiming != aiming)
        {
            startTime = Time.time;
            lastAiming = aiming;
        }
        float frac = (Time.time - startTime) / 0.1f;    // DEBUG
        if (aiming)
        {
            cr.aiming = true;
            AimingZoom();
            transform.localPosition = Vector3.Lerp(new Vector3(0.125f, 0.111f, 0), new Vector3(-0.5f, 0.111f, 0), frac);
        }
        else
        {
            cr.aiming = false;
            ResetAimingZoom();
            transform.localPosition = Vector3.Lerp(new Vector3(-0.5f, 0.111f, 0), new Vector3(0.125f, 0.111f, 0), frac);
        }
    }

    public void Fire()
    {
        if (canFire)
        {
            // Manage ammo
            curAmmo--;

            // DEBUG - Ammo amount indicator
            ammoIndicator.text = curAmmo + "/" + baseAmmoCap;

            // Activate listeners
            fired.Invoke();

            // Hitscan
            Ray mousePos = viewCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mousePos, out hit, Mathf.Infinity))
            {
                //Debug.DrawLine(transform.position, hit.point, Color.red, 2);
                DebugDrawLine(transform.position, hit.point, 10, 0.98f);
                Debug.Log("Did Hit");
                print(hit.point);
            }
            else
            {
                //DebugDrawLine(transform.position, hit.point, 10, 0.9f);
                Debug.Log("Did not Hit");
            }

            canFire = false;
        }
    }

    // Public facing reload func
    public void Reload()
    {
        StartCoroutine("ReloadCoroutine");
    }

    // Reload helper
    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(baseReloadDelay);
        curAmmo = baseAmmoCap;

        // DEBUG - Ammo amount indicator
        ammoIndicator.text = curAmmo + "/" + baseAmmoCap;
    }

    // Aiming zoom effect
    void AimingZoom()
    {
        viewCam.fieldOfView = Mathf.Lerp(viewCam.fieldOfView, defaultFOV / baseAimZoom, 25 * Time.deltaTime);
    }

    void ResetAimingZoom()
    {
        viewCam.fieldOfView = Mathf.Lerp(viewCam.fieldOfView, defaultFOV, 50 * Time.deltaTime);
    }

    // lenMult shrinks original line length by percentage
    private void DebugDrawLine(Vector3 start, Vector3 end, float dur, float lenMult = 1)
    {
        GameObject obj = new GameObject();
        LineRenderer line = obj.AddComponent<LineRenderer>();
        line.SetPosition(0, start + (end - start) * lenMult);
        line.SetPosition(1, end);
        line.startColor = Color.blue;
        line.endColor = Color.red;
        line.widthMultiplier = 0.05f;
        Destroy(obj, dur);
    }

    //private void OnDestroy()
    //{
    //    fired.RemoveListener(viewCam.GetComponentInParent<CamRecoil>().Fire);
    //}

    //public void Fire()
    //{
    //    if (canFire)
    //    {
    //        // Simulate Gun accuracy
    //        float accRan = Random.Range(baseAcc - 100, 100 - baseAcc);
    //        Vector2 trajectory = transform.right + transform.up * accRan * 0.005f;

    //        // Shoot Gun
    //        RaycastHit2D hit = Physics2D.Raycast(transform.position, trajectory, 20, hitLayers);
    //        if (!hit)   // Didn't hit in range
    //        {
    //            // Set hit point to max range
    //            hit.point = transform.position + (Vector3)(trajectory * 20);
    //        }
    //        Debug.DrawLine(transform.position, hit.point, Color.red, 2);

    //        // Briefly instantiate line renderer as bullet trail
    //        GameObject inst_bulletTrailObj = Instantiate(bulletTrailObj, transform);
    //        LineRenderer bulletTrail = inst_bulletTrailObj.GetComponent<LineRenderer>();
    //        bulletTrail.SetPosition(0, transform.position);
    //        bulletTrail.SetPosition(1, hit.point);
    //        Destroy(inst_bulletTrailObj, bulletTrailLifetime);

    //        // Do Damage
    //        if (hit.collider && hit.collider.gameObject.GetComponent<Damageable>() != null)
    //        {
    //            Damageable hitDmgable = hit.collider.gameObject.GetComponent<Damageable>();
    //            hitDmgable.DoDamage(baseDmg);
    //        }

    //        // Reset attack cooldown
    //        nextFire = Time.time + baseCd;
    //        canFire = false;
    //    }
    //}
}
