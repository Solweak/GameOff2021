using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlacement : MonoBehaviour
{
    Controller controller;
    public bool isSpawnLeft;
    public bool spawnable;
    private void Awake()
    {
        controller = FindObjectOfType<Controller>();
    }
    private void Start()
    {
        transform.position = controller.transform.position;
        spawnable = true;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, controller.transform.position);

        if (distance > 14)
        {
            spawnable = true;
        }
        else
        {
            spawnable = false;
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 10, LayerMask.GetMask("Ground"));

        if (hit.collider != null)
        {
            transform.position = new Vector2(transform.position.x, hit.point.y + 1f);
        }
        else
        {
            spawnable = false;
        }
        

        if (isSpawnLeft)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + Vector3.up, Vector2.left, 1, LayerMask.GetMask("Ground"));

            if (hitLeft.collider == null)
            {
                if (distance < 15)
                {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left, 25 * Time.deltaTime);
                }
            }
            else
            {
                transform.position = new Vector2(hitLeft.point.x + 1, hit.point.y + 1f);
            }
        }
        else
        {
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position + Vector3.up, Vector2.right, 1, LayerMask.GetMask("Ground"));

            if (hitRight.collider == null)
            {
                if (distance < 15)
                {
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.right, 25 * Time.deltaTime);
                }
            }
            else
            {
                transform.position = new Vector2(hitRight.point.x - 1, hit.point.y + 1f);
            }
        }
    }
}