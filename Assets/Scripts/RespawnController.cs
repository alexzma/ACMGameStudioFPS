using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player controller must have Respawn function and some kind of Die function
//Attach this to an empty GameObject
public class RespawnController : MonoBehaviour
{
    private static RespawnController _instance;
    public static RespawnController Instance { get { return _instance; } }
    private Dictionary<GameObject, RespawnPlayerObj> players;
    private List<RespawnPlayerObj> team1;
    private List<RespawnPlayerObj> team2;
    private bool team1AllDead;
    private bool team2AllDead;
    private void Awake()
    {
        //singleton design pattern
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        //easy way to access respawnplayerobj
        players = new Dictionary<GameObject, RespawnPlayerObj>();
        //list of respawnplayerobj for teams
        team1 = new List<RespawnPlayerObj>();
        team2 = new List<RespawnPlayerObj>();
    }
    public void Start()
    {
    }
    //Call this function at Start in playercontroller
    public void RegisterPlayer(GameObject player, int teamNumber)
    {
        if (!players.ContainsKey(player))
        {
            players[player] = new RespawnPlayerObj(player);
            if (teamNumber == 1)
                team1.Add(players[player]);
            else
                team2.Add(players[player]);
        }
    }
    //Call this function when player dies
    public void TellRespawnPlayerDied(GameObject player)
    {
        players[player].PlayerDiedAndUpdateTime();
    }
    //Call update function on the class RespawnPlayerObj
    //This will check to see if all players on a team are dead
    public void Update()
    {
        team1AllDead = true;
        team2AllDead = true;
        foreach (RespawnPlayerObj player in team1)
        {
            player.Update();
            //player.DebugInfo();
            team1AllDead = team1AllDead && player.isPlayerDead();
        }
        foreach (RespawnPlayerObj player in team2)
        {
            player.Update();
            team2AllDead = team2AllDead && player.isPlayerDead();
        }
        if (team1AllDead)
        {
            //Debug.Log("team 1 is all dead");
            //TODO: do something when team loses, like tell game controller
        }
        if (team2AllDead)
        {
            //Debug.Log("team 2 is all dead");
            //TODO: do something when team loses
        }
    }
    public float GetRespawnTime(GameObject player)
    {
        return players[player].GetRespawnTime();
    }

    //class created to keep track of times
    private class RespawnPlayerObj
    {
        private float respawnTime;
        private float currentTime;
        private bool isDead;
        private int currentDeaths;
        private GameObject playerGameObj;
        //constructor
        public RespawnPlayerObj(GameObject player)
        {
            respawnTime = 1f;
            currentDeaths = 0;
            playerGameObj = player;

        }
        //this updates time and houses the "formula" to update respawntimes
        public void PlayerDiedAndUpdateTime()
        {
            isDead = true;
            currentDeaths++;
            if (currentDeaths < 5)
                respawnTime = currentDeaths * 1.5f;
            else if (currentDeaths < 10)
                respawnTime = currentDeaths * 2.0f;
            else
                respawnTime = currentDeaths * 2.5f;
        }

        public bool isPlayerDead()
        {
            return isDead;
        }
        //thiscalls the respawn function for the player game obj
        private void Respawn()
        {
            //make the player respawn
            playerGameObj.GetComponent<PlayerController>().Respawn();
            isDead = false;

        }
        public float GetRespawnTime()
        {
            return respawnTime;
        }
        //this waits the respawnTime amount of seconds
        public void Update()
        {
            if (isDead)
            {
                currentTime += Time.deltaTime;

                //it is finished waiting out respawn time
                if (currentTime >= respawnTime)
                {
                    this.Respawn();
                    currentTime = 0f;
                }
            }


        }
        public void DebugInfo()
        {
            Debug.Log("My name is: " + playerGameObj.name);
            Debug.Log("Respawn Time: " + respawnTime);
            Debug.Log("Is dead: " + isDead);
        }
    }
}
