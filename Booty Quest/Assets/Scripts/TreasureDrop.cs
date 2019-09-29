using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Michael Taylor

public class TreasureDrop : MonoBehaviour
{
    // tunables
    private int m_treasure = 0;

    public int TreasureValue
    {
        set
        {
            m_treasure = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if player hits add to their treasure and destroy itself
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().AddTreasure(m_treasure);
            Destroy(this.gameObject);
        }
    }
}
