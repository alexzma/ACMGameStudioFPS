using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathCanvasScript : MonoBehaviour
{
    public Text killerText;
    public Text respawnText;

    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {}

    public void SetKiller(string killer)
	{
        killerText.text = "You were killed by " + killer;
	}

    public void SetRespawnTime(float time)
	{
        respawnText.text = time.ToString() + " seconds until respawn";
	}
}
