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

    private float hp;
    private float max_hp;
    // Start is called before the first frame update
    void Start()
    {
        hp = slider.maxValue;
        max_hp = hp;
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Backspace))
		{
            hp--;
            hpText.text = $"{hp} hp";
            slider.value = hp;
            if(hp/max_hp < 0.75f && hp/max_hp > 0.25f)
			{
                healthbar.color = new Color32(255, 255, 0, 255);
			}
            else if(hp/max_hp < 0.25f && hp/max_hp > 0)
			{
                healthbar.color = new Color32(255, 0, 0, 255);
			}
            else if(hp/max_hp <= 0)
			{
                SceneManager.LoadScene(loseScene);
			}
		}
    }
}
