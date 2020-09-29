using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    public float wait_time = 1;
    bool destroy;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        destroy = false;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
		if (destroy)
		{
            time += Time.deltaTime;
            if (time > wait_time)
            {
                Destroy(this.gameObject);
            }
		}
    }

    void Awake()
	{
        //DontDestroyOnLoad(this.gameObject);
	}

    public void Play()
	{
        AudioSource audio = this.gameObject.GetComponent<AudioSource>();
        audio.Play();
    }

    public void PlayDestroy()
	{
        AudioSource audio = this.gameObject.GetComponent<AudioSource>();
        audio.Play();
        destroy = true;
	}
}
