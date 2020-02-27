using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
    public Activator activator;
    public GameObject menu;
    public GameObject teamDisplay;
    public SetCursor setCursor;
    public Texture2D image;

    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            activator.ToggleActivate(menu);
            setCursor.ChangeCursor(image);
        }
		if (Input.GetKeyDown("tab"))
		{
            activator.ToggleActivate(teamDisplay);
			if (setCursor.IsChanged())
			{
                setCursor.ResetCursor();
            } else
			{
                setCursor.ChangeCursor(image);
			}
		}
    }
}
