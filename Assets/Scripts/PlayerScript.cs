using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

enum Team : ushort { yellow, blue }

public class PlayerScript : NetworkBehaviour
{
    // Player Variables
    [SyncVar] public string playerId = "Player 0";
    [SyncVar] private int health;
    public int MAX_HEALTH = 10;
    public float RECOVERY_TIME = 10f;
    //public float KILL_ALERT_DURATION = 3f;

    private Team _myTeam;
    private bool invincible = false;
    private bool isAlive = true;
    private string lastAttacker = ""; // Last person to shoot you
    private float MAX_SENSITIVITY;

    // Cameras
    public GameObject DeathCameraPrefab;
    private Camera firstCamera;     // Main Camera
    private GameObject _camera;     // My Camera
    private GameObject deathCam;

    // Canvas
    public GameObject _canvas;
    private GameObject _myCanvas;
    public GameObject _deathCanvas;
    private GameObject _myDeathCanvas;
    public Activator _activator;
    private Activator _myActivator;
    public GameObject winCanvas;
    public GameObject loseCanvas;
    private GameObject resultsCanvas;
    private TeamTracker _teamTracker;

    // Coroutines
    private IEnumerator RecoveryCoroutine;
    private bool RecovCo_running = false;

    // Method Declarations
    /*
     * private void Awake();
     * void Start();
     * void Update();
     *
     * public int GetHealth();
     * public bool GetInvicible();
     * public bool SetTeam(string);
     * public bool CheckAlive();
     * public void SetAmmoText(int);
     * public IEnumerator AddTeamMember(string, int);
     * public void AddTeamMemberWrapper(string, int);
     * public void UpdateTeamHealth();
     * public void RemoveTeamMember(string);
     * public void KillAlert(string, string);
     * public void KillStreakAlert(string, int);
     * public void GameOver(int);
     * [ClientRpc] public void RpcTakeDamage(int, string);
     * - [ClientRpc] public void RpcAwardKill(string, string);
     * - [Command] public void CmdAwardKill(string, string);
     * public void ChangeCharVisible(bool set)
     * private EnableComponents(bool);
     * private PlayerDefeated();
     * private RespawnPlayer();
     * private ResetPlayer();
     * private IEnumerator RecoverHealth(float);
     * private IEnumerator Countdown(int);
     * private IEnumerator MakeVisible(float time)
     * 
     * void OnDisable();
     */

    private void Awake()
    {
        // Start Canvas GameObjects
        _myCanvas = Instantiate(_canvas);
        _myActivator = Instantiate(_activator);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Assign Proper Variables
        _camera = GetComponentInChildren<Camera>().gameObject;

        // Enable proper cameras
        _camera.GetComponent<Camera>().enabled = true;
        _camera.GetComponent<AudioListener>().enabled = true;
        
        // The Yellow Team spawns facing the wrong direction
        //if (_myTeam == Team.yellow)
        //    transform.Rotate(0, 180, 0);

        // Set Player Health
        health = MAX_HEALTH;

        // Character invincible on startup
        invincible = true;
        //Debug.Log("Character is invincible for 2 sec on spawn");
        Invoke("RemoveInvicibility", 2f);

        // Start Spawn Effect
        StartCoroutine(GetComponent<SpawnEffectController>().RespawnEffect(2f));

        if (isLocalPlayer)
        {
            // Save the Main Camera
            firstCamera = Camera.main;
            // Disable the Main Camera if there is one
            if (firstCamera.enabled)
            {
                firstCamera.enabled = false;
                firstCamera.GetComponentInParent<AudioListener>().enabled = false;
                firstCamera.transform.Find("Canvas").gameObject.SetActive(false);
            }

            // Assign Canvas GameObjects
            _myCanvas.SetActive(true);
            _myActivator.enabled = true;
            _myCanvas.GetComponent<KeyListener>().activator = _myActivator;
            _myCanvas.GetComponent<KeyListener>()._playerScript = this;
            _myCanvas.GetComponent<LifeTracker>().activator = _myActivator;
            _myDeathCanvas = Instantiate(_deathCanvas);
            _myDeathCanvas.SetActive(false);
            //_myCanvas.GetComponent<LifeTracker>().deathCanvas = Instantiate(_deathCanvas);
            //_myDeathCanvas = _myCanvas.GetComponent<LifeTracker>().deathCanvas;
            _teamTracker = _myCanvas.GetComponent<TeamTracker>();

            // Set Proper Health
            _myCanvas.GetComponent<LifeTracker>().SetHP(health);

            // Assign to Local Player Layer
            gameObject.layer = LayerMask.NameToLayer("Local Player");

            // Set Starting Point
            transform.position = GameManagerScript.GetRespawn(playerId);

            MAX_SENSITIVITY = GetComponentInChildren<CameraLook>().mouseSensitivity;

            // Play Game Music
            //GameObject _music2 = GameObject.FindWithTag("Music2");
            //_music2.GetComponent<Music>().PlayMusic();
        }
        else
        {
            // Disable Camera
            _camera.SetActive(false);

            // Assign Remote Player Layer
            gameObject.layer = LayerMask.NameToLayer("Remote Player");

            // Disable Unused Scripts
            GetComponent<PlayerControllerFSM>().enabled = false;
            GetComponentInChildren<CameraLook>().enabled = false;
            //GetComponent<Shooting>().enabled = false;

            // Destroy unused objects
            Destroy(_myCanvas);
            Destroy(_myActivator);
        }
    }

    // All Clients should "check-in" with the Game Manager
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isServer)
        {
            // Need to test this part with mulitple computers during disconnection and reconnection to see when the
            // playerId is updated (before or after OnStartClient()
            // If playerId is updated before
            if (playerId != "Player 0")
                GameManagerScript.AddPlayer(this, playerId);
            else
                playerId = GameManagerScript.AddPlayer(this);
        }

        // Otherwise do some sort of check afterwards

        //playerId = GameManagerScript.AddPlayer(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (health <= 0 && isAlive)
        {
            isAlive = false;
            bool result = GameManagerScript.CheckEndGame();
            if (!result)
            {
                PlayerDefeated();
                return;
            }
        }

        if (isLocalPlayer)
        {
            /////////////// For suicidal debugging purposes only. Remove Later!!! ////////
            if (Input.GetKeyDown(KeyCode.P))
                gameObject.GetComponent<Shooting>().CmdPlayerShot(playerId, 2);
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public bool GetInvicible()
    {
        return invincible;
    }

    public void SetTeam(string player_name)
    {
        _myTeam = GameManagerScript.GetTeam(player_name) ? Team.blue : Team.yellow;
    }

    public bool CheckAlive()
    {
        return isAlive;
    }

    public void SetAmmoText(int amount)
    {
        _myCanvas.GetComponent<AmmoTracker>().SetAmmo(amount);
    }

    public IEnumerator AddTeamMember(string playerName, int health)
    {
        yield return new WaitForSeconds(0);
        if (isLocalPlayer)
        {
            _teamTracker.AddPlayer(playerName);
            _teamTracker.SetHealth(playerName, (float)health);
        }
    }

    public void AddTeamMemberWrapper(string playerName, int health)
    {
        StartCoroutine(AddTeamMember(playerName, health));
    }

    public void UpdateTeamHealth()
    {
        if (isLocalPlayer)
        {
            List<string> teammates;
            _teamTracker.GetPlayerNames(out teammates);
            for (int i = 0; i != teammates.Count; i++)
            {
                // Update Teammates' health
                _teamTracker.SetHealth(teammates[i], GameManagerScript.GetPlayerHealth(teammates[i]));
            }
        }
    }

    public void RemoveTeamMember(string playerName)
    {
        if (isLocalPlayer)
        {
            _teamTracker.RemovePlayer(playerName);
        }
    }

    public void KillAlert(string killer, string killed)
    {
        if (isLocalPlayer)
        {
            AppendText _appendText = _myCanvas.GetComponent<LifeTracker>().appendText;
            if (_appendText == null)
                _appendText = _myCanvas.transform.Find("Scroll View").transform.GetChild(0).GetChild(0).GetComponent<AppendText>();

            _appendText.appendKill(killer, killed);
        }
    }

    public void KillStreakAlert(string killer, int streak)
    {
        AppendText _appendText = _myCanvas.GetComponent<LifeTracker>().appendText;
        if (isLocalPlayer)
        {
            if (_appendText == null)
                _appendText = _myCanvas.transform.Find("Scroll View").transform.GetChild(0).GetChild(0).GetComponent<AppendText>();

            _appendText.killStreak(killer, streak);
        }
    }

    public void GameOver(int result)
    {
        //Debug.Log("Game Over has been called");
        if (isLocalPlayer)
        {
            switch (result)
            {
                case 0: break;
                case 1:
                case 2:
                default:
                    EnableComponents(false);
                    GetComponent<Shooting>().enabled = false;
                    _myCanvas.SetActive(false);

                    _camera.SetActive(false);


                    // I'll just turn on this camera for now
                    firstCamera.enabled = true;

                    if (_myTeam == Team.blue && result == 1 || _myTeam == Team.yellow && result == 2)
                    {
                        // Convert to Win Scene
                        resultsCanvas = Instantiate(winCanvas);
                    }
                    else if (result == 3)
                    {
                        // Convert to Tie Scene?
                        resultsCanvas = Instantiate(loseCanvas);
                    }
                    else
                    {
                        // Convert to Lose Scene
                        resultsCanvas = Instantiate(winCanvas);
                    }
                    resultsCanvas.SetActive(true);
                    // Turn off regular music

                    // Turn on after-game music
                    GameObject _music = GameObject.FindWithTag("Music");
                    if (_music == null)
                        Debug.Log("Music maker not found");
                    else
                    {
                        firstCamera.GetComponent<AudioListener>().enabled = true;
                        _music.GetComponent<Music>().ChangeSecondary(0.7f);
                        _music.GetComponent<Music>().ChangeMusic();
                    }
                    break;
            }
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage, string perpetrator)
    {
        health -= damage;
        if (isLocalPlayer)
        {
            _myCanvas.GetComponent<LifeTracker>().SetHP(health);
            GetComponent<AudioManager>().Play("Hit");
        }
        lastAttacker = perpetrator;

        // Check if the recovery coroutine is running. If it is, stop it
        if (RecovCo_running)
            StopCoroutine(RecoveryCoroutine);

        if (health <= 0)
        {
            //if (isLocalPlayer)
            //    CmdAwardKill(perpetrator, playerId);
            //else
            //    GameManagerScript.GetPlayer(GameManagerScript.localPlayer).CmdAwardKill(perpetrator, playerId);
            /*
            if (isLocalPlayer)
                KillAlert(perpetrator, playerId);
            else
                GameManagerScript.GetPlayer(GameManagerScript.localPlayer).KillAlert(perpetrator, playerId);
            */
            GameManagerScript.AwardKill(perpetrator, playerId);
            GameManagerScript.StreakBroken(playerId);
        }
        else
        {
            // Then Start the Recovery Coroutine
            RecoveryCoroutine = RecoverHealth(RECOVERY_TIME);
            StartCoroutine(RecoveryCoroutine);
        }
    }

    //[Command]
    //public void CmdAwardKill(string perpetrator, string playerId)
    //{
    //    RpcGiveKill(perpetrator, playerId);
    //}

    //[ClientRpc]
    //public void RpcGiveKill(string perpetrator, string playerId)
    //{
    //    Debug.Log(gameObject.name + " has been killed by " + perpetrator);

    //    //lastAttacker = perpetrator;

    //    // Award a kill to whoever shot you
    //    GameManagerScript.AwardKill(perpetrator, playerId);

    //    // Your own streak is broken and returns to 0
    //    GameManagerScript.StreakBroken(playerId);
    //}

    public void ChangeCharVisible(bool set)
    {
        foreach (SkinnedMeshRenderer _renderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            _renderer.enabled = set;
    }

    public void Sensitivity(float value)
    {
        GetComponentInChildren<CameraLook>().mouseSensitivity = MAX_SENSITIVITY * value / 10;
    }

    private void EnableComponents(bool setEnable)
    {
        // Enable or Disable components during death and respawn

        // Preventing and reinstating Movement and Shooting and visuals
        if (isLocalPlayer)
        {
            gameObject.GetComponent<PlayerControllerFSM>().enabled = setEnable;
            //gameObject.GetComponent<Shooting>().enabled = setEnable;
        }

        if (setEnable == true)
        {
            ChangeCharVisible(true);
        }
        else
        {
            StartCoroutine(GetComponent<SpawnEffectController>().DespawnEffect(2f));
            StartCoroutine(MakeVisible(2f));
        }
        
        gameObject.GetComponent<CapsuleCollider>().enabled = setEnable;
        gameObject.GetComponent<Rigidbody>().useGravity = setEnable;
        transform.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/Jetpack").gameObject.SetActive(setEnable);
        transform.Find("mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/Rifle").gameObject.SetActive(setEnable);
    }

    private void PlayerDefeated()
    {
        /*
         * for this to work we need to change how this is implemented, can't fix in an hour
        StartCoroutine(GetComponent<SpawnEffectController>().DespawnEffect(2f));
        StartCoroutine(WaitSecs(2f));
        Debug.Log("Finished waiting");
        */
        EnableComponents(false);
        int time = GameManagerScript.GetRespawnTime(playerId);
        GameManagerScript.IncreaseRespawnTime(playerId);
        if (isLocalPlayer)
        {
            // Turn Game Canvas off during death
            _myCanvas.SetActive(false);
            // Turn on Death Canvas and set Death Text
            _myDeathCanvas.SetActive(true);
            if (lastAttacker == "")
                Debug.Log("lastAttacker not recorded");
            else
            {
                _myDeathCanvas.GetComponent<DeathCanvasScript>().SetKiller(lastAttacker);
                lastAttacker = "";
            }

            // Switch Camera from player view to 3rd person death camera
            _camera.SetActive(false);
            deathCam = Instantiate(DeathCameraPrefab, transform.position, Quaternion.identity);

            StartCoroutine(Countdown(time));
        }

        Invoke("RespawnPlayer", time);
    }

    private void RespawnPlayer()
    {
        // Turn Canvas on during respawn
        if (isLocalPlayer)
        {
            _myCanvas.SetActive(true);
            _myDeathCanvas.SetActive(false);

            // Turn off death camera and turn on player camera
            Destroy(deathCam);
            _camera.SetActive(true);
        }

        ResetPlayer();
        invincible = true;
        Invoke("RemoveInvicibility", 2f);
        StartCoroutine(GetComponent<SpawnEffectController>().RespawnEffect(2f));
    }

    private void ResetPlayer()
    {
        // Reset player position, rotation, and camera angle
        transform.position = GameManagerScript.GetRespawn(playerId);
        Vector3 point;
        if (_myTeam == Team.blue)
        {
            point = transform.position + Vector3.forward;
        }
        else
        {
            point = transform.position + Vector3.back;
        }
        transform.LookAt(point);

        health = MAX_HEALTH;
        EnableComponents(true);
        isAlive = true;

        if (isLocalPlayer)
        {
            _myCanvas.GetComponent<LifeTracker>().SetHP(health);
            GetComponent<Shooting>().ResetBullets();
        }

        // Reset Player's Kill Streak Counter Text
        // TODO Adjust this part
    }

    private void RemoveInvicibility()
    {
        invincible = false;
    }

    private IEnumerator RecoverHealth(float time)
    {
        RecovCo_running = true;
        // Players wait for a certain time to recover health, after which health recovers rapidly
        yield return new WaitForSeconds(time);
        health++;
        if (isLocalPlayer)
            _myCanvas.GetComponent<LifeTracker>().SetHP(health);
        if (health < MAX_HEALTH)
        {
            RecoveryCoroutine = RecoverHealth(1);
            StartCoroutine(RecoveryCoroutine);
        }
        else
            RecovCo_running = false;
    }

    private IEnumerator Countdown(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            _myDeathCanvas.GetComponent<DeathCanvasScript>().SetRespawnTime(counter);
            yield return new WaitForSeconds(1);
            counter--;
        }
    }

    void OnDisable()
    {
        // Remove Player from registry
        GameManagerScript.RemovePlayer(transform.name);

        // Enable a scene view camera
        if (firstCamera != null)
            firstCamera.enabled = true;
    }

    private IEnumerator MakeVisible(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeCharVisible(false);
    }
}
