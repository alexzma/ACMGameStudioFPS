using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Slider slider = GetComponent<Slider>();
        AudioListener.volume = slider.value;
    }

    // Update is called once per frame
    void Update()
    {}

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
