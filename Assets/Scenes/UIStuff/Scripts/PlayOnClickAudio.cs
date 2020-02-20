using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnClickAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public SetCursor setCursor;
    public GameObject menu;

    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonDown(0) && setCursor.IsChanged() && !menu.activeSelf){
            audioSource.Play();
		}
    }
}
