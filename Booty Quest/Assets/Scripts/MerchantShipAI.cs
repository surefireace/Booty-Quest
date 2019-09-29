using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

//By Donovan Colen

// required components
[RequireComponent(typeof(NavMeshAgent))]


public class MerchantShipAI : MonoBehaviour
{

    //tuneables
    [SerializeField]
    private float m_maxHealth = 100;      // the maximum health for the ship
    [SerializeField]
    private GameObject m_treasureDrop;      // the treasure drop
    [SerializeField]
    private int m_treasure = 0;      // the amount of treasure the ship is carrying

    public enum MerchType
    {
        k_poor,
        k_wealthy,
        k_royalty,
        k_count
    }

    //private variables
    private NavMeshAgent m_navAgent = null;     // the nav mesh agent for the ship. it determines the speed, stopping distance, ect.. 
    private GameObject m_destination = null;      // the destination for the cargo
    private float m_health = 100;     // the ships current health
    private float m_armor = 1;
    private MerchType m_merchType;

    // Use this for initialization
    void Start ()
    {
        m_navAgent = GetComponent<NavMeshAgent>();

        if(m_destination == null)
        {
            Debug.LogError("no destination set");
        }
        m_navAgent.destination = m_destination.transform.position;
        m_health = m_maxHealth;
    }

    public float Health
    {
        get
        {
            return m_health;
        }
        set
        {
            m_health = value;
        }

    }

    public float Armor
    {
        get
        {
            return m_armor;
        }
        set
        {
            m_armor = value;
        }
    }


    public float MaxHealth
    {
        get
        {
            return m_maxHealth;
        }
    }

    public MerchType MerchantType
    {
        get
        {
            return m_merchType;
        }
        set
        {
            m_merchType = value;
        }
    }

    public void SetDestination(GameObject position)
    {
        m_destination = position;
    }

    // Update is called once per frame
    void Update ()
    {
		if(m_navAgent.remainingDistance < m_navAgent.stoppingDistance)
        {
            AIManager.Instance.DestroyMerchant(gameObject, false);
        }
        gameObject.transform.LookAt(m_destination.transform.position);

        // check health
        if (m_health <= 0)
        {
            // destroy ship
            Vector3 location = gameObject.transform.position;
            location.y = m_treasureDrop.transform.position.y;
            GameObject clone = Instantiate(m_treasureDrop, location, m_treasureDrop.transform.rotation);
            clone.GetComponent<TreasureDrop>().TreasureValue = m_treasure;
            AIManager.Instance.DestroyMerchant(gameObject, true);
            UILayer.Instance.RemoveEnemy(gameObject);
        }

    }
}
