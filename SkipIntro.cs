using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkipIntro : MonoBehaviour
{
    public GameObject skipIntroText;

    private void Start()
    {
        StartCoroutine("EndIntro");
        MusicThemeManager.instance.GoToDangerous();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            if (skipIntroText.activeInHierarchy)
            {
                StopAllCoroutines();
                SceneManager.LoadScene("Play");
            }
            else
            {
                skipIntroText.SetActive(true);
                StartCoroutine(HideIntroText());
            }
        }
    }

    IEnumerator HideIntroText()
    {
        yield return new WaitForSeconds(3f);
        skipIntroText.SetActive(false);
    }

    IEnumerator EndIntro()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("Play");
    }
}