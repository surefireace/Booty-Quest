using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Donovan Colen

public class AIManager : MonoBehaviour
{

    // singleton
    private static AIManager s_singleton = null;


    //tuneables
    [SerializeField]
    private GameObject m_enemyShip = null;      // the enemy ship prefab
    [SerializeField]
    private GameObject m_enemyBigShip = null;      // the enemy ship (Big) prefab
    [SerializeField]
    private GameObject m_poorMerchantShip = null;      // the poor merchant ship prefab
    [SerializeField]
    private GameObject m_wealthyMerchantShip = null;      // the wealthy merchant ship prefab
    [SerializeField]
    private GameObject m_royaltyMerchantShip = null;      // the royalty merchant ship prefab
    [SerializeField]
    private GameObject[] m_patrolWaypoints = null;      // the waypoints for the enemy ships to patrol
    [SerializeField]
    private GameObject[] m_portWaypoints = null;      // the waypoints for the ports the merchant ships travel between
    [SerializeField]
    private int m_maxEnemyShips = 1;      // the max amount of enemy ships allowed at the start
    [SerializeField]
    private int m_maxMerchShips = 3;      // the max amount of merch ships allowed at the start
    [SerializeField]
    private int m_enemyShipCap = 9;      // the cap to the number of enemy ships allowed in the scene at any point
    [SerializeField]
    private int m_merchShipCap = 9;    // the cap to the number of merch ships allowed in the scene at any point
    [SerializeField]
    private float m_enemyRespawnTime = 10;
    [SerializeField]
    private float m_merchRespawnTime = 5;
    [SerializeField]
    [Range(0, 100)]
    private float m_poorMerchSpawnPercentage = 60;
    [SerializeField]
    [Range(0, 100)]
    private float m_wealthyMerchSpawnPercentage = 30;
    [SerializeField]
    [Range(0, 100)]
    private float m_royaltyMerchSpawnPercentage = 10;
    [SerializeField]
    [Tooltip("this must be 100")]
    private float m_merchTotalSpawnPercent = 0;  // this must be 100
    [SerializeField]
    [Range(0, 100)]
    private float m_smallEnemySpawnPercentage = 80;
    [SerializeField]
    [Range(0, 100)]
    private float m_bigEnemySpawnPercentage = 20;
    [SerializeField]
    [Tooltip("this must be 100")]
    private float m_enemyTotalSpawnPercent = 0;  // this must be 100


    // privite variables
    private List<GameObject> m_enemyShips;      // list of the enemy ships
    private List<GameObject> m_merchShips;      // list of the merchant ships
    private int m_numEnemyShips = 0;        // current number of enemy ships
    private int m_numMerchShips = 0;        // current number of merchant ships
    private GameObject m_spawnedMerch = null;       // parent object for all the spawned merchant ships
    private GameObject m_spawnedEnemy = null;       // parent object for all the spawned enemy ships
    private bool m_canRespawnEnemy = true;
    private bool m_canRespawnMerch = true;
    private float m_armor = 1.0f;
    private float m_maxArmor = 3.5f;



    void OnValidate()
    {
        m_merchTotalSpawnPercent = m_poorMerchSpawnPercentage + m_wealthyMerchSpawnPercentage + m_royaltyMerchSpawnPercentage;
        if (m_merchTotalSpawnPercent != 100)
        {
            Debug.LogError("Mechant ship spawn percentages must add up to 100");

        }

        m_enemyTotalSpawnPercent = m_smallEnemySpawnPercentage + m_bigEnemySpawnPercentage;
        if(m_enemyTotalSpawnPercent != 100)
        {
            Debug.LogError("Enemy ship spawn percentages must add up to 100");

        }
    }

    // Use this for initialization
    void Awake ()
    {
        if (s_singleton == null)
        {
            s_singleton = this;
        }

        if (m_enemyShip == null)
        {
            Debug.LogError("no Enemy ship prefab selected");
        }
        if (m_poorMerchantShip == null)
        {
            Debug.LogError("no poor Merchant ship prefab selected");
        }
        if (m_wealthyMerchantShip == null)
        {
            Debug.LogError("no wealthy Merchant ship prefab selected");
        }
        if (m_royaltyMerchantShip == null)
        {
            Debug.LogError("no royalty Merchant prefab selected");
        }
        if (m_patrolWaypoints == null)
        {
            Debug.LogError("no patrol waypoints selected");
        }
        if (m_portWaypoints == null)
        {
            Debug.LogError("no port waypoints selected");
        }

        m_spawnedMerch = new GameObject("Merchant Ships");
        m_spawnedEnemy = new GameObject("Enemy Ships");

        m_enemyShips = new List<GameObject>();
        m_merchShips = new List<GameObject>();

        while (m_numEnemyShips < m_maxEnemyShips)
        {
            SpawnEnemyShip();
        }
        while (m_numMerchShips < m_maxMerchShips)
        {
            SpawnMerchShip();
        }

        Application.targetFrameRate = 30;

        if(PlayerShipData.HullUpgraded)
        {
            m_armor = 2.5f;
            m_smallEnemySpawnPercentage = 60;
            m_bigEnemySpawnPercentage = 40;

            m_poorMerchSpawnPercentage = 30;
            m_wealthyMerchSpawnPercentage = 45;
            m_royaltyMerchSpawnPercentage = 25;
        }
    }

    static public AIManager Instance
    {
        get
        {
            if (s_singleton == null)
            {
                s_singleton = GameObject.Find("AIManager").GetComponent<AIManager>();
            }
            return s_singleton;
        }
    }

    private void SpawnMerchShip()
    {
        if (m_canRespawnMerch)
        {
            int index = Random.Range(0, m_portWaypoints.Length);
            Vector3 location = m_portWaypoints[index].transform.position;
            GameObject position = m_portWaypoints[index];
            GameObject clone;
            float offset;

            float roll = Random.Range(0.0f, 100.0f);
            if (roll <= m_poorMerchSpawnPercentage)
            {
                location.y = m_poorMerchantShip.transform.position.y;
                clone = Instantiate(m_poorMerchantShip, location, Quaternion.identity, m_spawnedMerch.transform);
                offset = 20;
                clone.GetComponent<MerchantShipAI>().MerchantType = MerchantShipAI.MerchType.k_poor;
            }
            else if (roll > m_poorMerchSpawnPercentage && roll <= (m_poorMerchSpawnPercentage + m_wealthyMerchSpawnPercentage))
            {
                location.y = m_wealthyMerchantShip.transform.position.y;
                clone = Instantiate(m_wealthyMerchantShip, location, Quaternion.identity, m_spawnedMerch.transform);
                offset = 25;
                clone.GetComponent<MerchantShipAI>().MerchantType = MerchantShipAI.MerchType.k_wealthy;

            }
            else
            {
                location.y = m_royaltyMerchantShip.transform.position.y;
                clone = Instantiate(m_royaltyMerchantShip, location, Quaternion.identity, m_spawnedMerch.transform);
                offset = 30;
                clone.GetComponent<MerchantShipAI>().MerchantType = MerchantShipAI.MerchType.k_royalty;

            }

            int newIndex = Random.Range(0, m_portWaypoints.Length);
            while (index == newIndex)
            {
                newIndex = Random.Range(0, m_portWaypoints.Length);
            }
            clone.GetComponent<MerchantShipAI>().SetDestination(m_portWaypoints[newIndex]);
            clone.GetComponent<MerchantShipAI>().Armor = m_armor - 0.2f;
            UILayer.Instance.AddMerchant(clone, offset);
            m_merchShips.Add(clone);
            ++m_numMerchShips;
            if(clone.GetComponent<MerchantShipAI>().MerchantType == MerchantShipAI.MerchType.k_royalty && m_maxMerchShips < m_merchShipCap)
            {
                SpawnEnemyShip(position);
            }
        }
    }

    private void SpawnEnemyShip(GameObject position = null)
    {
        GameObject temp;
        // shuffle the patrol waypoints 
        for (int i = 0; i < m_patrolWaypoints.Length; ++i)
        {
            int randIndex = Random.Range(0, m_patrolWaypoints.Length);
            temp = m_patrolWaypoints[randIndex];
            m_patrolWaypoints[randIndex] = m_patrolWaypoints[i];
            m_patrolWaypoints[i] = temp;
        }
        Vector3 location = m_patrolWaypoints[0].transform.position;

        if(position != null)
        {
            location = position.transform.position;
        }
        GameObject clone = null;
        float offset = 0;

        float roll = Random.Range(0.0f, 100.0f);
        if (roll <= m_smallEnemySpawnPercentage)
        {
            location.y = m_enemyShip.transform.position.y;
            clone = Instantiate(m_enemyShip, location, Quaternion.identity, m_spawnedEnemy.transform);
            offset = 25;
        }
        else
        {
            location.y = m_enemyBigShip.transform.position.y;
            clone = Instantiate(m_enemyBigShip, location, Quaternion.identity, m_spawnedEnemy.transform);
            offset = 30;
        }

        clone.GetComponent<EnemyShipAI>().SetPatrolRoute(m_patrolWaypoints);
        clone.GetComponent<EnemyShipAI>().Armor = m_armor;
        UILayer.Instance.AddEnemy(clone, offset);
        m_enemyShips.Add(clone);
        ++m_numEnemyShips;
        
    }

    public void DestroyMerchant(GameObject ship, bool killed)
    {
        m_merchShips.Remove(ship);
        Destroy(ship);
        --m_numMerchShips;

        // if the merchant was killed security increases
        if (killed)
        {
            ++m_maxEnemyShips;

            if (m_maxEnemyShips > m_enemyShipCap)
            {
                m_maxEnemyShips = m_enemyShipCap;
            }
            m_canRespawnEnemy = false;
            StartCoroutine(RespawnEnemy());
            m_armor += .1f;

            if(m_armor > m_maxArmor)
            {
                m_armor = m_maxArmor;
            }
        }
        else
        {
            ++m_maxMerchShips;

            if (m_maxMerchShips > m_merchShipCap)
            {
                m_maxMerchShips = m_merchShipCap;
            }
        }
        m_canRespawnMerch = false;
        StartCoroutine(RespawnMerch());

    }

    public void DestroyEnemy(GameObject ship)
    {
        m_enemyShips.Remove(ship);
        Destroy(ship);
        --m_numEnemyShips;
        ++m_maxEnemyShips;

        if (m_maxEnemyShips > m_enemyShipCap)
        {
            m_maxEnemyShips = m_enemyShipCap;
        }
        m_canRespawnEnemy = false;
        StartCoroutine(RespawnEnemy());
        m_armor += .1f;

        if (m_armor > m_maxArmor)
        {
            m_armor = m_maxArmor;
        }
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(m_enemyRespawnTime);
        m_canRespawnEnemy = true;
    }

    private IEnumerator RespawnMerch()
    {
        yield return new WaitForSeconds(m_merchRespawnTime);
        m_canRespawnMerch = true;
    }

    // Update is called once per frame
    void Update ()
    {
        if (m_numEnemyShips < m_maxEnemyShips && m_canRespawnEnemy)
        {
            SpawnEnemyShip();
        }
        if (m_numMerchShips < m_maxMerchShips)
        {
            SpawnMerchShip();
        }

    }
}
