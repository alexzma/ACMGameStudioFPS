using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
    public Activator activator;
    public LifeTracker lifeTracker;
    public LifeTracker teamMate1Life;
    public LifeTracker teamMate2Life;
    public LifeTracker teamMate3Life;
    public LifeTracker teamMate4Life;
    public GameObject menu;
    public GameObject teamDisplay;
    public SetCursor setCursor;
    public Texture2D image;
    bool menuShowing;
    bool teamShowing;

    // Start is called before the first frame update
    void Start()
    {
        menuShowing = false;
        teamShowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") && !teamShowing)
        {
            menuShowing = !menuShowing;
            activator.ToggleActivate(menu);
            setCursor.ChangeCursor(image);
        }
		if (Input.GetKeyDown("tab") && !menuShowing)
		{
            teamShowing = !teamShowing;
            activator.ToggleActivate(teamDisplay);
			if (setCursor.IsChanged())
			{
                setCursor.ResetCursor();
            } else
			{
                setCursor.ChangeCursor(image);
			}
		}
        if (Input.GetKeyDown(KeyCode.Backspace))
		{
            lifeTracker.Decrement();
		}
        if (Input.GetKeyDown("1"))
		{
            teamMate1Life.Decrement();
		}
        if (Input.GetKeyDown("2"))
        {
            teamMate2Life.Decrement();
        }
        if (Input.GetKeyDown("3"))
        {
            teamMate3Life.Decrement();
        }
        if (Input.GetKeyDown("4"))
        {
            teamMate4Life.Decrement();
        }
    }
}
