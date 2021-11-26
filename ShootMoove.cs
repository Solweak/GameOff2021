using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMoove : MonoBehaviour
{
    public Vector3 moveDirectionPoint;
    bool groundHit;
    public int speed;
    public float damage;
    public AudioClip[] audioClips;
    AudioSource audioSource;

    public float diff = 0;

    // Start is called before the first frame update
    void Start()
    {
        moveDirectionPoint = transform.TransformPoint(transform.localPosition + Vector3.up * 10000 + Vector3.left * diff * 10000);
        transform.SetParent(null);
        groundHit = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (moveDirectionPoint != null && !groundHit)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveDirectionPoint, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("enemy"))
        {
            EnemyManager enemyManager = collision.GetComponentInParent<EnemyManager>();
            if (enemyManager.isBoss)
            {
                Debug.Log("bossHit");
                enemyManager.GetHit(damage);
            }else if (!enemyManager.locked)
            {
                enemyManager.GetHit(damage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            groundHit = true;
            GetComponent<Collider2D>().enabled = false;
            transform.position = collision.collider.ClosestPoint(transform.position);
        }
        if (collision.gameObject.CompareTag("enemyArmor"))
        {
            groundHit = true;
            GetComponent<Collider2D>().enabled = false;
            transform.position = collision.collider.ClosestPoint(transform.position);
            transform.SetParent(collision.gameObject.transform);
        }
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);

    }
}