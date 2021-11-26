using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombLaunch : MonoBehaviour
{
    Rigidbody2D rb;
    Shoot shoot;

    ScreenShake screenShake;

    private void Awake()
    {
        screenShake = FindObjectOfType<ScreenShake>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        shoot = FindObjectOfType<Shoot>();
    }

    private void Update()
    {
        if (rb.velocity.y < 0)
        {
            StartCoroutine(Explosion());
        }
    }

    public void Launching()
    {
        shoot.isLaunchingBomb = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1;
        rb.AddForce(Vector2.up * 400);
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(0.5f);
        Collider2D[] enemyList = Physics2D.OverlapCircleAll(transform.position, 8);
        if (enemyList != null)
        {
            foreach (Collider2D item in enemyList)
            {
                if (item.CompareTag("enemy"))
                {
                    item.GetComponentInParent<EnemyManager>().GetHit(100);
                }
            }
        }
        screenShake.ShakeDura(0.5f);
        Destroy(gameObject);
        shoot.isLaunchingBomb = false;
    }
}