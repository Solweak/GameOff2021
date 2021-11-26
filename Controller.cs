using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour
{
    public float _speed = 1.2f;
    public float currentSpeed;
    public float lifePointsMax;
    public float currentlifePoints;

    public Rigidbody2D _rb;

    public bool isInteracting;

    public Transform[] targets;

    public SpriteRenderer[] spritePlayerColor;
    public Color fullLifeColor;
    public Color endLifeColor;

    public Color currentColor;

    public int verticalForce;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        verticalForce = 50;
        lifePointsMax = 200;
        currentlifePoints = lifePointsMax;
        _speed = 1.2f;
        currentSpeed = 1.2f;
        _rb.AddForce(Vector2.right * 50f);
        ChangeColor();
        

        if (SceneManager.GetActiveScene().name == "Play")
        {
            SetLifePoints(-199);
            StartCoroutine(GameManager.instance.StartFirstDialog());
            ShopManager.instance.panelShop.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "PlayInfinite")
        {
            GameManager.instance.RefreshRef();
            StartCoroutine(LifeRegenAuto());
            if (GameManager.instance.firstShop)
            {
                GameManager.instance.StartDialog("shop");
                ShopManager.instance.panelShop.SetActive(false);
                GameManager.instance.firstShop = false;
            }
            else
            {
                ShopManager.instance.panelShop.SetActive(true);
            }
        }

    }
    void FixedUpdate()
    {
        if (Mathf.Abs(_rb.velocity.x) < currentSpeed && !isInteracting)
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                _rb.AddForce(Vector2.right * verticalForce);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A))
            {
                _rb.AddForce(-Vector2.right * verticalForce);
            }
        }
    }

    public void SetLifePoints(int delta)
    {
        currentlifePoints += delta;

        if (currentlifePoints <= 0)
        {
            currentlifePoints = 0;
            isInteracting = true;
        }
        if (currentlifePoints > lifePointsMax)
        {
            currentlifePoints = lifePointsMax;
        }
        ChangeColor();
    }

    public void SetSpeed(float delta)
    {
        _speed = _speed + delta;
        currentSpeed = Mathf.Clamp(_speed, 0.1f, 1.2f);
    }

    void ChangeColor()
    {
        currentColor = Color.Lerp(fullLifeColor, endLifeColor, 1 - (currentlifePoints / lifePointsMax));
        foreach (var item in spritePlayerColor)
        {
            item.color = currentColor;
        }
    }

    public IEnumerator LifeRegenAuto()
    {
        while (true)
        {
            if (currentSpeed == 1.2f)
            {
                SetLifePoints(5);
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void GameOver()
    {
        ShopManager.instance.RefreshEnergyMoney(-1000);
        GameManager.instance.tweenBlackScreen.Open();
        GameManager.instance.StartDialog("gameOver");
    }

    public void LockPlayer()
    {
        _rb.velocity = Vector2.zero;
        isInteracting = true;
    }
}