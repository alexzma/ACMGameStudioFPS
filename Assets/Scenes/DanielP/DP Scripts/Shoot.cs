using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Shoot : NetworkBehaviour
{
    // Aiming Variables
    public float rotationSpeed = 50f;
    public float rotationMultiplier = 0.2f;
    private float aimRotationMultiplier;
    private float normRotationMultiplier;
    private float _cameraVertAngle;
    private float _fov = 60f;
    private float _fovMin = 30f;
    private float _fovMax = 60f;
    private GameObject _camera;

    // Shooting Variables
    public float maxRange = 20f;
    public int bulletDamage = 2;
    private int zoomedBulletDamage;
    private Vector3 direction;
    private bool canShoot = true;

    // Constants
    int PLAYER_LAYER;
    bool TEAM_ATTACK = false;

    // Ammo/Reloading Variables;
    private int ammo = 0;
    private bool reloading = false;
    private UnityEngine.UI.Text reload_text;

    public int MAX_AMMO = 16;
    public float TIME_BETWEEN_SHOTS = 0.2f;
    public float RELOAD_TIME_PER_BULLET = 0.25f;

    // Coroutines
    private IEnumerator ReloadCoroutine;
    private bool ReloadCo_running = false;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            // Set Constants
            PLAYER_LAYER = LayerMask.GetMask("Remote Player");

            // Assign Camera
            _camera = this.transform.GetChild(1).gameObject;

            // Assign Private variables which rely on public variables
            aimRotationMultiplier = rotationMultiplier / 4;
            normRotationMultiplier = rotationMultiplier;
            zoomedBulletDamage = bulletDamage * 2;

            // Set Player Ammo
            ammo = MAX_AMMO;

            // Reload Canvas Text
            reload_text = transform.GetChild(2).GetChild(5).GetComponent<UnityEngine.UI.Text>();
            reload_text.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            // Scope Controls
            if (Input.GetMouseButton(1))
            {
                ZoomView("in");
                rotationMultiplier = aimRotationMultiplier;
            }
            else
            {
                ZoomView("out");
                rotationMultiplier = normRotationMultiplier;
            }
            _camera.transform.GetChild(0).GetComponent<Camera>().fieldOfView = _fov;
            _camera.transform.GetChild(1).GetComponent<Camera>().fieldOfView = _fov;
          
            // Shooting Controls. Checks ammo, shoot input, and current canShoot state
            if (canShoot && ammo != 0 && Input.GetMouseButton(0))
            {
                if (ReloadCo_running)
                {
                    reload_text.enabled = false;
                    StopCoroutine(ReloadCoroutine);
                    ReloadCo_running = false;
                }
                canShoot = false;
                ammo--;
                gameObject.GetComponent<Player>().SetAmmoText(ammo);
                FireGun();
                Invoke("ShootingLag", TIME_BETWEEN_SHOTS);
            }
            // Controls Reloading. A player cannot shoot and reload simulataneously
            else if (Input.GetKey(KeyCode.R) && !ReloadCo_running)
            {
                if (ammo != MAX_AMMO)
                {
                    ReloadCoroutine = Reload(RELOAD_TIME_PER_BULLET);
                    StartCoroutine(ReloadCoroutine);
                }         
            }
        }
    }

    // Changes Point of View
    private void ZoomView(string direction)
    {
        float tempFov = _fov;
        if (direction == "in")
        {
            if (_fov == _fovMin)
                return;

            tempFov -= Time.deltaTime * 500;
            if (tempFov <= _fovMin)
            {
                _fov = _fovMin;
                return;
            }
        }
        else if (direction == "out")
        {
            if (_fov == _fovMax)
                return;

            tempFov += Time.deltaTime * 500;
            if (tempFov >= _fovMax)
            {
                _fov = _fovMax;
                return;
            }
        }
        _fov = tempFov;
    }

    // Players cannot shoot continuously. This functions re-enables shooting and should be invoked
    private void ShootingLag()
    {
        canShoot = true;
    }

    // Shoots a Ray and damages the first player it contacts
    public void FireGun()
    {
        direction = _camera.transform.TransformDirection(Vector3.forward);
        if (Physics.Raycast(_camera.transform.position, direction, out RaycastHit hit, maxRange, PLAYER_LAYER))
        {
            if (hit.transform.CompareTag("Player"))
            {
                int damage = (_fov < _fovMax ? zoomedBulletDamage : bulletDamage);
                string targetName = hit.transform.name;
                bool my_team = GameManager.GetTeam(gameObject.name);
                if (TEAM_ATTACK || GameManager.GetTeam(targetName) != my_team)
                    CmdPlayerShot(hit.transform.name, damage);
                //Debug.Log("Shooting " + hit.transform.name + "!");
                return;
            }
        }
    }

    public void ResetBullets()
    {
        // Called during respawn
        ammo = MAX_AMMO;
    }

    // Instructs all versions of a player to take damage
    [Command]
    public void CmdPlayerShot(string playerId, int damage)
    {
        Player _player = GameManager.GetPlayer(playerId);
        _player.TakeDamage(damage, gameObject.name);
    }

    // Reloading
    private IEnumerator Reload(float time)
    {
        ReloadCo_running = true;
        reload_text.enabled = true;

        // Refills ammo while coroutine is active
        while (ammo != MAX_AMMO)
        {
            yield return new WaitForSeconds(time);
            ammo++;

            // Upadate Ammo value on Canvas
            gameObject.GetComponent<Player>().SetAmmoText(ammo);
        }

        reload_text.enabled = false;
        ReloadCo_running = false;
    }

    // Changes player View
    private void LateUpdate()
    {
        // This code is from the Unity FPS tutorial
        float lookInputsHoriz = Input.GetAxisRaw("Mouse X");
        float lookInputsVert = Input.GetAxisRaw("Mouse Y");

        // Transform player
        float rotate_y = lookInputsHoriz * rotationSpeed * rotationMultiplier;
        transform.Rotate(new Vector3(0f, rotate_y, 0f), Space.Self);

        // Add vertical angle, but set limits 
        _cameraVertAngle += lookInputsVert * rotationSpeed * rotationMultiplier;
        _cameraVertAngle = Mathf.Clamp(_cameraVertAngle, -75f, 75f);
        _camera.transform.localEulerAngles = new Vector3(-_cameraVertAngle, 0, 0);
    }
}
