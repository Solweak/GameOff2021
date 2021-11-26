using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpiderController : MonoBehaviour
{
    public float _speed = 1f;

    private Rigidbody2D _rb;

    
    Controller controller;
    float distance;
    bool attacking;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        controller = FindObjectOfType<Controller>();
    }
    private void Start()
    {
        attacking = false;
    }
    void FixedUpdate()
    {
        distance = 0;

        if (controller.transform.position.x > transform.position.x)
        {
            if (Mathf.Abs(_rb.velocity.x) < _speed)
            {
                _rb.AddForce(Vector2.right * 50f);
            }
        }
        else
        {
            if (Mathf.Abs(_rb.velocity.x) < _speed)
            {
                _rb.AddForce(Vector2.left * 50f);
            }
        }

        if (Mathf.Abs(_rb.velocity.x) <= 0.01f)
        {
            _rb.AddForce(Vector2.up * 50f);
        }

        distance = Vector3.Distance(controller.transform.position, transform.position);

        if (distance < 1 && !attacking)
        {
            attacking = true;
            StartCoroutine("Attack");
        }

        else if(distance > 1 && attacking)
        {
            attacking = false;
            StopCoroutine("Attack");
        }
    }

    

    IEnumerator Attack()
    {
        while (true)
        {
            controller.SetLifePoints(-10);
            yield return new WaitForSeconds(2f);
        }
    }
}