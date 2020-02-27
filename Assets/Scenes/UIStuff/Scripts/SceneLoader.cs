using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Texture2D image;
    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {}

    public void LoadScene(string scene)
	{
        SceneManager.LoadScene(scene);
    }

    public void ExtractAndLoadScene(Text text)
	{
        LoadScene(text.text);
	}
}
