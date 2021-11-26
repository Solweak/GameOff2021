using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    private static PauseManager _instance;
    public static PauseManager instance { get { return _instance; } }

    [HideInInspector]
    public GameObject panelPause;

    public bool isPaused;

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
        DontDestroyOnLoad(instance);
        panelPause = transform.GetChild(0).gameObject;
        isPaused = false; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Tab))
        {
            panelPause.SetActive(!panelPause.activeInHierarchy);
            isPaused = panelPause.activeInHierarchy;
            if (panelPause.activeInHierarchy)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }
}