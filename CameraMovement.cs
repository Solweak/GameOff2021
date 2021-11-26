using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform playerTransform;

    private void Update()
    {
        if (transform.position.x - playerTransform.position.x > 2)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerTransform.position.x - 2, playerTransform.position.y+2, -10), Time.deltaTime /2);
        }
        else if (playerTransform.position.x - transform.position.x  > 2)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(playerTransform.position.x + 2, playerTransform.position.y+2, -10), Time.deltaTime /2);
        }
    }
}