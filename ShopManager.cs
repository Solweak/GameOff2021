using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    private static ShopManager _instance;
    public static ShopManager instance { get { return _instance; } }

    [HideInInspector]
    public GameObject panelShop;

    Shoot shoot;
    public bool unlocked;

    GameManager gameManager;
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
        shoot = FindObjectOfType<Shoot>();
        gameManager = GameManager.instance;
    }

    public Button sawDmgBtn;
    public Image[] sawDamages;
    public Button sawRadiusBtn;
    public Image[] sawRadius;

    public Button spikeDamageBtn;
    public Image[] spikeDamages;
    public Button spikeFireRateBtn;
    public Image[] spikeFireRate;

    public Button wipeBombBtn;
    public Image[] wipeBomb;
    public GameObject prefabWipeBomb;

    public int moneyEnergy;
    public Text textMoneyEnergy;

    public Button quitButton;

    public Color buyColored;
    public Color emptyColored;

    public float spikeDamage;
    public float coolDownSpeed;
    public int nbWipeBomb;

    public float saw_damageSec;
    public float coolDownSaw;
    public float radiusAttack;

    private void Start()
    {
        RefreshEnergyMoney(10500);
        panelShop = transform.GetChild(0).gameObject;
        quitButton.onClick.AddListener(QuitShop);
        ActualizePlayerWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            panelShop.SetActive(!panelShop.activeInHierarchy);
        }
    }

    void QuitShop()
    {
        GameManager.instance.ActivePlayer();
        ActualizePlayerWeapon();
        panelShop.SetActive(false);
    }

    void ChangeToBuy(Image img)
    {
        img.color = buyColored;
    }

    public void RefreshEnergyMoney(int energyMoneyAdd)
    {
        moneyEnergy += energyMoneyAdd;
        if (moneyEnergy < 0)
        {
            moneyEnergy = 0;
        }

        textMoneyEnergy.text = moneyEnergy.ToString();
    }

    public void NotEnoughMoney()
    {
        Debug.Log("Not enough money");
    }

    public void BuyTriSpike()
    {
        if (moneyEnergy < 500)
        {
            NotEnoughMoney();
        }
        else
        {
            unlocked = true;
            RefreshEnergyMoney(-500);
        }
    }

    public void BuyDamageSpike(int spikeUp)
    {
        if (moneyEnergy < 250)
        {
            NotEnoughMoney();
        }
        else
        {
            int index = 0;
            for (int i = 0; i < spikeDamages.Length; i++)
            {
                if (spikeDamages[i].color != buyColored)
                {
                    index = i;
                    break;

                }
            }
            ChangeToBuy(spikeDamages[index]);
            spikeDamage += spikeUp;
            RefreshEnergyMoney(-250);
            if (index == 4)
            {
                spikeDamageBtn.gameObject.SetActive(false);
            }
        }
    }

    public void BuyFireRateSpike(float spikeRateUp)
    {
        if (moneyEnergy < 250)
        {
            NotEnoughMoney();
        }
        else
        {
            int index = 0;
            for (int i = 0; i < spikeFireRate.Length; i++)
            {
                if (spikeFireRate[i].color != buyColored)
                {
                    index = i;
                    break;

                }
            }
            ChangeToBuy(spikeFireRate[index]);
            coolDownSpeed += spikeRateUp;
            RefreshEnergyMoney(-250);
            if (index == 4)
            {
                spikeFireRateBtn.gameObject.SetActive(false);
            }
        }
    }

    public void BuyDamageSaw(int sawDamageUp)
    {
        if (moneyEnergy < 250)
        {
            NotEnoughMoney();
        }
        else
        {
            int index = 0;
            for (int i = 0; i < sawDamages.Length; i++)
            {
                if (sawDamages[i].color != buyColored)
                {
                    index = i;
                    break;
                }
            }
            ChangeToBuy(sawDamages[index]);
            saw_damageSec = sawDamageUp;
            RefreshEnergyMoney(-250);
            if (index == 4)
            {
                sawDmgBtn.gameObject.SetActive(false);
            }
        }
    }

    public void BuySizeSaw(float newSize)
    {
        if (moneyEnergy < 250)
        {
            NotEnoughMoney();
        }
        else
        {
            int index = 0;
            for (int i = 0; i < sawRadius.Length; i++)
            {
                if (sawRadius[i].color != buyColored)
                {
                    index = i;
                    break;

                }
            }
            ChangeToBuy(sawRadius[index]);
            //GameManager.instance.playerController.circularSaw.transform.localScale += Vector3.one * newSize;
            radiusAttack += newSize;
            ActualizePlayerWeapon();
            //GameManager.instance.playerController.shootParent.transform.localPosition += Vector3.up * (newSize * 0.42f);
            RefreshEnergyMoney(-250);
            if (index == 4)
            {
                sawRadiusBtn.gameObject.SetActive(false);
            }
        }
    }

    public void BuyWipeBomb()
    {
        if (moneyEnergy < 50)
        {
            NotEnoughMoney();
        }
        else if(nbWipeBomb < 3)
        {
            int index = 0;
            for (int i = 0; i < wipeBomb.Length; i++)
            {
                if (wipeBomb[i].color != buyColored)
                {
                    index = i;
                    wipeBomb[i].color = buyColored;
                    break;
                }
            }
            nbWipeBomb += 1;
            Instantiate(prefabWipeBomb, GameManager.instance.playerController.wipeBombHolder[index]);
            RefreshEnergyMoney(-50);
            if (index == 2)
            {
                wipeBombBtn.gameObject.SetActive(false);
            }
        }
    }

    public void UseWipeBomb()
    {
        wipeBombBtn.gameObject.SetActive(true);
        for (int i = 2; i >= 0; i--)
        {
            if (wipeBomb[i].color == buyColored)
            {
                wipeBomb[i].color = emptyColored;
                break;

            }
        }
    }
    
    public void ActualizePlayerWeapon()
    {
        shoot = FindObjectOfType<Shoot>();
        gameManager = GameManager.instance;
        GameManager.instance.playerController.circularSaw.transform.localScale = Vector3.one * radiusAttack;
        GameManager.instance.playerController.shootParent.transform.localPosition = Vector3.up * (radiusAttack * 0.42f);
    }
}