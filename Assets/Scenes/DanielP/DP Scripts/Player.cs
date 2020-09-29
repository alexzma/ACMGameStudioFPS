using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

enum team : ushort { yellow, blue }

public class Player : NetworkBehaviour
{
    // Player Variables
    [SyncVar] public string playerId = "Player ";
    [SyncVar] public int health = 20;
    public int MAX_HEALTH = 20;
    public float RECOVERY_TIME = 10f;
    public float KILL_ALERT_DURATION = 3f;

    private Vector3 myRespawnPoint;
    private team _myTeam;
    private bool invincible = false;
    private bool isAlive = true;
    private string lastAttacker = ""; // Last person to shoot you

    // Cameras
    public GameObject DeathCameraPrefab;
    private Camera firstCamera;
    private GameObject _camera;
    private GameObject deathCam;

    // Canvas
    private GameObject _myCanvas;
    private UnityEngine.UI.Text _myHealthText;
    private UnityEngine.UI.Text _myAmmoText;
    private GameObject _deathCanvas;
    private UnityEngine.UI.Text _deathTimerText;
    private GameObject _winLoseCanvas;

    // Coroutines
    private IEnumerator RecoveryCoroutine;
    private bool RecovCo_running = false;

    // String Constants
    string KILLED_PREFIX = "Killed by ";
    string KILLER_PREFIX = "You killed ";

    // Start is called before the first frame update
    void Start()
    {
        // Assign Proper Variables
        _camera = this.transform.GetChild(1).gameObject;
        _myCanvas = transform.GetChild(2).gameObject;
        _deathCanvas = transform.GetChild(3).gameObject;
        _winLoseCanvas = transform.GetChild(4).gameObject;

        if (isLocalPlayer)
        {
            firstCamera = Camera.main;
            // Enable proper cameras
            _camera.transform.GetChild(0).GetComponent<Camera>().enabled = true;
            _camera.transform.GetChild(0).GetComponent<AudioListener>().enabled = true;

            _camera.transform.GetChild(1).GetComponent<Camera>().enabled = true;

            // Start Canvas GameObject
            _myCanvas.SetActive(true);
            _myCanvas.transform.GetChild(6).gameObject.SetActive(false);
            _myCanvas.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text = "Kill Count: 0";
            _myHealthText = _myCanvas.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>();
            _myAmmoText = _myCanvas.transform.GetChild(4).GetComponent<UnityEngine.UI.Text>();
            _myAmmoText.text = "Bullets: 16";
            _deathTimerText = _deathCanvas.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>();

            // Assign Local Player Tag
            gameObject.layer = LayerMask.NameToLayer("Local Player");

            // Set Gun to Local Gun Layer
            // A different camera will capture the gun the player holds which is then overlayed
            // onto the existing image, preventing the gun from ever being blocked from view
            GameObject gun = _camera.transform.GetChild(2).gameObject;
            foreach (Transform child in gun.transform)
                child.gameObject.layer = LayerMask.NameToLayer("Local Gun");

            ////////////////////// For Debugging Purposes. Remove Later ////////////////////////
            _myCanvas.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = playerId;
            ////////////////////////////////////////////////////////////////////////////////////

            // Disable the Main Camera if there is one
            if (firstCamera.enabled)
            {
                firstCamera.enabled = false;
            }

            // Set Starting Point
            transform.position = GameManager.GetRespawn(playerId);
            // Set Team
            _myTeam = GameManager.GetTeam(playerId) ? team.blue : team.yellow;
        }
        else
        {
            ///////////////// Disable Components ////////////////
            // Disable Cameras
            CameraOnOff(_camera, false);
            _camera.transform.GetChild(0).GetComponent<AudioListener>().enabled = false;

            // Disable Canvas
            _myCanvas.SetActive(false);

            // Disable Unused Scripts
            gameObject.GetComponent<MoveMe>().enabled = false;
            gameObject.GetComponent<Shoot>().enabled = false;

            // Assign Remote Player Layer
            gameObject.layer = LayerMask.NameToLayer("Remote Player");
        }

        // The Yellow Team spawns facing the wrong direction
        if (_myTeam == team.yellow)
            transform.Rotate(0, 180, 0);

        // Disable Unused Canvas
        _deathCanvas.SetActive(false);
        _winLoseCanvas.SetActive(false);

    }

    // All Clients should "check-in" with the Game Manager
    public override void OnStartClient()
    {
        base.OnStartClient();
        string _id = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();
        playerId = GameManager.AddPlayer(_id, _player);
    }

    // The server will always be Player 1
    public override void OnStartServer()
    {
        base.OnStartServer();
        playerId = "Player 1";
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            // Set Health Text
            _myHealthText.text = health.ToString() + " HP";

            /////////////// For suicidal debugging purposes only. Remove Later!!! ////////
            if (Input.GetKeyDown(KeyCode.P))
                GetComponent<Shoot>().CmdPlayerShot(playerId, 5);
        }

        if (health <= 0 && isAlive)
        {
            isAlive = false;
            bool result = GameManager.CheckEndGame();
            if (!result)
            {
                PlayerDefeated();
                return;
            }
            //PlayerDefeated();
        }
    }

    private void PlayerDefeated()
    {
        EnableComponents(false);
        int time = GameManager.GetRespawnTime(playerId);
        GameManager.IncreaseRespawnTime(playerId);
        if (isLocalPlayer)
        {
            // Turn Game Canvas off during death
            _myCanvas.SetActive(false);
            // Turn on Death Canvas and set Death Text
            _deathCanvas.SetActive(true);
            _deathCanvas.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = KILLED_PREFIX + lastAttacker + "!";

            // Switch Camera from player view to 3rd person death camera
            CameraOnOff(_camera, false);
            deathCam = Instantiate(DeathCameraPrefab, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);

            StartCoroutine(Countdown(time));
        }
        Invoke("RespawnPlayer", time);
    }

    private void EnableComponents(bool setEnable)
    {
        // Enable or Disable components during death and respawn

        // Preventing and reinstating Movement and Shooting and visuals
        if (isLocalPlayer)
        {
            gameObject.GetComponent<MoveMe>().enabled = setEnable;
            gameObject.GetComponent<Shoot>().enabled = setEnable;
        }
        gameObject.GetComponent<Renderer>().enabled = setEnable;
        gameObject.GetComponent<CapsuleCollider>().enabled = setEnable;
        gameObject.GetComponent<CharacterController>().enabled = setEnable;
        // This disables the gun
        _camera.transform.GetChild(2).gameObject.SetActive(setEnable);
    }

    private void RespawnPlayer()
    {
        // Turn Canvas on during respawn
        if (isLocalPlayer)
        {
            _myCanvas.SetActive(true);
            _deathCanvas.SetActive(false);

            // Turn off death camera and turn on player camera
            Destroy(deathCam);
            CameraOnOff(_camera, true);
        }

        ResetPlayer();
    }

    private void CameraOnOff(GameObject camera, bool change)
    {
        foreach (Camera _cam in GetComponentsInChildren<Camera>())
            _cam.enabled = change;
    }

    private void ResetPlayer()
    {
        // Reset player position, rotation, and camera angle
        transform.position = GameManager.GetRespawn(playerId);
        Vector3 point;
        if (_myTeam == team.blue)
        {
            point = transform.position + Vector3.forward;
        }
        else
        {
            point = transform.position + Vector3.back;
        }
        transform.LookAt(point);
        _camera.transform.LookAt(point);

        health = MAX_HEALTH;
        transform.GetComponent<Shoot>().ResetBullets();
        EnableComponents(true);
        isAlive = true;

        // Reset Player's velocity
        GetComponent<MoveMe>().ResetPlayerVelocity();

        // Reset Player's Kill Streak Counter Text
        _myCanvas.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text = GetKillStatsText(GameManager.GetKillCount(playerId), GameManager.GetKillStreak(playerId));
    }

    public bool CheckAlive()
    {
        return isAlive;
    }

    public void SetAmmoText(int amount)
    {
        _myAmmoText.text = "Bullets: " + amount.ToString();
    }

    public void TakeDamage(int damage, string perpetrator)
    {
        health -= damage;
        Debug.Log(perpetrator + " shot " + transform.name + ". " + transform.name + " has " + health + " health remaining");

        // Check if the recovery coroutine is running. If it is, stop it
        if (RecovCo_running)
            StopCoroutine(RecoveryCoroutine);

        if (health <= 0)
        {
            //Debug.Log(gameObject.name + " has been killed by " + perpetrator);

            //// Award a kill to whoever shot you
            //GameManager.AwardKill(perpetrator);

            //// Your own streak is broken and returns to 0
            //GameManager.StreakBroken(playerId);
            //lastAttacker = perpetrator;
            RpcGiveKill(perpetrator, playerId);
        }
        else
        {
            // Then Start the Recovery Coroutine
            RecoveryCoroutine = RecoverHealth(RECOVERY_TIME);
            StartCoroutine(RecoveryCoroutine);
        }
    }

    public void KillAlert(string playerSlain)
    {
        if (isLocalPlayer)
        {
            _myCanvas.transform.GetChild(6).GetComponent<UnityEngine.UI.Text>().text = KILLER_PREFIX + playerSlain + "!";
            _myCanvas.transform.GetChild(6).gameObject.SetActive(true);
            _myCanvas.transform.GetChild(3).GetComponent<UnityEngine.UI.Text>().text =
                GetKillStatsText(GameManager.GetKillCount(playerId), GameManager.GetKillStreak(playerId));
            Invoke("TurnOffKillAlert", KILL_ALERT_DURATION);
        }
    }

    public string GetKillStatsText(int killcount, int killstreak)
    {
        return "Kill Count: " + killcount.ToString() + "\nKill Streak: " + killstreak.ToString();
    }

    public void TurnOffKillAlert()
    {
        _myCanvas.transform.GetChild(6).gameObject.SetActive(false);
    }

    public void GameOver(int result)
    {
        Debug.Log("Game Over has been called");
        if (isLocalPlayer)
        {
            switch (result)
            {
                case 0: break;
                case 1:
                case 2:
                default:
                    EnableComponents(false);
                    _myCanvas.SetActive(false);
                    _winLoseCanvas.SetActive(true);
                    CameraOnOff(_camera, false);
                    firstCamera.enabled = true;

                    if (_myTeam == team.blue && result == 1 || _myTeam == team.yellow && result == 2)
                    {
                        _winLoseCanvas.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "You Win!";
                    }
                    else if (result == 3)
                    {
                        _winLoseCanvas.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "You Tied!";
                    }
                    else
                    {
                        _winLoseCanvas.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "You Lose!";
                    }
                    break;
            }
        }
    }

    IEnumerator Countdown(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            _deathTimerText.text = counter.ToString();
            yield return new WaitForSeconds(1);
            counter--;
        }
    }

    IEnumerator RecoverHealth(float time)
    {
        RecovCo_running = true;
        // Players wait for a certain time to recover health, after which health recovers rapidly
        yield return new WaitForSeconds(time);
        health++;
        if (health < MAX_HEALTH)
        {
            RecoveryCoroutine = RecoverHealth(1);
            StartCoroutine(RecoveryCoroutine);
        }
        else
            RecovCo_running = false;
    }

    [ClientRpc]
    void RpcGiveKill(string perpetrator, string playerId)
    {
        Debug.Log(gameObject.name + " has been killed by " + perpetrator);

        lastAttacker = perpetrator;

        // Award a kill to whoever shot you
        GameManager.AwardKill(perpetrator, playerId);

        // Your own streak is broken and returns to 0
        GameManager.StreakBroken(playerId);
    }

    void OnDisable()
    {
        // Remove Player from registry
        GameManager.RemovePlayer(transform.name);

        // Enable a scene view camera
        if (firstCamera != null)
            firstCamera.enabled = true;
    }
}
