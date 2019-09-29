using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Michael Taylor

public class AmmoDrop : MonoBehaviour
{
    // tunables
    private int m_ammo = 0;

    public int AmmoValue
    {
        set
        {
            m_ammo = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if player hits add to their ammo and destroy itself
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().AddAmmo(m_ammo);
            Destroy(this.gameObject);
        }
    }
}
