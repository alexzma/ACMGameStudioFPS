using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LifeTracker : MonoBehaviour
{
    public Text hpText;
    public Slider slider;
    public string loseScene;
    public Image healthbar;
    public bool isPlayer;

    private float hp;
    private float max_hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = slider.maxValue;
        max_hp = hp;
    }

    void UpdateUI()
	{
        hpText.text = $"{hp} hp";
        slider.value = hp;
        if (hp / max_hp < 0.75f && hp / max_hp > 0.25f)
        {
            healthbar.color = new Color32(255, 255, 0, 255);
        }
        else if (hp / max_hp < 0.25f && hp / max_hp > 0)
        {
            healthbar.color = new Color32(255, 0, 0, 255);
        }
        else if (hp / max_hp <= 0 && isPlayer)
        {
            SceneManager.LoadScene(loseScene);
        }
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
    }

    public void SetHP(float health)
	{
        if (health <= max_hp && health >= 0)
		{
            hp = health;
            UpdateUI();
		}
	}

    // Update is called once per frame
    void Update()
    {}
}
