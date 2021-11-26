using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicThemeManager : MonoBehaviour
{
    private static MusicThemeManager _instance;
    public static MusicThemeManager instance { get { return _instance; } }

    public AudioSource[] themeLayers;

    public string currentState;

    // 1 base 
    // 2 hit hat 
    // 3 voice
    // 4 bassdrum
    // 5 dynamic
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(instance);
    }

    public void StartFadeIn(int layer, float volumeMax = 1)
    {
        StartCoroutine(FadeIn_Co(themeLayers[layer -1], volumeMax));
    }

    public void StartFadeOut(int layer)
    {
        StartCoroutine(FadeOut_Co(themeLayers[layer -1]));
    }

    IEnumerator FadeOut_Co(AudioSource audioSource)
    {
        while (audioSource.volume > 0.001f)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime);
            yield return null;
        }
        audioSource.volume = 0;
    }

    IEnumerator FadeIn_Co(AudioSource audioSource, float volumeMax = 1)
    {
        while (audioSource.volume < (volumeMax - 0.01))
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, volumeMax, Time.deltaTime);
            yield return null;
        }
        audioSource.volume = volumeMax;
    }

    public void GoToCalm()
    {
        currentState = "calm";
        StopAllCoroutines();
        for (int i = 2; i < 6; i++)
        {
            StartFadeOut(i);
        }
    }

    public void GoToDangerous()
    {
        currentState = "dangerous";
        StopAllCoroutines();

        StartFadeIn(1, 0.5f);
        StartFadeIn(3, 0.5f);
        StartFadeOut(5);
    }

    public void GoToMortal()
    {
        currentState = "mortal";
        StopAllCoroutines();

        StartFadeIn(1, 0.5f);
        StartFadeOut(3);
        StartFadeIn(5);
    }

    
}