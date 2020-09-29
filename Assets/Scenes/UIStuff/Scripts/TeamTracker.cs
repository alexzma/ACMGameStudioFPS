using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTracker : MonoBehaviour
{
    public GameObject playerContainer;
    public GameObject playerPortrait;

    private Dictionary<string, GameObject> playerPortraits;

    // Start is called before the first frame update
    void Start()
    {
        playerPortraits = new Dictionary<string, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {}

    public void AddPlayer(string name)
	{
        if (playerPortraits.ContainsKey(name))
        {
            return;
        }
        GameObject player = Instantiate(playerPortrait, playerContainer.GetComponent<Transform>());
        PortraitScript portraitScript = player.GetComponent<PortraitScript>();
        portraitScript.SetName(name);
        playerPortraits.Add(name, player);
	}

    public void RemovePlayer(string name)
	{
        Destroy(playerPortraits[name]);
        playerPortraits.Remove(name);
	}

    public void SetHealth(string name, float health)
	{
        PortraitScript portraitScript = playerPortraits[name].GetComponent<PortraitScript>();
        portraitScript.SetHealth(health);
	}

    public void GetPlayerNames(out List<string> list)
    {
        list = new List<string>(playerPortraits.Keys);
    }

}
