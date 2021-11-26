using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{
    public GameObject prefabShootSpike;
    public GameObject prefabShootTriSpike;

    Controller controller;
    PlayerController playerController;

    public AudioClip[] audioClips;
    public AudioSource audioSource;

    public Image coolDownImgSpike;
    public Image coolDownImgTriSpike;

    bool coolD;

    List<GameObject> spikesShot = new List<GameObject>();
    public int limitSpikeVisible;

    public int WeaponType;
    public bool isLaunchingBomb;

    private void Awake()
    {
        controller = GetComponentInParent<Controller>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        coolD = true;
        coolDownImgSpike.fillAmount = 1;
        spikesShot.Clear();
        WeaponType = 1;
        isLaunchingBomb = false;
        ChangeWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseManager.instance.isPaused)
        {
            if ((Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W)) && ShopManager.instance.unlocked)
            {
                ChangeWeapon();
            }

            if (Input.GetMouseButtonDown(1) && !controller.isInteracting && coolD)
            {
                coolD = false;
                ShotSpike();
            }
            if (Input.GetMouseButton(1) && !controller.isInteracting && coolD)
            {
                coolD = false;
                ShotSpike();
            }
            if (!coolD)
            {
                coolDownImgSpike.fillAmount += ShopManager.instance.coolDownSpeed * Time.deltaTime;
                coolDownImgTriSpike.fillAmount += ShopManager.instance.coolDownSpeed * Time.deltaTime;
            }
            if (coolDownImgSpike.fillAmount >= 1)
            {
                coolD = true;
            }
            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && ShopManager.instance.nbWipeBomb > 0 && !isLaunchingBomb)
            {
                ShotWipeBomb();
            }
        }
    }

    void ChangeWeapon()
    {
        WeaponType = WeaponType + 1;
        if (WeaponType > 1)
        {
            WeaponType = 0;
        }
        coolDownImgSpike.enabled = WeaponType == 0;
        coolDownImgTriSpike.enabled = WeaponType == 1;
    }

    void ShotWipeBomb()
    {
        for (int i = 2; i >= 0; i--)
        {
            if (playerController.wipeBombHolder[i].childCount != 0)
            {

                playerController.wipeBombHolder[i].GetChild(0).GetComponent<BombLaunch>().Launching();
                break;
            }
        }
        ShopManager.instance.nbWipeBomb -= 1;
        ShopManager.instance.UseWipeBomb();
    }

    void ShotSpike()
    {
        ShopManager.instance.RefreshEnergyMoney(-5);
        if (controller.currentlifePoints > 10)
        {
            controller.SetLifePoints(-1);
        }
        coolDownImgSpike.fillAmount = 0;
        coolDownImgTriSpike.fillAmount = 0;
        switch (WeaponType)
        {
            case 0:
                ShootOneSpike();
                break;
            case 1:
                ShootTriSpike();
                break;
            default:
                break;
        }
    }

    void ShootOneSpike()
    {
        GameObject shot = Instantiate(prefabShootSpike, transform.GetChild(0));
        shot.transform.position = transform.GetChild(0).position;
        shot.transform.localPosition += Vector3.up * 0.55f;
        shot.transform.localScale = shot.transform.localScale * 5;
        shot.GetComponent<ShootMoove>().damage = ShopManager.instance.spikeDamage;
        shot.GetComponent<SpriteRenderer>().color = controller.currentColor;
        if (spikesShot.Count >= limitSpikeVisible)
        {
            Destroy(spikesShot[0]);
            spikesShot.Remove(spikesShot[0]);
        }
        spikesShot.Add(shot);
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)], 0.4f);
    }

    void ShootTriSpike()
    {
        GameObject shot = Instantiate(prefabShootTriSpike, transform.GetChild(0));
        shot.transform.position = transform.GetChild(0).position;
        shot.transform.localPosition += Vector3.up * 0.55f;
        shot.transform.localScale = shot.transform.localScale * 5;

        ShootMoove[] shots = shot.GetComponentsInChildren<ShootMoove>();
        foreach (var item in shots)
        {
            item.GetComponent<SpriteRenderer>().color = controller.currentColor;

            item.damage = ShopManager.instance.spikeDamage / 3;
           spikesShot.Add(item.gameObject);
           if (spikesShot.Count >= limitSpikeVisible)
           {
               Destroy(spikesShot[0]);
               spikesShot.Remove(spikesShot[0]);
           }
        }
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)], 0.4f);
    }
}