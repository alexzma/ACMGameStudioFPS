using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shooting : NetworkBehaviour
{
    private float _fov;
    private float FOV_MIN = 30f;
    private float FOV_MAX = 60f;
    private Camera _camera;

    // Ammo Variables
    [SerializeField] private int ammo = 0;
    [SerializeField] private int MAX_AMMO = 16;
    [SerializeField] private float TIME_BETWEEN_SHOTS = 0.18f;
    //[SerializeField] private float RELOAD_TIME_PER_BULLET = 0.25f;

    // Shooting Variables
    public float MAX_RANGE = 20f;
    public int BULLET_DAMAGE = 2;
    public float ZOOM_SPEED = 500;
    private int AIMED_BULLET_DAMAGE;
    private bool canShoot = true;
    private float mouseSensitivity;
    private bool zoomed;
    private Vector3 gunoffset = new Vector3(0, -1.3f, 0);

    // Constants
    int PLAYER_LAYER;
    int MYSELF_LAYER;
    bool TEAM_ATTACK = false;
    float RELOAD_DURATION = 4.75f;

    // Coroutines
    private IEnumerator ReloadCoroutine;
    private bool ReloadCo_running = false;

    // Components
    public Animator charAnimator;
    private KeyListener _keyListener;

    // Gun Effects
    BulletEffects be;
    MuzzleEffects me;

    // Bullet Effects
    public Material material;
    public GameObject gun;

    // Method Declarations
    /*
     * void Start();
     * void Update();
     * 
     * public void ResetBullets();
     * [Command] public void CmdPlayerShot(string, int);
     * private void ZoomIn(bool);
     * private void FireGun();
     * private void ReEnableShooting();
     * private IEnumerator DelayedStart();
     * private IEnumerator Reload(float);
     * 
     */

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            // Set Constants
            PLAYER_LAYER = LayerMask.GetMask("Remote Player");
            MYSELF_LAYER = LayerMask.GetMask("Local Player");

            // Assign Field of View to Camera
            _camera = GetComponentInChildren<Camera>();
            _camera.fieldOfView = FOV_MAX;
            _fov = FOV_MAX;

            // Set Ammo Value
            ammo = MAX_AMMO;

            // Set Aimed bullet Damage
            AIMED_BULLET_DAMAGE = BULLET_DAMAGE * 2;

            // Get Mouse sensitivity
            StartCoroutine(DelayedStart());
            zoomed = false;

            _keyListener = GameObject.Find("Canvas(Clone)").GetComponent<KeyListener>();

            // Set Effects
            be = this.GetComponent<BulletEffects>();
            me = this.GetComponent<MuzzleEffects>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            // Scope Controls
            if (Input.GetMouseButton(1))
            {
                ZoomIn(true);
                gameObject.GetComponentInChildren<CameraLook>().mouseSensitivity = mouseSensitivity / 4;
            }
            else
            {
                if (zoomed)
                    gameObject.GetComponentInChildren<CameraLook>().mouseSensitivity = mouseSensitivity;
                ZoomIn(false);
            }

            // Shooting Controls. Checks ammo, shoot input, and current canShoot state
            if (canShoot && ammo != 0 && !ReloadCo_running && !_keyListener.menuShowing && Input.GetMouseButton(0)) // 
            {
                //if (ReloadCo_running)
                //{
                //    StopCoroutine(ReloadCoroutine);
                //    ReloadCo_running = false;
                //}
                charAnimator.SetBool("Shooting", true);
                canShoot = false;
                ammo--;
                gameObject.GetComponent<PlayerScript>().SetAmmoText(ammo);
                FireGun();
                Invoke("ReEnableShooting", TIME_BETWEEN_SHOTS);
            }
            // Controls Reloading. A player cannot shoot and reload simulataneously
            else if (Input.GetKey(KeyCode.R) && !ReloadCo_running)
            {
                //if (ammo != MAX_AMMO)
                //{
                //    ////// A Player cannot shoot while reloading //////
                //    canShoot = false;
                //    ReloadCoroutine = Reload(RELOAD_TIME_PER_BULLET);
                //    StartCoroutine(ReloadCoroutine);
                //    Invoke("ReEnableShooting", RELOAD_DURATION);
                //}

                canShoot = false;
                ReloadCoroutine = Reload(RELOAD_DURATION);
                StartCoroutine(ReloadCoroutine);
                Invoke("ReEnableShooting", RELOAD_DURATION);
            }
            if(ammo == 0 || !Input.GetMouseButton(0) || ReloadCo_running || Input.GetKey(KeyCode.R))
            {
                charAnimator.SetBool("Shooting", false);
            }
        }
    }

    public void ResetBullets()
    {
        // Called during respawn
        ammo = MAX_AMMO;
        gameObject.GetComponent<PlayerScript>().SetAmmoText(ammo);
    }

    // Instructs all versions of a player to take damage
    [Command]
    public void CmdPlayerShot(string playerId, int damage)
    {
        PlayerScript _player = GameManagerScript.GetPlayer(playerId);
        if (!_player.GetInvicible())
            _player.RpcTakeDamage(damage, gameObject.name);
    }

    [ClientRpc]
    public void RpcGunFiring()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, MAX_RANGE, ~MYSELF_LAYER))
        {
            // Create Bullet-line Effect
            DrawLine(gun.transform.position + gun.transform.forward + gun.transform.InverseTransformVector(gunoffset), hit.point, Color.blue, 0.03f);
        }
        else
        {
            // Create Bullet-line Effect
            DrawLine(gun.transform.position + gun.transform.forward + gun.transform.InverseTransformVector(gunoffset),
                _camera.transform.position + _camera.transform.TransformDirection(Vector3.forward).normalized * MAX_RANGE, Color.blue, 0.03f);
        }

        // Play Shooting Sound
        GetComponent<AudioManager>().Play("Shooting");

        // Create Muzzle Effect
        me.CreateMuzzleEffect();
    }

    [Command]
    public void CmdGunFiring()
    {
        RpcGunFiring();
    }

    // Zooming in/Out
    private void ZoomIn(bool zoom)
    {
        zoomed = zoom;
        _fov = _camera.fieldOfView;
        if (zoom)
        {
            if (_fov == FOV_MIN)
                return;

            _fov -= Time.deltaTime * ZOOM_SPEED;
            if (_fov < FOV_MIN)
            {
                _fov = FOV_MIN;
            }
        }
        else
        {
            if (_fov == FOV_MAX)
                return;

            _fov += Time.deltaTime * ZOOM_SPEED;
            if (_fov > FOV_MAX)
            {
                _fov = FOV_MAX;
            }
        }
        _camera.fieldOfView = _fov;
    }

    // Shoots a Ray and damages the first player it contacts
    private void FireGun()
    {
        be.CreateBulletEffect();
        //me.CreateMuzzleEffect();
        Vector3 direction = _camera.transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(_camera.transform.position, direction, out RaycastHit hit, MAX_RANGE, PLAYER_LAYER))
        {
            if (hit.transform.CompareTag("Player"))
            {
                int damage = (_fov < FOV_MAX ? AIMED_BULLET_DAMAGE : BULLET_DAMAGE);
                string targetName = hit.transform.name;
                bool my_team = GameManagerScript.GetTeam(gameObject.name);
                if (TEAM_ATTACK || GameManagerScript.GetTeam(targetName) != my_team)
                    CmdPlayerShot(hit.transform.name, damage);
                return;
            }
        }
        // Create Bullet-line Effect
        //DrawLine(gun.transform.position + gun.transform.forward + gun.transform.InverseTransformVector(gunoffset), _camera.transform.position + direction * 3, Color.blue, 2f);

        // Play Shooting Sound
        //GetComponent<AudioManager>().Play("Shooting");
        if (isServer)
            RpcGunFiring();
        else
            CmdGunFiring();
    }

    // Players cannot shoot continuously. This functions re-enables shooting and should be invoked
    private void ReEnableShooting()
    {
        canShoot = true;
    }

    // Waits a bit before setting variables
    private IEnumerator DelayedStart()
    {
        yield return 0;
        mouseSensitivity = gameObject.GetComponentInChildren<CameraLook>().mouseSensitivity;
        gameObject.GetComponent<PlayerScript>().SetAmmoText(ammo);
    }

    // Reloading
    private IEnumerator Reload(float time)
    {
        ReloadCo_running = true;

        // Refills ammo while coroutine is active
        //while (ammo != MAX_AMMO)
        //{
        //    yield return new WaitForSeconds(time);
        //    ammo++;

        //    // Upadate Ammo value on Canvas
        //    gameObject.GetComponent<PlayerScript>().SetAmmoText(ammo);
        //}


        // Play Reload Sound
        GetComponent<AudioManager>().Play("Reload");

        yield return new WaitForSeconds(time);
        ammo = MAX_AMMO;
        gameObject.GetComponent<PlayerScript>().SetAmmoText(ammo);

        ReloadCo_running = false;
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color, float time)
    {
        GameObject line = new GameObject();
        line.AddComponent<LineRenderer>();
        line.SetActive(true);
        LineRenderer lineRender = line.GetComponent<LineRenderer>();
        lineRender.enabled = true;
        //Material blueLight = (Material)Resources.Load("Assets/Scenes/DanielP/Sci-Fi Styled Modular Pack/Materials/blue emission");
        lineRender.material = material;
        lineRender.startColor = color;
        lineRender.endColor = color;
        lineRender.startWidth = 0.05f;
        lineRender.endWidth = 0.1f;
        lineRender.SetPosition(0, start);
        lineRender.SetPosition(1, end);
        GameObject.Destroy(line, time);
    }
}
