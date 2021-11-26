using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager instance { get { return _instance; } }

    AudioSource audioSource;

    AudioSource sfxAudioSource;

    public float volumeMax;

    public AudioClip[] audioClips;

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

        this.audioSource = GetComponent<AudioSource>();
        this.sfxAudioSource = transform.GetChild(0).GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip audioclip,float volume = 1, float pitch = 1)
    {
        sfxAudioSource.volume = volume;
        sfxAudioSource.pitch = pitch;
        sfxAudioSource.PlayOneShot(audioclip);
    }

    public void FadeOut()
    {
        StartCoroutine("FadeOut_Co");
    }

    public void FadeIn()
    {
        StartCoroutine("FadeIn_Co");
    }

    public void ChangeMusicWithFade(AudioClip nextAudio)
    {
        StartCoroutine(ChangeMusicWithFade_Co(nextAudio));
    }

    IEnumerator FadeOut_Co()
    {
        while (audioSource.volume > 0.001f)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime);
            yield return null;
        }
        audioSource.volume = 0;
    }

    IEnumerator FadeIn_Co()
    {
        while (audioSource.volume < (volumeMax - 0.01))
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, volumeMax, Time.deltaTime);
            yield return null;
        }
        audioSource.volume = volumeMax;
    }

    IEnumerator ChangeMusicWithFade_Co(AudioClip nextAudio)
    {
        yield return FadeOut_Co();
        audioSource.clip = nextAudio;
        audioSource.Play();
        yield return FadeIn_Co();
    }
}