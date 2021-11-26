using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventToTrigger : MonoBehaviour
{
    public int indexEvent;
    public bool disableAfterEvent = true;
    public bool destroyAfterEvent = false;

    Controller controller;

    private void Awake()
    {
        controller = FindObjectOfType<Controller>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (indexEvent)
            {
                case 0:
                    GameManager.instance.StartDialog("start");
                    break;
                case 1:
                    GameManager.instance.SpawWorm();
                    break;
                case 2:
                    StartCoroutine(GameManager.instance.SpawnerSticky());
                    StartCoroutine(GameManager.instance.SpawnerCocoonSticky());
                    break;
                case 21:
                    StartCoroutine(GameManager.instance.SpawnerSticky());
                    StartCoroutine(GameManager.instance.SpawnerCocoonSticky());
                    StartCoroutine(GameManager.instance.SpawnerSpidey());
                    break;
                case 3:
                    StartCoroutine(GameManager.instance.GameOver());
                    break;
                case 4:
                    controller.SetLifePoints(200);
                    StartCoroutine(Absorbed());
                    break;
                case 6:
                    GameManager.instance.Win1();
                    StartCoroutine(GoDown());
                    break;
                case 100:
                    GameManager.instance.StartDialog("start");
                    break;
                case 101:
                    GameManager.instance.StartDialog("start1");
                    controller.LockPlayer();
                    break;
                case 102:
                    GameManager.instance.StartDialog("start2");
                    controller.LockPlayer();
                    break;
                case 103:
                    GameManager.instance.StartDialog("start3");
                    controller.LockPlayer();
                    break;
            }
            if (disableAfterEvent)
            {
                GetComponent<Collider2D>().enabled = false;
            }
            if (destroyAfterEvent)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator GoDown()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.down * 10, Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator Absorbed()
    {
        Color currentColor = GetComponent<SpriteRenderer>().color;
        while (true)
        {
            currentColor = Color.Lerp(currentColor, Color.white, Time.deltaTime);
            yield return null;
        }
    }
}