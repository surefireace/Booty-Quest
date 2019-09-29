using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

// By Michael Taylor

public class UILayer : MonoBehaviour
{
    // singleton instance
    private static UILayer s_instance = null;
    
    // references
    private GameObject m_shopUI;
    private GameObject m_shipUI;
    private GameObject m_mainUI;
    private GameObject m_playerHealthMeter;

    // UI elements
    private Text m_gameLogText;
    private Text m_ammoText;
    private Text m_islandText;
    private Canvas m_baseHealthMeter;
    private Dictionary<int, ResourceMeter> m_meters;

    // script objects
    private PlayerController m_player;
    private List<GameObject> m_enemies;
    private List<GameObject> m_merchants;

    private float m_logTextTimer = 5.0f;
    private int m_logCounter = 0;

    static public UILayer Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = GameObject.FindGameObjectWithTag("MainUI").GetComponent<UILayer>();
            }
            return s_instance;
        }
    }

    // initial setup and sanity checking
    private void Awake ()
    {
        // singleton sanity check
        if (s_instance == null)
        {
            s_instance = this;
        }

        // setup references
        m_mainUI = GameObject.FindGameObjectWithTag("MainUI");
        if (!m_mainUI)
            Debug.LogError("MainUI not instantiated!");

        m_meters = new Dictionary<int, ResourceMeter>();

        // sets unless in pirate island
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex != 1)
        {
            m_baseHealthMeter = GameObject.FindGameObjectWithTag("ResourceMeter").GetComponent<Canvas>();
            if (!m_baseHealthMeter)
                Debug.LogError("ResourceMeter not instantiated!");

            m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            if (!m_player)
                Debug.LogError("Player not instantiated!");

            m_enemies = new List<GameObject>();
            m_merchants = new List<GameObject>();
        }

        // sets up for pirate island only
        if (sceneIndex == 1)
        {
            m_shopUI = GameObject.Find("MainUI/ShopUI");
            if (!m_shopUI)
                Debug.LogError("UpgradeUI not instantiated!");
            m_shopUI.gameObject.SetActive(false);
        }

        m_playerHealthMeter = GameObject.FindGameObjectWithTag("PlayerHealth");
        if (!m_playerHealthMeter)
            Debug.LogError("PlayerShipHealth not instantiated!");

        m_shipUI = GameObject.Find("MainUI/ShipUI");
        if (!m_shipUI)
            Debug.LogError("ShipUI not instantiated!");

        // setup UI elements
        m_ammoText = GameObject.Find("MainUI/ShipUI/AmmoText").GetComponent<Text>();
        if (!m_ammoText)
            Debug.LogError("AmmoText not instantiated!");

        m_islandText = GameObject.Find("MainUI/ShipUI/IslandText").GetComponent<Text>();
        if (!m_islandText)
            Debug.LogError("IslandText not instantiated!");

        m_gameLogText = GameObject.Find("MainUI/GameLogUI/LogText").GetComponent<Text>();
        if (!m_gameLogText)
            Debug.LogError("IslandText not instantiated!");

        m_meters.Add(m_playerHealthMeter.GetInstanceID(), m_playerHealthMeter.GetComponent<ResourceMeter>());
        m_meters.Add(m_shipUI.GetInstanceID(), m_shipUI.GetComponent<ResourceMeter>());
    }

    // Update is called once per frame
    private void Update ()
    {
        m_logTextTimer -= Time.deltaTime;
        if (m_logTextTimer <= 0.0f)
        {
            m_gameLogText.text = "";
            m_logTextTimer = 5.0f;
        }

        ResourceMeter playerHealth = m_meters[m_playerHealthMeter.GetInstanceID()];
        ResourceMeter shipTreasure = m_meters[m_shipUI.GetInstanceID()];

        playerHealth.m_value = (int)PlayerShipData.Health;
        playerHealth.m_maxValue = (int)PlayerShipData.MaxHealth;
        shipTreasure.m_value = PlayerShipData.Treasure;
        shipTreasure.m_maxValue = PlayerShipData.MaxShipTreasure;

        m_ammoText.text = "Ammo: " + PlayerShipData.CurrAmmo.ToString() + "/" + PlayerShipData.MaxAmmo.ToString();
        m_islandText.text = "Pirate Bank: " + PirateIsland.BankTreasure.ToString();
        
        // grab current scene index
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // updates unless in pirate island
        if (sceneIndex != 1)
        {
            // enemy health meters
            for (int i = 0; i < m_enemies.Count; ++i)
            {
                GameObject enemy = m_enemies[i];

                // check if ship has been destroyed
                if (enemy)
                {
                    ResourceMeter enemyMeter = m_meters[enemy.GetInstanceID()];

                    // check if damaged
                    if (enemy.GetComponent<EnemyShipAI>().Health < enemy.GetComponent<EnemyShipAI>().MaxHealth)
                    {
                        enemyMeter.gameObject.SetActive(true);
                    }
                    else
                    {
                        enemyMeter.gameObject.SetActive(false);
                    }

                    enemyMeter.m_value = (int)enemy.GetComponent<EnemyShipAI>().Health;
                    enemyMeter.m_maxValue = (int)enemy.GetComponent<EnemyShipAI>().MaxHealth;
                    Vector3 enemyPos = enemy.gameObject.transform.position;
                    enemyMeter.transform.position = new Vector3(enemyPos.x, enemyMeter.transform.position.y, enemyPos.z);
                    enemyMeter.transform.LookAt(Camera.main.transform);
                }
            }

            // merchant health meters
            for (int i = 0; i < m_merchants.Count; ++i)
            {
                GameObject merchant = m_merchants[i];

                // check if ship has been destroyed
                if (merchant)
                {
                    ResourceMeter meter = m_meters[merchant.GetInstanceID()];

                    // check if damaged
                    if (merchant.GetComponent<MerchantShipAI>().Health < merchant.GetComponent<MerchantShipAI>().MaxHealth)
                    {
                        meter.gameObject.SetActive(true);
                    }
                    else
                    {
                        meter.gameObject.SetActive(false);
                    }

                    meter.m_value = (int)merchant.GetComponent<MerchantShipAI>().Health;
                    meter.m_maxValue = (int)merchant.GetComponent<MerchantShipAI>().MaxHealth;
                    Vector3 pos = merchant.GetComponent<MerchantShipAI>().transform.position;
                    meter.transform.position = new Vector3(pos.x, meter.transform.position.y, pos.z);
                    meter.transform.LookAt(Camera.main.transform);
                }
            }
        }
    }

    //----------------------------------------------------------------//
    // Helper functions                                               //
    //----------------------------------------------------------------//

    private void CreateMeter(int id, Transform transform, float offset = 25)
    {
        m_baseHealthMeter.gameObject.SetActive(true);
        Vector3 location = transform.position + new Vector3(0, offset, 0);
        ResourceMeter healthMeter = Instantiate(m_baseHealthMeter.GetComponentInChildren<ResourceMeter>(), location, Quaternion.identity, transform);
        m_meters.Add(id, healthMeter);
        m_baseHealthMeter.gameObject.SetActive(false);
    }

    //----------------------------------------------------------------//
    // Global functions                                               //
    //----------------------------------------------------------------//
    public void RemoveEnemy(GameObject enemy)
    {
        m_enemies.Remove(enemy);
        m_meters.Remove(enemy.GetInstanceID());
    }

    public void RemoveMerchant(GameObject merchant)
    {
        m_merchants.Remove(merchant);
        m_meters.Remove(merchant.GetInstanceID());
    }

    public void AddEnemy(GameObject enemy, float offset)
    {
        m_enemies.Add(enemy);
        CreateMeter(enemy.GetInstanceID(), enemy.GetComponent<EnemyShipAI>().transform, offset);
    }

    public void AddMerchant(GameObject merchant, float offset)
    {
        m_merchants.Add(merchant);
        CreateMeter(merchant.GetInstanceID(), merchant.GetComponent<MerchantShipAI>().transform, offset);
    }

    public void ToggleShopUI()
    {
        if (m_shopUI.activeSelf)
            m_shopUI.SetActive(false);
        else
            m_shopUI.SetActive(true);
    }

    public bool UpgradeUIOpened()
    {
        return m_shopUI.activeSelf;
    }

    public void WriteToGameLog(string str)
    {
        if (m_logCounter == 6)
        {
            m_gameLogText.text = "";
            m_logCounter = 0;
        }

        if (str != "")
        {
            m_gameLogText.text += str + "\n";
            m_logTextTimer += 0.1f;
            ++m_logCounter;
        }
    }
}
