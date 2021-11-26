using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject baseMonster;

    public float lifePoints;
    Controller controller;
    Transform bugTarget;
    public bool locked;
    Vector2 randomPos;

    Vector2 target;

    public int up = 1;
    private bool jump;

    public bool isBoss;
    public bool isCocoon;
    public bool isSpidey;
    public int cocoonSize;
    bool stunned;

    public float speed;
    private bool invu;

    public Transform bodySprite;

    public AudioClip[] screams;
    AudioSource audioSource;
    private bool jumped;

    public GameObject prefabParticule;

    public Transform EyeToClose;

    public AudioClip screamBoss1;
    public AudioClip screamBoss2;
    private bool screamed;
    public float distance = 0;
    public Collider2D collider2DMob;

    private void Awake()
    {
        controller = FindObjectOfType<Controller>();
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        if (isBoss)
        {
            audioSource.PlayOneShot(screamBoss1, 0.5f);
        }
    }

    private void Start()
    {
        if (!isSpidey)
        {
            bugTarget = controller.targets[Random.Range(0, controller.targets.Length)];
            if (bugTarget == controller.targets[0])
            {
                randomPos = Random.insideUnitCircle * bugTarget.GetComponent<BugTarget>().radius;
            }
            else
            {
                randomPos = new Vector2(0, Random.Range(-1.99f, 1.99f));
            }

            locked = false;
            target = Vector2.zero;
            jump = false;
            stunned = false;
            invu = true;
            jumped = false;
            screamed = true;
            hitcount = 0;
            if (!isBoss)
            {
                if (isCocoon)
                {
                    speed = Random.Range(speed * 0.5f, speed * 1f);
                }
                else
                {
                    speed = Random.Range(speed * 0.5f, speed * 1.5f);
                }
            }

            StartCoroutine(InvuDuration());
            if (Random.Range(0, 2) == 0 || isBoss)
            {
                up = 1;
                if (!isBoss && !isCocoon)
                {
                    bodySprite.localPosition -= Vector3.up * 0.25f;
                }
            }
            else
            {
                up = -1;
                if (!isBoss && !isCocoon)
                {
                    bodySprite.localPosition += Vector3.up * 0.4f;
                }
            }

            cocoonSize = Random.Range(0, 10);
        }
    }

    private void Update()
    {
        if (!locked && !isSpidey)
        {
            distance = 0;

            if (bugTarget.transform.position.x > transform.position.x)
            {
                distance = Mathf.Abs(bugTarget.transform.position.x - transform.position.x);
            }
            else
            {
                distance = Mathf.Abs(transform.position.x - bugTarget.transform.position.x);
            }

            if (distance < 1f || jump)
            {
                if (!isBoss && !isCocoon)
                {
                    if (!jumped)
                    {
                        jumped = true;
                        audioSource.loop = false;
                        audioSource.PlayOneShot(screams[Random.Range(0, screams.Length)]);
                    }

                    jump = true;
                    transform.SetParent(bugTarget);
                    target = bugTarget.transform.TransformPoint(randomPos);
                    transform.position = Vector2.MoveTowards(transform.position,
                    bugTarget.transform.TransformPoint(randomPos), 35 * Time.deltaTime);

                    if (Vector2.Distance(target, transform.position) < 0.01f)
                    {
                        controller.SetSpeed(-0.1f);
                        StartCoroutine(DamageStick());
                        locked = true;
                    }
                }
                if (isCocoon)
                {
                    GetHit(100, true);
                }
            }
            else if (!jump && !stunned)
            {
                transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(bugTarget.transform.position.x, transform.position.y), speed * Time.deltaTime);

                RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up * up, Mathf.Infinity, LayerMask.GetMask("Ground"));

                // If it hits something...
                if (hit.collider != null)
                {
                    transform.position = new Vector2(transform.position.x, hit.point.y + 0.25f * up);
                }

                if (isBoss && distance <= 13 && screamed)
                {
                    audioSource.PlayOneShot(screamBoss2, 1f);
                    screamed = false;
                }
                if (isBoss && distance >= 17)
                {
                    screamed = true;
                }
            }
        }
    }

    IEnumerator DamageStick()
    {
        while (true)
        {
            controller.SetLifePoints(-1);
            yield return new WaitForSeconds(1);
        }
    }

    public void GetHit(float nbDegat, bool hatch = false)
    {
        if (!invu)
        {

            audioSource.PlayOneShot(screams[Random.Range(0, screams.Length)], Random.Range(0.3f,1f));

            lifePoints -= nbDegat;
            if (isBoss)
            {
                if (hitcount <= 5)
                {
                    EyeToClose.localPosition -= Vector3.up * 0.01f;
                    audioSource.PlayOneShot(screamBoss1, 1);
                    stunned = true;
                    StartCoroutine(StunDuration());
                }
                else
                {
                    EyeToClose.localPosition -= Vector3.up * 0.01f;
                    audioSource.PlayOneShot(screamBoss1, 1);
                    GameOver();
                }
            }
            GameObject particul = Instantiate(prefabParticule, transform);
            particul.transform.SetParent(null);
            Destroy(particul, 0.4f);
            if (lifePoints <= 0)
            {
                GameOver(hatch);
            }
        }
    }

    public void GameOver(bool hatch = false)
    {
        if (!isBoss)
        {
            if (locked)
            {
                controller.SetSpeed(0.1f);
            }

            if (isCocoon)
            {
                GameManager.instance.nbCocoonSticky -= 1;
                ShopManager.instance.RefreshEnergyMoney(50);
                controller.SetLifePoints(5);
            }
            else if (!isBoss)
            {
                GameManager.instance.nbSticky -= 1;
                ShopManager.instance.RefreshEnergyMoney(10);
                controller.SetLifePoints(3);
            }

            GameManager.instance.score += 1;

            if (isCocoon && hatch)
            {
                GameManager.instance.nbSticky += cocoonSize;
                for (int i = 0; i < cocoonSize; i++)
                {
                    GameObject mob_temp = Instantiate(baseMonster, null);
                    mob_temp.transform.position = transform.position + Vector3.right * Random.Range(-0.7f, 0.7f);
                    mob_temp.GetComponent<EnemyManager>().up = up;
                }
            }
            GameObject particul = Instantiate(prefabParticule, transform);
            particul.transform.SetParent(null);
            Destroy(particul, 0.4f);
            if (isSpidey)
            {
                GameManager.instance.nbSpidey -= 1;
                Destroy(this.gameObject.transform.parent.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);
            GameManager.instance.StopAllCoroutines();

            Collider2D[] enemyList = Physics2D.OverlapCircleAll(transform.position, 50);
            if (enemyList != null)
            {
                foreach (Collider2D item in enemyList)
                {
                    if (item.CompareTag("enemy"))
                    {
                        item.GetComponentInParent<EnemyManager>().GetHit(100);
                    }
                }
            }

            StartCoroutine(GameManager.instance.EndWin());
        }
    }

    int hitcount;

    IEnumerator StunDuration()
    {
        hitcount++;
        collider2DMob.enabled = false;
        Vector3 tmp = EyeToClose.localPosition;
        EyeToClose.localPosition = new Vector3(EyeToClose.localPosition.x, 0.2f, 0);

        yield return new WaitForSeconds(5);
        collider2DMob.enabled = true;

        EyeToClose.localPosition = tmp;

        stunned = false;
    }

    IEnumerator InvuDuration()
    {
        yield return new WaitForSeconds(0.25f);
        invu = false;
    }
}