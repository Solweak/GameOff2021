using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GoDown());
    }

    // Update is called once per frame
    IEnumerator GoDown()
    {

        while (transform.position != new Vector3(0.5f, -1.53f, 0))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0.5f, -1.53f, 0), Time.deltaTime * 2);
            yield return null;
        }

        GameManager.instance.ActivePlayer();
    }
}
