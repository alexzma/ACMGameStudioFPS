using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    // Dictionaries for saving player variables
    private const string ID_PREFIX = "Player ";
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    private static Dictionary<string, Vector3> respawnLocation = new Dictionary<string, Vector3>();
    private static Dictionary<string, int> killCount = new Dictionary<string, int>();
    private static Dictionary<string, int> killStreak = new Dictionary<string, int>();
    private static Dictionary<string, float> respawnTimer = new Dictionary<string, float>();

    // Constants
    static float MAX_TIME = 15f;                // The maximum revive time
    static float INITIAL_RESPAWN_TIMER = 5f;    // Initial respawn time
    static int MAX_PLAYER_COUNT = 6;

    // Pre-Game Barriers
    private bool preGame = true;
    public GameObject barrier;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (preGame && players.Count > 1)
            GameStart();
    }

    private void GameStart()
    {
        barrier.SetActive(false);
        preGame = false;
    }

    public static string AddPlayer(string netId, Player _player)
    {
        int currentPlayers = players.Count;
        for (int nextName = 1; nextName <= currentPlayers + 1; nextName++)
        {
            string player_name = ID_PREFIX + nextName.ToString();
            if (!players.ContainsKey(player_name))
            {
                players.Add(player_name, _player);
                _player.transform.name = player_name;
                killCount.Add(player_name, 0);
                killStreak.Add(player_name, 0);
                respawnTimer.Add(player_name, INITIAL_RESPAWN_TIMER);
                return player_name;
            }
        }

        return "Player 0";
    }

    public static void RemovePlayer(string playerId)
    {
        players.Remove(playerId);
        killCount.Remove(playerId);
        respawnTimer.Remove(playerId);
        killCount.Remove(playerId);
        killStreak.Remove(playerId);
    }

    public static Player GetPlayer(string playerId)
    {
        return players[playerId];
    }

    public static Vector3 GetRespawn(string playerId)
    {
        return respawnLocation[playerId];
    }

    public static bool GetTeam(string playerId)
    {
        return (respawnLocation[playerId].z < 0);
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

        players[Killer].KillAlert(killed);
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
        bool team1lost = true;
        bool team2lost = true;
        int gameResult = 0;
        
        foreach(KeyValuePair<string, Player> pair in players)
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

        foreach (KeyValuePair<string, Player> pair2 in players)
            pair2.Value.GameOver(gameResult);

        return true;
    }
}
