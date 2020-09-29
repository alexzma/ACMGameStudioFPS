using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeTracker : MonoBehaviour
{
    public Text hpText;
    public Slider slider;
    public GameObject deathCanvas;
    public Image healthbar;
    public bool isPlayer;
    public GameObject blood1;
    public GameObject blood2;
    public Activator activator;
    public AppendText appendText;

    private float hp;
    private float max_hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = slider.maxValue;
        max_hp = hp;

        appendText = GameObject.Find("Scroll View").transform.GetChild(0).GetChild(0).GetComponent<AppendText>();
    }

    void UpdateUI()
	{
        hpText.text = $"{hp} HP";
        slider.value = hp;
        if (hp / max_hp > 0.75f)
        {
            healthbar.color = new Color32(0, 255, 0, 255);
            if (isPlayer)
            {
                activator.Deactivate(blood1);
                activator.Deactivate(blood2);
            }
        }
        else if (hp / max_hp < 0.75f && hp / max_hp > 0.25f)
        {
            healthbar.color = new Color32(255, 255, 0, 255);
            if (isPlayer)
            {
                activator.Activate(blood1);
                activator.Deactivate(blood2);
            }
        }
        else if (hp / max_hp < 0.25f && hp / max_hp > 0)
        {
            healthbar.color = new Color32(255, 0, 0, 255);
            if (isPlayer)
            {
                activator.Deactivate(blood1);
                activator.Activate(blood2);
            }
        }
        else if (hp / max_hp <= 0 && isPlayer)
        {
            healthbar.color = new Color32(255, 0, 0, 255);
            activator.Deactivate(blood1);
            activator.Activate(blood2);
            //activator.Activate(deathCanvas);
        }
        //else if (hp / max_hp <= 0 && !isPlayer && appendText)
        //{
        //    appendText.appendKill("Host", "Player");
        //}
    }

    public void Increment()
	{
        if (max_hp > hp)
        {
            hp++;
            UpdateUI();
        }
	}

    public void Add(float health)
	{
        float added = hp + health;
        if(added >= 0 && added <= max_hp)
		{
            hp = added;
		}
        else if(added < 0)
		{
            hp = 0;
		}
        else
		{
            hp = max_hp;
		}
        UpdateUI();
	}

    public void Decrement()
	{
        if (hp > 0)
        {
            hp--;
            UpdateUI();
        }
	}

    public void Subtract(float health)
	{
        float subtracted = hp - health;
        if (subtracted >= 0 && subtracted <= max_hp)
        {
            hp = subtracted;
        }
        else if (subtracted < 0)
        {
            hp = 0;
        }
        else
        {
            hp = max_hp;
        }
        UpdateUI();
    }

    public void SetHP(float health)
	{
        if (health <= max_hp && health >= 0)
		{
            hp = health;
            UpdateUI();
		}
	}

    public float GetHP()
	{
        return hp;
	}

    // Update is called once per frame
    void Update()
    {}
}
