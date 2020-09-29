using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppendText : MonoBehaviour
{
    private Text m_text;

    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {}

    public void appendKill(string killer, string killed)
	{
        m_text.text = killer + " killed " + killed + "\n" + m_text.text;
	}

    public void killStreak(string killer, int streak)
	{
        m_text.text = killer + " now has a kill streak of " + streak.ToString() + "\n" + m_text.text;
	}
}
