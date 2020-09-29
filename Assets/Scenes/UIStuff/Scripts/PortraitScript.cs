using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitScript : MonoBehaviour
{
    public Text text;

    public LifeTracker lifeTracker;
    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {}

    public void SetName(string name)
	{
        text.text = name;
	}

    public void SetHealth(float health)
	{
        lifeTracker.SetHP(health);
	}
}
