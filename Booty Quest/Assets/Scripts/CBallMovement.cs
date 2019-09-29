using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By: Aaron Peneueta

public class CBallMovement : MonoBehaviour {


    private bool m_hitSomething = false;
    private Rigidbody m_rigidbody;

    private int m_cannonballDmg = 0;

	// Use this for initialization
	void Start () 
    {
        print(m_cannonballDmg);
    }
	
    public int Damage
    {
        get
        {
            return m_cannonballDmg;
        }
        set
        {
            m_cannonballDmg = value;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // [Mike]TODO: Handle collision for anything damagable
        // Hit the player and damaged the player
        if(other.gameObject.tag == "Player" && !m_hitSomething)
        {
            m_hitSomething = true;
            other.gameObject.GetComponent<PlayerController>().DamageHealth(m_cannonballDmg);
            Destroy(this.gameObject);

        }

        //[Mike]: hit and damage enemy
        if (other.gameObject.tag == "Enemy" && !m_hitSomething)
        {
            m_hitSomething = true;
            other.gameObject.GetComponent<EnemyShipAI>().Health -= (m_cannonballDmg / other.gameObject.GetComponent<EnemyShipAI>().Armor);
            Destroy(this.gameObject);

        }

        //[Mike] hit and damage merchant
        if (other.gameObject.tag == "Merchant" && !m_hitSomething)
        {
            m_hitSomething = true;
            other.gameObject.GetComponent<MerchantShipAI>().Health -= (m_cannonballDmg / other.gameObject.GetComponent<MerchantShipAI>().Armor);
            Destroy(this.gameObject);

        }

        // Hit a collision object
        Destroy(this.gameObject);
    }
}
