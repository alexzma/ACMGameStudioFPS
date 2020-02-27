using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    public Texture2D image;
    private bool isChanged;
    // Start is called before the first frame update
    void Start()
    {
        ChangeCursor(image);
        isChanged = true;
    }

    // Update is called once per frame
    void Update()
    {}

    void OnDestroy()
	{
        ResetCursor();
	}

    public bool IsChanged()
	{
        return isChanged;
	}

    public void ResetCursor()
	{
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        isChanged = false;
	}

    public void ChangeCursor(Texture2D image)
	{
        Cursor.SetCursor(image, Vector2.zero, CursorMode.Auto);
        isChanged = true;
	}
}
