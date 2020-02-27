using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour
{
    public Activator activator;
    public GameObject menu;
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
    }
}
