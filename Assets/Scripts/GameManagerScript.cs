using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    // Dictionaries for saving player variables
    private const string ID_PREFIX = "Player ";
    private static Dictionary<string, PlayerScript> players = new Dictionary<string, PlayerScript>();
    private static Dictionary<string, Vector3> respawnLocation = new Dictionary<string, Vector3>();
    private static Dictionary<string, int> killCount = new Dictionary<string, int>();
    private static Dictionary<string, int> killStreak = new Dictionary<string, int>();
    private static Dictionary<string, float> respawnTimer = new Dictionary<string, float>();

    private static List<string> team1 = new List<string> { "Player 1", "Player 3", "Player 5", "Player 7" };
    private static List<string> team2 = new List<string> { "Player 2", "Player 4", "Player 6", "Player 8" };
    private static List<string> allTeam = new List<string> { "Player 1", "Player 2", "Player 3", "Player 4", "Player 5", "Player 6", "Player 7", "Player 8" };

    // For Testing purposes only. Change back when finished.
    //private static List<string> team1 = new List<string> { "Player 1", "Player 2", "Player 3", "Player 4" };
    //private static List<string> team2 = new List<string> { "Player 5", "Player 6", "Player 7", "Player 8" };

    // Constants
    static float MAX_TIME = 15f;                // The maximum revive time
    static float INITIAL_RESPAWN_TIMER = 5f;    // Initial respawn time
    static int MAX_PLAYER_COUNT = 8;

    // Pre-Game Barriers
    public GameObject barrier;

    // Save Local Player Name
    public static string localPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // Team 1 Respawn Location
        respawnLocation.Add(ID_PREFIX + "1", new Vector3(4.5f, 1.25f, -72f));
        respawnLocation.Add(ID_PREFIX + "3", new Vector3(-4.5f, 1.25f, -72f));
        respawnLocation.Add(ID_PREFIX + "5", new Vector3(4.5f, 1.25f, -68f));
        respawnLocation.Add(ID_PREFIX + "7", new Vector3(-4.5f, 1.25f, -68f));

        // Team 2 Respawn Locations
        respawnLocation.Add(ID_PREFIX + "2", new Vector3(-4.5f, 1.25f, 92f));
        respawnLocation.Add(ID_PREFIX + "4", new Vector3(4.5f, 1.25f, 92f));
        respawnLocation.Add(ID_PREFIX + "6", new Vector3(-4.5f, 1.25f, 88f));
        respawnLocation.Add(ID_PREFIX + "8", new Vector3(4.5f, 1.25f, 88f));


        // Find Local Player
        Debug.Log("Searching for local player");
        StartCoroutine(FindLocalPlayer());

        StartCoroutine(GameStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            barrier.SetActive(false);
    }

    public static string AddPlayer(PlayerScript _player)
    {
        int currentPlayers = players.Count;
        for (int nextName = 1; nextName <= currentPlayers + 1; nextName++)
        {
            string player_name = ID_PREFIX + nextName.ToString();
            if (!players.ContainsKey(player_name))
            {
                AddPlayerInternal(player_name, _player);

                return player_name;
            }
        }

        return "Player 0";
    }

    public static bool AddPlayer(PlayerScript _player, string player_name)
    {
        if (players.ContainsKey(player_name))
        {
            AddPlayer(_player);
            Debug.Log("Tried to add player with existing Id");
            return false;
        }

        AddPlayerInternal(player_name, _player);
        
        return true;
    }

    private static void AddPlayerInternal(string player_name, PlayerScript _player)
    {
        players.Add(player_name, _player);
        _player.transform.name = player_name;
        killCount.Add(player_name, 0);
        killStreak.Add(player_name, 0);
        respawnTimer.Add(player_name, INITIAL_RESPAWN_TIMER);

        // Setup Team Tab
        List<string> possibleTeam = GetTeamMembers(player_name);
        if (possibleTeam != null)
        {
            for (int i = 0; i != possibleTeam.Count; i++)
            {
                string player = possibleTeam[i];
                if (players.ContainsKey(player))
                {
                    // Give this Player their teammates
                    if (player_name == player)
                        _player.AddTeamMemberWrapper(player, _player.MAX_HEALTH);
                    else
                        _player.AddTeamMemberWrapper(player, players[player].GetHealth());

                    // Give teammates this player, but skip yourself
                    if (player_name == player)
                        continue;
                    players[player].AddTeamMemberWrapper(player_name, _player.MAX_HEALTH);
                }
            }
        }
        else
            Debug.Log("Error while searching for teammates");
    }

    public static string AddPlayer(out int whichTeam)
    {
        int currentPlayers = players.Count;
        for (int nextName = 1; nextName <= currentPlayers + 1; nextName++)
        {
            string player_name = ID_PREFIX + nextName.ToString();
            if (!players.ContainsKey(player_name))
            {
                //players.Add(player_name, _player);
                //_player.transform.name = player_name;
                players.Add(player_name, null);
                killCount.Add(player_name, 0);
                killStreak.Add(player_name, 0);
                respawnTimer.Add(player_name, INITIAL_RESPAWN_TIMER);
                whichTeam = nextName % 2;
                return player_name;
            }
        }
        whichTeam = -1;
        return "Player 0";
    }

    public static bool AddPlayerScript(string player_name, PlayerScript _playerScript)
    {
        if (!players.ContainsKey(player_name))
            return false;

        players[player_name] = _playerScript;
        _playerScript.transform.name = player_name;
        _playerScript.playerId = player_name;

        // Setup Team Tab
        List<string> possibleTeam = GetTeamMembers(player_name);
        if (possibleTeam != null)
        {
            for (int i = 0; i != possibleTeam.Count; i++)
            {
                string player = possibleTeam[i];
                if (players.ContainsKey(player))
                {
                    // Give this Player their teammates
                    if (player_name == player)
                        _playerScript.AddTeamMemberWrapper(player, _playerScript.MAX_HEALTH);
                    else
                        _playerScript.AddTeamMemberWrapper(player, players[player].GetHealth());

                    // Give teammates this player, but skip yourself
                    if (player_name == player)
                        continue;
                    players[player].AddTeamMemberWrapper(player_name, _playerScript.MAX_HEALTH);
                }
            }
        }
        else
            Debug.Log("Error while searching for teammates");

        return true;
    }

    public static void RemovePlayer(string playerId)
    {
        players.Remove(playerId);
        killCount.Remove(playerId);
        respawnTimer.Remove(playerId);
        killCount.Remove(playerId);
        killStreak.Remove(playerId);
        RemoveMember(playerId);
    }

    public static PlayerScript GetPlayer(string playerId)
    {
        return players[playerId];
    }

    public static string GetName(PlayerScript _playerScript)
    {
        foreach (string _name in allTeam)
        {
            if (players.ContainsKey(_name) && players[_name] == _playerScript)
                return _name;
        }
        return "Player 0";
    }

    public static Vector3 GetRespawn(string playerId)
    {
        return respawnLocation[playerId];
    }

    public static bool GetTeam(string playerId)
    {
        // True is blue and False is yellow
        return (respawnLocation[playerId].z < 0);
    }

    public static int GetPlayerHealth(string playerId)
    {
        return players[playerId].GetHealth();
    }

    public static int GetKillCount(string playerId)
    {
        return killCount[playerId];
    }

    public static int GetKillStreak(string playerId)
    {
        return killStreak[playerId];
    }

    public static int GetRespawnTime(string playerId)
    {
        return (int)System.Math.Floor(respawnTimer[playerId]);
    }

    public static void IncreaseRespawnTime(string playerId)
    {
        float newTime = respawnTimer[playerId];
        if (newTime == 15)
            return;
        else if (newTime >= 15)
            respawnTimer[playerId] = 15;
        else
        {
            // Wait Time set to +1 second per death
            newTime += 1;
            if (newTime > MAX_TIME)
                respawnTimer[playerId] = MAX_TIME;
            else
                respawnTimer[playerId] = newTime;
        }
    }

    public static void StreakBroken(string playerId)
    {
        killStreak[playerId] = 0;

        Debug.Log(playerId + " has been slain. His kill streak is now 0.");
    }

    public static void AwardKill(string Killer, string killed)
    {
        killCount[Killer]++;
        killStreak[Killer]++;

        Debug.Log(Killer + "'s kill count is now " + killCount[Killer]);
        Debug.Log(Killer + "'s kill streak is now " + killStreak[Killer]);

        if (localPlayer == null)
            Debug.Log("Local Player is null");
        else
        {
            players[localPlayer].KillAlert(Killer, killed);
            int streak = killStreak[Killer];
            if (streak % 5 == 0)
                players[localPlayer].KillStreakAlert(Killer, streak);
        }
    }

    public static void RemoveMember(string playerId)
    {
        // Remove the player from the team of his teammates
        List<string> possibleTeam = GetTeamMembers(playerId);
        if (possibleTeam != null)
        {
            for (int i = 0; i != possibleTeam.Count; i++)
            {
                string player = possibleTeam[i];
                if (players.ContainsKey(player))
                    players[player].RemoveTeamMember(playerId);
            }
        }
    }

    public static bool CheckEndGame()
    {
        /* Default lost status to true, then check for living players
        // Yields the following states
        // 0 - A team has not won
        // 1 - Team 1 has won
        // 2 - Team 2 has won
        // 3 - There has been a tie (Currently only possible if 1v1 and both players die within 1 frame)
        */

        if (players.Count <= 1)
            return false;

        bool team1lost = true;
        bool team2lost = true;
        int gameResult = 0;

        foreach (KeyValuePair<string, PlayerScript> pair in players)
        {
            if (pair.Value.CheckAlive())
            {
                if (GetTeam(pair.Key))
                    team1lost = false;
                else
                    team2lost = false;
            }

            if (!team1lost && !team2lost)       // Game result is 0
                return false;
        }

        if (team1lost ^ team2lost)
            gameResult = team2lost ? 1 : 2;
        else
            gameResult = 3;

        foreach (KeyValuePair<string, PlayerScript> pair2 in players)
            pair2.Value.GameOver(gameResult);

        return true;
    }

    public void ChangeSensitivity(Slider _slider)
    {
        //Debug.Log("Slider value is " + _slider.value);
        players[localPlayer].Sensitivity(_slider.value);
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1);
        if (players.Count > 1)
        {
            barrier.SetActive(false);
        }
        else
            StartCoroutine(GameStart());
    }

    private IEnumerator FindLocalPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        // Check if transform.name matches playerId
        foreach (string _name in allTeam)
        {
            if (players.ContainsKey(_name) && players[_name].transform.name != _name)
                players[_name].transform.name = _name;
        }

        // Check for Local Player
        foreach (string _name in allTeam)
            if (players.ContainsKey(_name) && players[_name].isLocalPlayer)
            {
                localPlayer = _name;
                Debug.Log("Local Player found: " + _name);
                yield break;
            }

        Debug.Log("Local Player Not Found");
        StartCoroutine(FindLocalPlayer());
    }

    private static List<string> GetTeamMembers(string playerId)
    {
        if (team1.Contains(playerId))
            return team1;
        else if (team2.Contains(playerId))
            return team2;
        else
            return null;
    }
}
