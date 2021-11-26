using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float xlimit;
    public float ylimitMax;
    public float ylimitMini;
    public float maxDistance;

    Controller controller;
    Vector2 savedPos;
    AudioSource audioSource;

    public GameObject circularSaw;
    public GameObject shootParent;

    private bool coolDowndSawBool;
    public Image sawImage;

    public AudioClip sawInHeat;

    public Transform[] wipeBombHolder;
    public bool isLaunchingBomb;
    private void Awake()
    {
        controller = FindObjectOfType<Controller>();
        audioSource = GetComponent<AudioSource>();
        coolDowndSawBool = true;
    }
    private void Start()
    {
        ShopManager.instance.coolDownSaw = 2f;
    }

    void Update()
    {
        if (!PauseManager.instance.isPaused)
        {
            if (!controller.isInteracting)
            {
                Vector2 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Ray2D x = new Ray2D(transform.parent.position, Input.mousePosition - (Camera.main.WorldToScreenPoint(transform.parent.position)));
                float distanceFromCanon = Vector2.Distance(transform.parent.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                distanceFromCanon = Mathf.Clamp(distanceFromCanon, 0, maxDistance);

                Vector2 temp = x.GetPoint(distanceFromCanon);
                temp.x = Mathf.Clamp(temp.x, transform.parent.position.x - xlimit, transform.parent.position.x + xlimit);
                temp.y = Mathf.Clamp(temp.y, transform.parent.position.y - ylimitMini, transform.parent.position.y + ylimitMax);

                transform.position = Vector2.Lerp(transform.position, temp, 20 * Time.deltaTime);

                Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.parent.position;
                diff.Normalize();

                float rot_z = (Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg - 90);

                transform.rotation = Quaternion.Euler(0f, 0f, rot_z);

                /*if (Camera.main.ScreenToWorldPoint(Input.mousePosition).y < transform.parent.position.y)
                {
                    transform.rotation = Quaternion.Euler(0f, 0f, -rot_z + 180);
                }*/
            }

            if (Input.GetMouseButtonDown(0) && !coolDowndSawBool)
            {
                AudioManager.instance.PlayOneShot(sawInHeat, 0.5f, 2f);
            }

            if (Input.GetMouseButtonDown(0) && !controller.isInteracting && ShopManager.instance.coolDownSaw > 0.01f && coolDowndSawBool)
            {
                StartCoroutine("Attack");
                audioSource.mute = false;
                ShopManager.instance.coolDownSaw -= 0.5f;
                if (controller.currentlifePoints > 50)
                {
                    controller.SetLifePoints(-10);
                }
            }

            if (Input.GetMouseButton(0) && !controller.isInteracting && ShopManager.instance.coolDownSaw > 0.01f && coolDowndSawBool)
            {
                circularSaw.transform.Rotate(new Vector3(0, 0, 720) * Time.deltaTime);
                ShopManager.instance.coolDownSaw -= Time.deltaTime;

                if (ShopManager.instance.coolDownSaw <= 0.01f)
                {
                    ShopManager.instance.coolDownSaw = 0;
                    coolDowndSawBool = false;
                    StartCoroutine("CoolDawnSaw");
                }
            }
            else if (coolDowndSawBool)
            {
                ShopManager.instance.coolDownSaw += Time.deltaTime;
                ShopManager.instance.coolDownSaw = Mathf.Clamp(ShopManager.instance.coolDownSaw, 0, 2);
            }

            if (Input.GetMouseButtonUp(0) || ShopManager.instance.coolDownSaw <= 0.01f)
            {
                StopCoroutine("Attack");
                audioSource.mute = true;
            }

            sawImage.fillAmount = ShopManager.instance.coolDownSaw / 2;
        }
    }

    IEnumerator CoolDawnSaw()
    {
        AudioManager.instance.PlayOneShot(sawInHeat, 0.5f, 2f);

        yield return new WaitForSeconds(2);
        coolDowndSawBool = true;
        ShopManager.instance.coolDownSaw = 0.02f;
    }

    IEnumerator Attack()
    {
        while (true)
        {
            Collider2D[] enemyList = Physics2D.OverlapCircleAll(transform.position, ShopManager.instance.radiusAttack);
            if (enemyList != null)
            {
                foreach (Collider2D item in enemyList)
                {
                    if (item.CompareTag("enemy"))
                    {
                        item.GetComponentInParent<EnemyManager>().GetHit(ShopManager.instance.saw_damageSec/0.2f);
                    }
                }
            }
            if (controller.currentlifePoints > 50)
            {
                controller.SetLifePoints(-1);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}