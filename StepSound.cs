using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
    GameManager gameManager;
    AudioSource audioSource;
    public AudioClip[] stepSounds;
    bool coolDown;
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioSource = gameManager.GetComponent<AudioSource>();
        coolDown = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ground") && coolDown)
        {
            coolDown = false;
            StartCoroutine(Cooldown());
            AudioManager.instance.PlayOneShot(stepSounds[Random.Range(0,stepSounds.Length)],0.1f);
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.2f);
        coolDown = true;
    }
}