using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class CustomNetworkManager : NetworkManager
{
    public GameObject yellow_playerModel;
    public GameObject blue_playerModel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override public void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (yellow_playerModel == null || blue_playerModel == null)
        {
            if (LogFilter.logError) { Debug.LogError("The PlayerPrefab is empty on the NetworkManager. Please setup a PlayerPrefab object."); }
            return;
        }

        if (yellow_playerModel.GetComponent<NetworkIdentity>() == null || blue_playerModel.GetComponent<NetworkIdentity>() == null)
        {
            if (LogFilter.logError) { Debug.LogError("The PlayerPrefab does not have a NetworkIdentity. Please add a NetworkIdentity to the player prefab."); }
            return;
        }

        if (playerControllerId < conn.playerControllers.Count && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
        {
            if (LogFilter.logError) { Debug.LogError("There is already a player at that playerControllerId for this connections."); }
            return;
        }

        GameObject player;
        int team;
        string player_name = GameManagerScript.AddPlayer(out team);

        if (team == -1)
        {
            Debug.Log("Error instantiating Player");
            GameManagerScript.RemovePlayer(player_name);
        }

        Debug.Log(player_name);

        player = (GameObject)Instantiate(team == 0 ? yellow_playerModel : blue_playerModel, GameManagerScript.GetRespawn(player_name), team == 0 ? Quaternion.Euler(0,180,0) : Quaternion.identity);
        player.SetActive(true);

        GameManagerScript.AddPlayerScript(player_name, player.GetComponent<PlayerScript>());
        player.GetComponent<PlayerScript>().playerId = player_name;
        player.transform.name = player_name;

        // Set Team
        player.GetComponent<PlayerScript>().SetTeam(player_name);

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }
}
