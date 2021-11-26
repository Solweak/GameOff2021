using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{

    public DialogManager dialogManager;

    private static GameManager _instance;

    public static GameManager instance { get { return _instance; } }

    [HideInInspector]
    public Controller controller;

    public SpawnPlacement spawnRight;
    public SpawnPlacement spawnLeft;

    public GameObject prefabMonsterBase;
    public GameObject prefabCocoon;

    public GameObject prefabWorms;
    public GameObject prefabSpider;

    public ObjectTween[] teeths;
    public AudioClip bossScream;

    public GameObject menuEnd;

    public ObjectTween fader;
    bool chrono;
    public float chronoL;
    public int score;

    public Text chronoText;
    public Text scoreText;
    public GameObject levelDesign;
    public GameObject elevator;
    bool end;

    [Header("Limite nb Mob")]
    public int limitSticky;
    public int limitCocoonSticky;
    public int limitSpidey;

    public int nbSticky;
    public int nbCocoonSticky;
    public int nbSpidey;

    [Header("Energy Stats")]
    public int globalEnergy;
    public int levelEnergy;

    [Header("Weapon Stats")]
    public PlayerController playerController;
    public Shoot shoot;

    public bool firstShop;
    public ObjectTween tweenBlackScreen;

    /*
     * Correction :
     * 
     * Déplacement bestiole = Règle de level de design pas de parroi à la vertical pour le plafond
     * 
     * Premier Plan - esthétique, ne pas géner le joueur ou très très peu
     * Faire variante pour infinteLevel & Boss
     * Sprite Minerai Background
     * Changer le curseur, mettre un réticule
     * Panel explication système vie / énergie etc
     * Sprite araignée
     * Sprite sticky - ajout dent rouge
     * Sprite Cocoon - ajout corps rouge
     * Sprite Worm
     * 
     * Particule bomb
     *  Bruit bomb
     *  Bruit sourd
     * Bruit Absorption minerai
     * Anim Absorption
     * 
     * 
     * Particule sticky - vert + rouge
     * Particule cocoon - vert + rouge
     * Particule spider - violet
     * 
     * 
     * 
     * Ajout :
     * Vrai boss à tuer
     * 
     * Intro dialog + son
     * 
     * Début :
     *  Intro mission
     *  
     *  Event premier caillou
     *  
     *  Controls Weapon + Pause
     * 
     *  Arrivée mob
     *  
     *Début 2
     *  Shop Ouverture
     *  
     *Début 3 
     *  Boss
     * 
     */

    private void Awake()
    {
        RefreshRef();
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        dialogManager = FindObjectOfType<DialogManager>();
    }

    void Start()
    {
        firstShop = true;
        chrono = true;
        score = 0;
        end = false;
        tweenBlackScreen.Close();
        StartCoroutine(StartGameFromElevetor());
        dialogManager.gameObject.SetActive(false);
        MusicThemeManager.instance.GoToCalm();
    }

    public void RefreshRef()
    {
        controller = FindObjectOfType<Controller>();
        playerController = FindObjectOfType<PlayerController>();
        shoot = FindObjectOfType<Shoot>();
        GameObject.Find("MasterCanvas").transform.GetChild(3).gameObject.SetActive(true);
        menuEnd = GameObject.Find("MenuParent");
        spawnRight = GameObject.Find("SpawnRight").GetComponent<SpawnPlacement>();
        spawnLeft = GameObject.Find("SpawnLeft").GetComponent<SpawnPlacement>();
        fader = GameObject.Find("BlackFadingScreen").GetComponent<ObjectTween>();
        chronoText = menuEnd.transform.GetChild(4).GetComponent<Text>();
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        levelDesign = GameObject.Find("____LevelDesign____");
        elevator = GameObject.Find("EndWin");
        menuEnd.SetActive(false);
    }

    private void Update()
    {
        if (chrono)
        {
            chronoL += Time.deltaTime;
        }
        if (nbSpidey <= 2 &&
            nbCocoonSticky <= 2 &&
            nbSticky <= 6)
        {
            MusicThemeManager.instance.GoToCalm();
        }
        else if (nbSpidey <= 4 &&
            nbCocoonSticky <= 4 &&
            nbSticky <= 10)
        {
            MusicThemeManager.instance.GoToDangerous();
        }
        else
        {
            MusicThemeManager.instance.GoToMortal();
        }
    }

    IEnumerator StartGameFromElevetor()
    {
        yield return new WaitForSeconds(0.5f);
        controller._rb.velocity = Vector2.zero;
        controller._rb.gravityScale = 0;
        controller.isInteracting = true;
        controller.GetComponentInChildren<SpiderProceduralAnimation>().enabled = false;
    }

    public void ActivePlayer()
    {
        controller = FindObjectOfType<Controller>();
        playerController = FindObjectOfType<PlayerController>();
        shoot = FindObjectOfType<Shoot>();
        controller._rb.velocity = Vector2.zero;
        controller._rb.gravityScale = 1;
        controller._rb.AddForce(Vector2.right * 50);
        controller.isInteracting = false;
        controller.GetComponentInChildren<SpiderProceduralAnimation>().enabled = true;
    }

    public IEnumerator StartFirstDialog()
    {
        yield return new WaitForSeconds(2f);
        StartDialog("start");
    }

    public void EndCurrentDialog(string dialogTag)
    {
        dialogManager.gameObject.SetActive(false);
        controller.isInteracting = false;
        switch (dialogTag)
        {
            case "start":
                ActivePlayer();
                break;
            case "start1":
                Debug.Log($"End of dialog {dialogTag}");
                StartCoroutine(controller.LifeRegenAuto());
                break;
            case "start2":
                Debug.Log($"End of dialog {dialogTag}");
                break;
            case "start3":
                Debug.Log($"End of dialog {dialogTag}");
                break;
            case "shop":
                Debug.Log($"End of dialog {dialogTag}");
                ShopManager.instance.panelShop.SetActive(true);
                break;
            case "gameOver":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            case "finalEnd":
                SceneManager.LoadScene("MenuScene");
                break;
            default:
                Debug.Log($"End of dialog {dialogTag}");
                break;
        }
    }

    IEnumerator StartDialogAfterTime(string tag, float time)
    {
        yield return new WaitForSeconds(time);
        StartDialog(tag);
    }

    public void StartDialoguingAfterTime(string tag, float time)
    {
        StartCoroutine(StartDialogAfterTime(tag, time));
    }

    public void StartDialog(string tag)
    {
        //Debug.Log("Starting dialog : " + tag + " and lock the character");
        dialogManager.gameObject.SetActive(true);
        dialogManager.SetDialog(Dialogs.dialogs[tag], tag);
        controller.isInteracting = true;
    }

    public IEnumerator GameOver()
    {
        
        controller.isInteracting = true;
        AudioManager.instance.PlayOneShot(bossScream, 1);
        teeths[0].Open();
        teeths[1].Open();
        AudioManager.instance.FadeOut();
        end = true;
        Destroy(levelDesign);
        menuEnd.SetActive(true);
        StopAllCoroutines();
        yield return new WaitForSeconds(3);
        controller.GameOver();
    }

    public void Win1()
    {
        end = true;
        Destroy(levelDesign);
        /*scoreText.enabled = true;
        chronoText.enabled = true;
        string minutes = Mathf.Floor(chronoL / 60).ToString("00");
        string seconds = (chronoL % 60).ToString("00");

        scoreText.text = "Enemy slaughtered : " + score.ToString();
        chronoText.text = "Chrono : " + minutes + " minutes " + seconds + " seconds ";*/
        chrono = false;
        controller.GetComponent<Rigidbody2D>().gravityScale = 0;
        controller.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        controller.transform.SetParent(elevator.transform);
        controller.GetComponentInChildren<SpiderProceduralAnimation>().enabled = false;
        controller.isInteracting = true;
        fader.Close();
        menuEnd.SetActive(true);
        StopAllCoroutines();
    }

    public IEnumerator SpawnerSticky()
    {
        Debug.Log("Spawn Sticky");
        while (true && !end)
        {
            if (nbSticky <= limitSticky)
            {
                GameObject tmpMob;
                if (Random.Range(0, 2) == 0)
                {
                    if (spawnLeft.spawnable)
                    {
                        tmpMob = Instantiate(prefabMonsterBase, spawnLeft.transform);
                        tmpMob.transform.localPosition = Vector3.zero;
                        tmpMob.transform.SetParent(levelDesign.transform);
                        nbSticky++;
                    }
                }
                else
                {
                    if (spawnRight.spawnable)
                    {
                        tmpMob = Instantiate(prefabMonsterBase, spawnRight.transform);
                        tmpMob.transform.localPosition = Vector3.zero;
                        tmpMob.transform.SetParent(levelDesign.transform);
                        nbSticky++;
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 2.01f));
        }
    }

    public IEnumerator SpawnerCocoonSticky()
    {
        Debug.Log("Spawn Cocoon");
        while (true && !end)
        {
            if (nbCocoonSticky <= limitCocoonSticky && nbSticky <= limitSticky)
            {

                GameObject tmpMob;
                if (Random.Range(0, 2) == 0)
                {
                    if (spawnLeft.spawnable)
                    {
                        tmpMob = Instantiate(prefabCocoon, spawnLeft.transform);
                        tmpMob.transform.localPosition = Vector3.zero;
                        tmpMob.transform.SetParent(levelDesign.transform);
                        nbCocoonSticky++;
                    }
                }
                else
                {
                    if (spawnRight.spawnable)
                    {
                        tmpMob = Instantiate(prefabCocoon, spawnRight.transform);
                        tmpMob.transform.localPosition = Vector3.zero;
                        tmpMob.transform.SetParent(levelDesign.transform);
                        nbCocoonSticky++;
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(1.01f, 3.01f));
        }
    }

    public IEnumerator SpawnerSpidey()
    {
        Debug.Log("Spawn Spidey");
        while (true && !end)
        {
            if (nbSpidey <= limitSpidey)
            {

                GameObject tmpMob;
                if (Random.Range(0, 2) == 0)
                {
                    if (spawnLeft.spawnable)
                    {
                        tmpMob = Instantiate(prefabSpider, spawnLeft.transform);
                        tmpMob.transform.localPosition = Vector3.zero;
                        tmpMob.transform.SetParent(levelDesign.transform);
                        nbSpidey++;
                    }
                }
                else
                {
                    if (spawnRight.spawnable)
                    {
                        tmpMob = Instantiate(prefabSpider, spawnRight.transform);
                        tmpMob.transform.localPosition = Vector3.zero;
                        tmpMob.transform.SetParent(levelDesign.transform);
                        nbSpidey++;
                    }
                }
            }
            yield return new WaitForSeconds(Random.Range(1.01f, 3.01f));
        }
    }

    public void SpawWorm()
    {
        GameObject tmpMob;

        tmpMob = Instantiate(prefabWorms, spawnLeft.transform);
        tmpMob.transform.localPosition = Vector3.zero;
        tmpMob.transform.SetParent(null);
    }

    public void SpawSpider()
    {
        GameObject tmpMob;

        tmpMob = Instantiate(prefabSpider, spawnLeft.transform);
        tmpMob.transform.localPosition = Vector3.zero;
        tmpMob.transform.SetParent(levelDesign.transform);
    }

    public IEnumerator EndWin()
    {
        yield return new WaitForSeconds(5);
        StartDialog("finalEnd");
    }

}