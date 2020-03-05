using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
    public Activator activator;
    public LifeTracker lifeTracker;
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
    }
}
