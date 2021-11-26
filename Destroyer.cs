using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyManager enemyManager = collision.GetComponent<EnemyManager>();
        if (enemyManager != null)
        {
            if (!enemyManager.isBoss)
            {
                if (enemyManager.isCocoon)
                {
                    GameManager.instance.nbCocoonSticky -= 1;
                    Destroy(collision.gameObject);
                }
                else if (enemyManager.isSpidey)
                {
                    GameManager.instance.nbSpidey -= 1;
                    Destroy(collision.gameObject.transform.parent.parent.gameObject);
                }
                else
                {
                    GameManager.instance.nbSticky -= 1;
                    Destroy(collision.gameObject);
                }
                
            }
        }
    }
}
