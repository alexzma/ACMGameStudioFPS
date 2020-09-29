using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Music : MonoBehaviour
{
    AudioSource[] sources = new AudioSource[2];
    float fade_speed = 0.5f;
    int songNumber = -1;
    public bool ddol;

    private void Awake()
    {
        if (ddol)
            DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (songNumber == -1)
        {
            songNumber++;
            return;
        }
        else
            ChangeMusic();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioSource[] sourceList = GetComponents<AudioSource>();
        int i = 0;
        foreach (AudioSource audio in sourceList)
        {
            sources[i] = sourceList[i];
            i++;
        }
    }

    private IEnumerator FadeMusic(AudioSource source)
    {
        if (source.volume <= 0)
        {
            source.Stop();
            yield break;
        }

        float lower = Time.deltaTime * fade_speed;
        if (source.volume <= lower)
        {
            source.volume = 0;
            source.Stop();
        }
        else
        {
            source.volume -= Time.deltaTime * fade_speed;
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(FadeMusic(source));
        }
    }

    public void SetVolume(float volume)
    {
        sources[0].volume = volume;
    }

    public void ChangeSecondary(float volume)
    {
        sources[1].volume = volume;
    }

    public void ChangeMusic()
    {
        StartCoroutine(FadeMusic(sources[songNumber]));

        songNumber = (songNumber + 1) % 2;
        sources[songNumber].Play();
    }
}
