using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSound : MonoBehaviour
{
    public AudioClip clip;
    private AudioSource source;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        source.spatialBlend = 1;
        source.clip = clip;
    }

    public void PlaySound()
    {
        source.Play();
    }
}
