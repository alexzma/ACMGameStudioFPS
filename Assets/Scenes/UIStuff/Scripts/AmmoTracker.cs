using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoTracker : MonoBehaviour
{
    public Text text;
    public int starting_ammo = 16;

    private int m_ammo;
    // Start is called before the first frame update
    void Start()
    {
        m_ammo = starting_ammo;
    }

    // Update is called once per frame
    void Update()
    {}

    public void SetAmmo(int ammo)
	{
        text.text = "Ammo: " + ammo.ToString() + " Bullets";
        m_ammo = ammo;
	}

    public void DecrementAmmo()
	{
        m_ammo--;
        text.text = "Ammo: " + m_ammo.ToString() + " Bullets";
	}
}
