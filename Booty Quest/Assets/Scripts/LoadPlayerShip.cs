using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Michael Taylor

public class LoadPlayerShip : MonoBehaviour
{
    private GameObject m_smallShip;
    private GameObject m_bigShip;
    private void Awake()
    {
        m_smallShip = GameObject.Find("Ships/Player Ship");
        m_bigShip = GameObject.Find("Player Ship (Big)");

        if (PlayerShipData.HullUpgraded)
        {
            m_smallShip.SetActive(false);
            m_bigShip.SetActive(true);
            Debug.Log("Hull was Upgraded!");
        }
        else
        {
            m_smallShip.SetActive(true);
            m_bigShip.SetActive(false);
            Debug.Log("Hull not upgraded!");
        }
    }

    private void Update()
    {
        if (PlayerShipData.HullUpgraded)
        {
            m_smallShip.SetActive(false);
            m_bigShip.SetActive(true);
            Debug.Log("Hull was Upgraded!");
        }
        else
        {
            m_smallShip.SetActive(true);
            m_bigShip.SetActive(false);
            Debug.Log("Hull not upgraded!");
        }
    }
}
