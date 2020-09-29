using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
    public Activator activator;
    public LifeTracker lifeTracker;
    public TeamTracker teamTracker;
    public GameObject menu;
    public GameObject teamDisplay;
    public SetCursor setCursor;
    public Texture2D image;
    public float max_health = 10;

    public bool menuShowing;
    public bool teamShowing;
    bool[] playerPortrait;
    int[] playerHealth;

    private IEnumerator TeamHealthCoroutine;
    private float HEALTH_UPDATE_TIME = 0.1f;
    public PlayerScript _playerScript;

    // Start is called before the first frame update
    void Start()
    {
        menuShowing = false;
        teamShowing = false;
        playerPortrait = new bool[10];
        for(int i = 0; i < playerPortrait.Length; i++)
		{
            playerPortrait[i] = false;
		}
        playerHealth = new int[10];
        for (int i = 0; i < playerHealth.Length; i++)
        {
            playerHealth[i] = 10;
        }
    }

    // Update is called once per frame
    void Update()
    {
        menuShowing = menu.activeInHierarchy;
        if (Input.GetKeyDown("escape") && !teamShowing)
        {
            menuShowing = !menuShowing;
            activator.ToggleActivate(menu);
        }
        if (menuShowing)
        {
            setCursor.ShowCursor();
        } else
        {
            setCursor.HideCursor();
        }
		if (Input.GetKeyDown("tab") && !menuShowing)
		{
            teamShowing = !teamShowing;

            /////////////////////////////////////////
            // Updates team healths if teamShowing
            if (teamShowing)
            {
                TeamHealthCoroutine = UpdateHealths();
                StartCoroutine(TeamHealthCoroutine);
            }
            else
                StopAllCoroutines();
            //////////////////////////////////////////

            activator.ToggleActivate(teamDisplay);
			if (Cursor.visible)
			{
                setCursor.HideCursor();
            } else
			{
                setCursor.ShowCursor();
			}
		}
        /*if (Input.GetKeyDown(KeyCode.Backspace))
		{
            if (lifeTracker.GetHP() > 0)
                lifeTracker.Decrement();
            else
                lifeTracker.SetHP(max_health);
		}*/
        /*if (Input.GetKeyDown("1"))
		{
			if (!playerPortrait[0])
			{
                teamTracker.AddPlayer("Player 1");
                playerPortrait[0] = true;
			}
            else if(playerHealth[0] > 0)
			{
                playerHealth[0]--;
                teamTracker.SetHealth("Player 1", playerHealth[0]);
			} else if(playerHealth[0] == 0)
			{
                teamTracker.RemovePlayer("Player 1");
                playerPortrait[0] = false;
                playerHealth[0] = 10;
			}
		}
        if (Input.GetKeyDown("2"))
        {
            if (!playerPortrait[1])
            {
                teamTracker.AddPlayer("Player 2");
                playerPortrait[1] = true;
            }
            else if (playerHealth[1] > 0)
            {
                playerHealth[1]--;
                teamTracker.SetHealth("Player 2", playerHealth[1]);
            }
            else if (playerHealth[1] == 0)
            {
                teamTracker.RemovePlayer("Player 2");
                playerPortrait[1] = false;
                playerHealth[1] = 10;
            }
        }*/
    }

    private IEnumerator UpdateHealths()
    {
        _playerScript.UpdateTeamHealth();
        yield return new WaitForSeconds(HEALTH_UPDATE_TIME);
        TeamHealthCoroutine = UpdateHealths();
        StartCoroutine(TeamHealthCoroutine);
    }
}
